using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

public partial class Policy_PolicyProtectEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnBack.PostBackUrl = string.Format("PolicyProtectList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            if (Request["id"] != null & Request["id"] != "")
            {
                ViewState["id"] = Request["id"];
                Getinfo();
            }
            lbsave.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    protected void Getinfo()
    {
        Tb_Policy_Protect mpolicyprotect = baseDataManage.CallMethod("Tb_Policy_Protect", "GetById", null, new object[] { ViewState["id"].ToString() }) as Tb_Policy_Protect;
        txtAirCode.Text = mpolicyprotect.CarryCode;
        txtFromCityCode.Text = mpolicyprotect.FromCityCode;
        txtToCityCode.Text = mpolicyprotect.ToCityCode;
    }
    protected void lbsave_Click(object sender, EventArgs e)
    {
        string msg = "";
        IHashObject parameter = new HashObject();
        try
        {
            parameter.Add("OperTime", DateTime.Now);
            parameter.Add("OperLoginName", mUser.LoginName);
            parameter.Add("OperUserName", mUser.UserName);
            parameter.Add("CarryCode", this.txtAirCode.Text.ToUpper().Trim().ToString());
            parameter.Add("FromCityCode", txtFromCityCode.Text.ToUpper().Trim().ToString());
            parameter.Add("ToCityCode", txtToCityCode.Text.ToUpper().Trim().ToString());
            parameter.Add("State", Convert.ToInt32(rblstate.SelectedValue));
            if (ViewState["id"] != null)
            {
                parameter.Add("id", ViewState["id"].ToString());
                if ((bool)baseDataManage.CallMethod("Tb_Policy_Protect", "Update", null, new object[] { parameter }) == true)
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
             
                List<Tb_Policy_Protect> listprotect = baseDataManage.CallMethod("Tb_Policy_Protect", "GetList", null, new Object[] { "CarryCode='" + txtAirCode.Text.Trim() + "' and FromCityCode='" + txtFromCityCode.Text.Trim() + "' and ToCityCode='" + txtToCityCode.Text.Trim() + "' and CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Policy_Protect>;
                if (listprotect != null && listprotect.Count > 0)
                {
                    msg = "此航空公司航段已设置";
                }
                else
                {
                    msg = (bool)baseDataManage.CallMethod("Tb_Policy_Protect", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
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