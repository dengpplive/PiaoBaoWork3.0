using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;

public partial class Policy_PolicyShareTakeoffEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.btnBack.PostBackUrl = string.Format("PolicyShareTakeoffList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            if (Request["id"] != null & Request["id"] != "")
            {
                ViewState["id"] = Request["id"];
                GetPolicyshareinfo();
                this.showbt.Visible = false;
                this.ddljk.Enabled = false;
            }
            //判断是否为平台登录（修改的内容不同）
            if (mCompany.RoleType == 1 )
	        {
                hidroletype.Value = "平台";
                this.showpb.Visible = true;
                this.txtPolicyPoint.Text = "0";
                this.txtPolicyMoney.Text = "0";
                showcpyname.Visible = true;
	        }
            else
	        {
                this.showpolicy.Visible = true;
                this.txtPbPoint.Text = "0";
                this.txtPbMoney.Text = "0";
	        }
            
            //获取选择的商家
            if (Request["names"] != null)
            {
                txtCpyName.Text = "";
                string[] values = Request["names"].Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    txtCpyName.Text += values[i].ToString().Split('|')[0] + ",";
                }
                txtCpyName.Text = txtCpyName.Text.Trim(',');
            }
            Bindjk();
            lbsave.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    /// <summary>
    /// 绑定接口
    /// </summary>
    protected void Bindjk()
    {
        List<Bd_Base_Dictionary> list = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=24" }) as List<Bd_Base_Dictionary>;
        this.ddljk.DataSource = list;
        ddljk.DataTextField = "ChildName";
        ddljk.DataValueField = "ChildID";
        this.ddljk.DataBind();
    }
    protected void GetPolicyshareinfo()
    {
        Tb_ShareInterface_TakeOff policy = new Tb_ShareInterface_TakeOff();
        policy = baseDataManage.CallMethod("Tb_ShareInterface_TakeOff", "GetById", null, new object[] { ViewState["id"].ToString() }) as Tb_ShareInterface_TakeOff;
        txtCpyName.Text = policy.CpyName;
        ddljk.SelectedValue = policy.PolicySource.ToString();
        txtPolicyMoney.Text = policy.PolicyMoney.ToString();
        txtPolicyPoint.Text = policy.PolicyPoint.ToString();
        txtPbPoint.Text = policy.PbPoint.ToString();
        txtPbMoney.Text = policy.PbMoney.ToString();
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbsave_Click(object sender, EventArgs e)
    {
        string msg = "";
        IHashObject parameter = new HashObject();
        Log_Operation logoper = new Log_Operation();
        logoper.ModuleName = "共享接口扣点设置";
        logoper.LoginName = mUser.LoginName;
        logoper.UserName = mUser.UserName;
        logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
        logoper.CpyNo = mCompany.UninCode;
        logoper.OrderId = "";
        try
        {
            parameter.Add("OperTime", DateTime.Now);
            parameter.Add("OperLoginName", mUser.LoginName);
            parameter.Add("OperUserName", mUser.UserName);
            parameter.Add("PolicySource", ddljk.SelectedValue);
            if (mCompany.RoleType == 1)
            {
                parameter.Add("PbPoint", txtPbPoint.Text);
                parameter.Add("PbMoney", txtPbMoney.Text);
            }
            else
            {
                parameter.Add("PolicyPoint", txtPolicyPoint.Text);
                parameter.Add("PolicyMoney", txtPolicyMoney.Text);
            }
            if (ViewState["id"] != null)
            {
                parameter.Add("id", ViewState["id"]);
                if ((bool)baseDataManage.CallMethod("Tb_ShareInterface_TakeOff", "Update", null, new object[] { parameter }) == true)
                {
                    msg = "更新成功";
                    logoper.OperateType = "修改";
                    logoper.OptContent = "id=" + ViewState["id"];
                    new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);
                }
                else
                {
                    msg = "更新失败";
                }
            }
            else
            {
                if (mCompany.RoleType==1)//平台可多选商家
                {
                   
                        List<string> list = new List<string>();
                        if (Request["names"] != null)
                        {
                            string[] values = Server.HtmlEncode(Request["names"].ToString()).Split(',');
                            for (int i = 0; i < values.Length; i++)
                            {
                                List<Tb_ShareInterface_TakeOff> listIn = baseDataManage.CallMethod("Tb_ShareInterface_TakeOff", "GetList", null, new Object[] { "CpyNo='" + values[i].ToString().Split('|')[1] + "' and PolicySource=" + ddljk.SelectedValue }) as List<Tb_ShareInterface_TakeOff>;
                                if (listIn != null && listIn.Count > 0)
                                {
                                    msg = "["+values[i].ToString().Split('|')[0]+"]用户已添加此接口";
                                }
                                else
                                {
                                    list.Add("insert into Tb_ShareInterface_TakeOff(CpyNo,CpyName,CpyType,OperTime,OperLoginName,OperUserName,PolicySource,PolicyPoint,PolicyMoney,PbPoint,PbMoney) " +
                                                                   "values('" + values[i].ToString().Split('|')[1] + "','" + values[i].ToString().Split('|')[0] + "'," + values[i].ToString().Split('|')[2] + ",'" + DateTime.Now + "','" + mUser.LoginName + "','" + mUser.UserName + "'," + ddljk.SelectedValue + ",'" + txtPolicyPoint.Text.Trim() + "','" + txtPolicyMoney.Text.Trim() + "'," + txtPbPoint.Text.Trim() + "," + txtPbMoney.Text.Trim() + ")");
                                }
                            }
                        }
                        msg = (bool)baseDataManage.CallMethod("Tb_ShareInterface_TakeOff", "ExecuteSqlTran", null, new object[] { list }) == true ? "添加成功" : "添加失败";
                }
                else //商家操作
                {
                    List<Tb_ShareInterface_TakeOff> list = baseDataManage.CallMethod("Tb_ShareInterface_TakeOff", "GetList", null, new Object[] { "CpyNo='" + mCompany.UninCode + "' and PolicySource=" + ddljk.SelectedValue }) as List<Tb_ShareInterface_TakeOff>;
                    if (list!=null && list.Count>0)
                    {
                        msg = "该用户已添加此接口";
                    }
                    else
                    {
                        parameter.Add("CpyNo", mCompany.UninCode);
                        parameter.Add("CpyName", mCompany.UninAllName);
                        parameter.Add("CpyType", mCompany.RoleType);
                        msg = (bool)baseDataManage.CallMethod("Tb_ShareInterface_TakeOff", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
                    }
                }
            }
        }
        catch (Exception)
        {

            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}