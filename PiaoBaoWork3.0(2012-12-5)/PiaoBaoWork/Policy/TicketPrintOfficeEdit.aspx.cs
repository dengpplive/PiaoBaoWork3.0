using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

public partial class Manager_Base_TicketPrintOfficeEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnBack.PostBackUrl = string.Format("TicketPrintOfficeList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            if (Request.QueryString["Id"] != null)
            {
                ViewState["Id"] = Request.QueryString["Id"];
                GetInfoById();
            }
            lbsave.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    /// <summary>
    /// 获取要修改的舱位
    /// </summary>
    protected void GetInfoById()
    {
        Tb_Ticket_PrintOffice mprintoffice = baseDataManage.CallMethod("Tb_Ticket_PrintOffice", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Tb_Ticket_PrintOffice;
        if (mprintoffice != null)
        {
            txtAirPortCode.Value = mprintoffice.AirCode.ToString();
            txtOffice.Text = mprintoffice.OfficeCode.ToString();

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
        IHashObject parameter = new HashObject();
        try
        {
            parameter.Add("OperTime", DateTime.Now);
            parameter.Add("OperLoginName", mUser.LoginName);
            parameter.Add("OperUserName", mUser.UserName);
            parameter.Add("OfficeCode", txtOffice.Text.Trim().ToString());
            parameter.Add("PrintCode", txtPrintCode.Text.Trim().ToString());
            
            if (ViewState["Id"]!=null)
            {
                parameter.Add("id", ViewState["Id"].ToString());
                if ((bool)baseDataManage.CallMethod("Tb_Ticket_PrintOffice", "Update", null, new object[] { parameter }) == true)
                {
                    msg = "更新成功";
                }
                else
                {
                    msg = "更新失败";
                }
            }
            else
            {
                parameter.Add("CpyNo", mCompany.UninCode);
                parameter.Add("CpyName", mCompany.UninAllName);
                parameter.Add("CpyType", mCompany.RoleType);
                parameter.Add("AirCode", txtAirPortCode.Value);
                List<Tb_Ticket_PrintOffice> listprintoffice = baseDataManage.CallMethod("Tb_Ticket_PrintOffice", "GetList", null, new Object[] { "AirCode='" + txtAirPortCode.Text + "' and CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Ticket_PrintOffice>;
                if (listprintoffice!=null && listprintoffice.Count>0)
                {
                    msg = "此航空公司已设置";
                }
                else
                {
                    msg = (bool)baseDataManage.CallMethod("Tb_Ticket_PrintOffice", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
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