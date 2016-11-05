using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

public partial class Policy_StrategyGroupEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnBack.PostBackUrl = string.Format("StrategyGroupList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            if (Request["id"]!=null)
            {
                ViewState["id"]=Request["id"];
                GetStrategyGroupinfo();
            }
           
            lbsave.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    protected void GetStrategyGroupinfo()
    {
        Tb_Ticket_StrategyGroup group = baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetById", null, new object[] { ViewState["id"].ToString() }) as Tb_Ticket_StrategyGroup;
        this.txtGroupName.Text = group.GroupName;
        this.txtUnitePoint.Text = group.UnitePoint.ToString();
        this.rblDefaultFlag.SelectedValue = group.DefaultFlag.ToString();
        this.rblUniteFlag.SelectedValue = group.UniteFlag.ToString();
        this.txtGroupName.Enabled = false;
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
        try
        {
           
            parameter.Add("OperTime",DateTime.Now);
            parameter.Add("OperLoginName",mUser.LoginName);
            parameter.Add("OperUserName",mUser.UserName);
            parameter.Add("GroupName",txtGroupName.Text.Trim());
            parameter.Add("DefaultFlag",rblDefaultFlag.SelectedValue);
            parameter.Add("UniteFlag",rblUniteFlag.SelectedValue);
            parameter.Add("UnitePoint",txtUnitePoint.Text.Trim());
            if (rblDefaultFlag.SelectedValue == "true")
            {
                new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo("Update Tb_Ticket_StrategyGroup set DefaultFlag='false'");
            }
            if (ViewState["id"]!=null)
            {
                #region 修改
                parameter.Add("id",ViewState["id"]);
                if ((bool)baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "Update", null, new object[] { parameter }) == true)
                {
                    msg = "更新成功";
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
                parameter.Add("CpyName", mCompany.UninAllName);
                parameter.Add("CpyType", mCompany.RoleType);
                List<Tb_Ticket_StrategyGroup> listgroup = baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetList", null, new Object[] { "GroupName='" + txtGroupName.Text.Trim() + "' and CpyNo='"+mCompany.UninCode+"'" }) as List<Tb_Ticket_StrategyGroup>;
                if (listgroup!=null && listgroup.Count>0)
                {
                    msg = "该用户已存在此扣点组";
                }
                else
                {
                    msg = (bool)baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
                }
                
                #endregion
            }

        }
        catch (Exception)
        {
            
            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}