using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;

public partial class DiscountSet_SalesDiscountSet : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetOneInfo();
        }
    }
    protected void GetOneInfo()
    {
        try
        {
            List<Tb_Ticket_TakeOffDetail> list = baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "GetList", null, new Object[] { "CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Ticket_TakeOffDetail>;
            if (list != null && list.Count>0)
            {
                txtPoint.Text = list[0].PointScope.Split('^')[1].ToString();
                txtMoney.Text = list[0].PointScope.Split('^')[2].ToString();
                rblSelectType.SelectedValue = list[0].SelectType.ToString();
            }
        }
        catch (Exception)
        {
            
            throw;
        }
    }
    protected void lbsave_Click(object sender, EventArgs e)
    {
        IHashObject parameter = new HashObject();
        string msg = "";
        try
        {
            parameter.Add("OperTime", DateTime.Now);
            parameter.Add("OperLoginName", mUser.LoginName);
            parameter.Add("OperUserName", mUser.UserName);
            parameter.Add("CarryCode","ALL");
            parameter.Add("BaseType", "0");//分销和二级分销的直接为全类型,设置了就本地,接口,共享都起效 2013-02-28 yyy add
            
            parameter.Add("FromCityCode","ALL");
            parameter.Add("ToCityCode","ALL");
            parameter.Add("SelectType", rblSelectType.SelectedValue);
            parameter.Add("TimeScope", "2000-01-01|2100-01-01");//分销和二级分销的扣点不需要时间,设置了就包括全时间 2013-02-28 yyy add
            parameter.Add("PointScope","1-100^"+txtPoint.Text.Trim()+"^"+txtMoney.Text.Trim());
            List<Tb_Ticket_TakeOffDetail> list = baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "GetList", null, new Object[] { "CpyNo='"+mCompany.UninCode+"'" }) as List<Tb_Ticket_TakeOffDetail>;
            if (list!=null && list.Count>0)
            {
                parameter.Add("id", list[0].id);
                msg = (bool)baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "Update", null, new object[] { parameter }) == true ? "设置成功" : "设置失败";
            }
            else
            {
                parameter.Add("CpyNo", mCompany.UninCode);
                parameter.Add("CpyName", mCompany.UninAllName);
                parameter.Add("CpyType", mCompany.RoleType);
                msg = (bool)baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
            }
        }
        catch (Exception)
        {
            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}