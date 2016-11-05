using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using PbProject.Dal.SQLEXDAL;
using System.Data;

public partial class Financial_PosEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnRet.PostBackUrl = string.Format("PosList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            Bind();
            if (Request.QueryString["Id"] != null)
            {
                ViewState["Id"] = Request.QueryString["Id"];
                GetOnePosInfo();
                txtTerminalNo.Enabled = false;
            }
        }
    }
    /// <summary>
    /// 获取要修改的信息
    /// </summary>
    protected void GetOnePosInfo()
    {
        try
        {
            Tb_PosInfo mpos = (baseDataManage.CallMethod("Tb_PosInfo", "GetList", null, new Object[] { "id='" + Guid.Parse(ViewState["Id"].ToString()) + "'" }) as List<Tb_PosInfo>)[0];
            SQLEXDAL_Base sqlexdal_base = new SQLEXDAL_Base();
            DataTable table = sqlexdal_base.ExecuteStrSQL("select UninAllName,UninCode,RoleType,LoginName from User_Company _cpy inner join User_Employees _user on _cpy.UninCode=_user.CpyNo where _user.IsAdmin=0 and UninCode = '" + mpos.CpyNo + "'");
            txtTerminalNo.Text = mpos.PosNo.ToString();
            ddltype.SelectedValue = mpos.PosMode.ToString();
            ddlGYList.Value = table.Rows[0]["UninCode"].ToString() + "-" + table.Rows[0]["RoleType"].ToString();
            txtfeil.Text = mpos.PosRate.ToString();
            ddlGYList.Text = table.Rows[0]["LoginName"].ToString() + "-" + table.Rows[0]["UninAllName"].ToString();
        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// 绑定证件
    /// </summary>
    protected void Bind()
    {
        //绑定pos机类型
        List<Bd_Base_Dictionary> list = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=4 and ChildID in(9,10,11,12,13)" }) as List<Bd_Base_Dictionary>;
        this.ddltype.DataSource = list;
        ddltype.DataTextField = "ChildName";
        ddltype.DataValueField = "ChildID";
        this.ddltype.DataBind();


        SQLEXDAL_Base sqlexdal_base = new SQLEXDAL_Base();
        DataTable table = sqlexdal_base.ExecuteStrSQL("select UninAllName,UninCode,RoleType,LoginName from User_Company _cpy inner join User_Employees _user on _cpy.UninCode=_user.CpyNo where _user.IsAdmin=0 and  len(UninCode) > 12 and UninCode like '" + mCompany.UninCode + "%' order by LoginName");
        ddlGYList.DataTableSource = table;
        ddlGYList.DataFiledText = "LoginName-UninAllName";
        ddlGYList.DataFiledValue = "UninCode-RoleType";
        ddlGYList.DataBind();
        //绑定
        //List<User_Company> listcpy = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "len(UninCode) > 12 and UninCode like '" + mCompany.UninCode + "%'" }) as List<User_Company>;
        //for (int i = 0; i < listcpy.Count; i++)
        //{
        //    listcpy[i].UninCode = listcpy[i].UninCode + "^" + listcpy[i].RoleType;
        //}
        //ddlcpy.DataSource = listcpy;
        //ddlcpy.DataTextField = "UninAllName";
        //ddlcpy.DataValueField = "UninCode";
        //ddlcpy.DataBind();
        //hidCustomervalue(listcpy);
        ///////////////////////////////
        //SQLEXDAL_Base sqlexdal_base = new SQLEXDAL_Base();
        //DataTable table = sqlexdal_base.ExecuteStrSQL("select UninAllName,UninCode,RoleType,LoginName from User_Company _cpy inner join User_Employees _user on _cpy.UninCode=_user.CpyNo where _user.IsAdmin=0 and  len(UninCode) > 12 and UninCode like '" + mCompany.UninCode + "%'");
        //ddlcpy_0.Items.Clear();
        //ddlcpy_0.Items.Add(new ListItem("---请选择用户---", ""));
        //if (table != null && table.Rows.Count > 0)
        //{
        //    foreach (DataRow dr in table.Rows)
        //    {
        //        ListItem li = new ListItem(dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString(), dr["UninCode"].ToString() + "@" + dr["LoginName"].ToString() + "@" + dr["UninAllName"].ToString()+"@"+dr["RoleType"].ToString());
        //        ddlcpy_0.Items.Add(li);
        //    }
        //}



    }
    /// <summary>
    /// 添加pos机
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBind_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            List<User_Employees> list = baseDataManage.CallMethod("User_Employees", "GetList", null, new Object[] { "CpyNo='" + ddlGYList.Value.Split('-')[0].ToString() + "'" }) as List<User_Employees>;

            IHashObject parameter = new HashObject();
            if (list != null && list.Count > 0)
            {
                parameter.Add("CpyLoginName", list[0].LoginName);
            }
            parameter.Add("CpyNo", ddlGYList.Value.Split('-')[0].ToString());
            parameter.Add("CpyName", ddlGYList.Text.Split('-')[1].ToString());
            parameter.Add("CpyType", ddlGYList.Value.Split('-')[1].ToString());
            parameter.Add("OperTime", Convert.ToDateTime(DateTime.Now));
            parameter.Add("OperCpyNo", mCompany.UninCode);
            parameter.Add("OperCpyName", mCompany.UninAllName);
            parameter.Add("OperLoginName", mUser.LoginName);
            parameter.Add("OperUserName", mUser.UserName);
            parameter.Add("PosMode", int.Parse(ddltype.SelectedValue));
            parameter.Add("PosNo", txtTerminalNo.Text.Trim());
            parameter.Add("PosRate", txtfeil.Text);
            Log_Operation logoper = new Log_Operation();
            logoper.ModuleName = "pos机管理";
            logoper.LoginName = mUser.LoginName;
            logoper.UserName = mUser.UserName;
            logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
            logoper.CpyNo = mCompany.UninCode;
            if (ViewState["Id"] != null)
            {
                #region 修改
                parameter.Add("id", ViewState["Id"].ToString());
                if ((bool)baseDataManage.CallMethod("Tb_PosInfo", "Update", null, new object[] { parameter }))
                {
                    msg = "更新成功";
                    #region 操作日志
                    logoper.OperateType = "修改Pos机";
                    logoper.OptContent = "id:" + ViewState["Id"].ToString() + ",Pos编号:" + txtTerminalNo.Text.Trim() + ",Pos类型:" + ddltype.SelectedValue + ",Pos费率:" + txtfeil.Text;
                    new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);
                    #endregion
                }
                else
                {
                    msg = "更新失败";
                }
                #endregion

            }
            else
            {
                #region 添加
                List<Tb_PosInfo> listpos = baseDataManage.CallMethod("Tb_PosInfo", "GetList", null, new Object[] { "PosNo='" + txtTerminalNo.Text + "'" }) as List<Tb_PosInfo>;
                if (listpos.Count > 0)
                {
                    msg = "此pos机已存在";
                }
                else
                {
                    if ((bool)baseDataManage.CallMethod("Tb_PosInfo", "Insert", null, new Object[] { parameter }))
                    {
                        msg = "添加成功";
                        #region 操作日志
                        logoper.OperateType = "添加Pos机";
                        logoper.OptContent = "Pos编号:" + txtTerminalNo.Text.Trim() + ",Pos类型:" + ddltype.SelectedValue + ",Pos费率:" + txtfeil.Text;
                        new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);
                        #endregion
                    }
                    else
                    {
                        msg = "添加失败";
                    }

                }
                #endregion
            }
        }
        catch (Exception ex)
        {
            msg = "操作异常,商家和pos机类型必选";
            Log_Error logerror = new Log_Error();
            logerror.CpyNo = mCompany.UninCode;
            logerror.DevName = "xw";
            logerror.ErrorContent = ex.Message;
            logerror.LoginName = mUser.LoginName;
            logerror.Page = "PosEdit";
            new PbProject.Logic.Log.Log_ErrorBLL().InsertLog_Error(logerror);
            Bind();
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
        Bind();
    }
}