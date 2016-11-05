using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

public partial class Policy_PolicySupplyEdit : BasePage
{
    public Tb_Policy_Supply policy = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnBack.PostBackUrl = string.Format("PolicySupplyList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            btselect.PostBackUrl = string.Format("CpyList.aspx?policytype=bd&currentuserid={0}", Request["currentuserid"].ToString());
            if (Request["id"] != null & Request["id"] != "")
            {
                ViewState["id"] = Request["id"];
                GetOneplicyinfo();
                this.showbt.Visible = false;
            }
            //获取选择的商家
            if (Session["Cpynames"] != null && Session["Cpynames"]!="")
            {
                ViewState["Cpynames"] = Session["Cpynames"];
                txtCpyName.Text = "";
                string[] values = Session["Cpynames"].ToString().Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    txtCpyName.Text += values[i].ToString().Split('|')[0] + ",";
                }
                txtCpyName.Text = txtCpyName.Text.Trim(',');
                Session["Cpynames"] = null;
            }
            lbsave.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    /// <summary>
    /// 要修改的补点信息
    /// </summary>
    protected void GetOneplicyinfo()
    {
        policy = baseDataManage.CallMethod("Tb_Policy_Supply", "GetById", null, new object[] { ViewState["id"].ToString() }) as Tb_Policy_Supply;
        txtCpyName.Text = policy.CpyName;
        txtFromCityCode.Text = policy.FromCityCode;
        txtToCityCode.Text = policy.ToCityCode;
        txtAirCode.Text = policy.CarryCode;
        txtPolicyPoint.Text = policy.PolicyPoint.ToString();
        rblstate.SelectedValue = policy.State.ToString();
        if (!string.IsNullOrEmpty(policy.A7))
        {
            ListItemCollection listColl = cblPolicy.Items;
            ListItem item = null;
            for (int i = 0; i < listColl.Count; i++)
            {
                item = listColl[i];
                if (policy.A7.Contains(item.Value.Trim()))
                {
                    item.Selected = true;
                }
            }
        }
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbsave_Click(object sender, EventArgs e)
    {
        string msg = "";
        string cblpolicyvalue = ",";
        for (int i = 0; i < cblPolicy.Items.Count; i++)
        {
            if (cblPolicy.Items[i].Selected == true)
                cblpolicyvalue += cblPolicy.Items[i].Value + ",";
        }
        IHashObject parameter = new HashObject();
        Log_Operation logoper = new Log_Operation();
        logoper.ModuleName = "平台政策补点设置";
        logoper.LoginName = mUser.LoginName;
        logoper.UserName = mUser.UserName;
        logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
        logoper.CpyNo = mCompany.UninCode;
        logoper.OrderId = "";
        try
        {
            if (ViewState["id"]!=null)
            {
                parameter.Add("CarryCode",txtAirCode.Text.ToUpper());
                parameter.Add("FromCityCode",txtFromCityCode.Text.ToUpper());
                parameter.Add("ToCityCode",txtToCityCode.Text.ToUpper());
                parameter.Add("PolicyPoint",txtPolicyPoint.Text);
                parameter.Add("State",rblstate.SelectedValue);
                parameter.Add("A7", cblpolicyvalue);
                parameter.Add("id",ViewState["id"]);
                if ((bool)baseDataManage.CallMethod("Tb_Policy_Supply", "Update", null, new object[] { parameter }) == true )
                {
                    msg = "更新成功";
                    logoper.OperateType = "修改";
                    logoper.OptContent = "id=" + ViewState["id"] + "##CarryCode=" + txtAirCode.Text + "##FromCityCode=" + txtFromCityCode.Text + "##ToCityCode=" + txtToCityCode.Text + "##PolicyPoint=" + txtPolicyPoint.Text + "##State=" + rblstate.SelectedValue;
                    new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);
                }
                else
                {
                    msg = "更新失败"; 
                }
            }
            else
            {
                List<string> list = new List<string>();
                if (ViewState["Cpynames"] != null)
                {
                    string[] values = ViewState["Cpynames"].ToString().Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        list.Add("insert into Tb_Policy_Supply(CpyNo,CpyName,CpyType,OperTime,OperLoginName,OperUserName,CarryCode,FromCityCode,ToCityCode,PolicyPoint,State,A7) " +
                                                       "values('" + values[i].ToString().Split('|')[1] + "','" + values[i].ToString().Split('|')[0] + "'," + values[i].ToString().Split('|')[2] + ",'" + DateTime.Now + "','" + mUser.LoginName + "','" + mUser.UserName + "','" + txtAirCode.Text + "','" + txtFromCityCode.Text + "','" + txtToCityCode.Text + "'," + txtPolicyPoint.Text + "," + rblstate.SelectedValue + ",'" + cblpolicyvalue + "')");
                    }
                }
                Session["Cpynames"] = null;
                msg = (bool)baseDataManage.CallMethod("Tb_Policy_Supply", "ExecuteSqlTran", null, new object[] { list }) == true ? "添加成功" : "添加失败";
                
            }
        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
        
      
    }
}