using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.User;
using PbProject.Dal.ControlBase;
using System.Data;
public partial class Base_WebBlack : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindSupList();
        }
    }
    public void BindSupList()
    {
        //获取落地供应商
        StringBuilder sbSelect = new StringBuilder();
        sbSelect.Append("<select id=\"UserList\" style=\"width:250px;\" onchange=\"SetSel(this)\">\r\n");
        if (Hid_sup.Value == "")
        {
            sbSelect.Append("<option value=\"\" selected=\"selected\">---请选择运营商---</option>\r\n");
        }
        string key = "", Value = "", strHeiPingCanShu = "", strDaPeiZhiCanShu = "", CpyNo = "", UninAllName = "";
        System.Data.DataTable table = this.baseDataManage.GetCompanyConfigInfo();
        int i = 0;
        foreach (DataRow dr_item in table.Rows)
        {
            key = (dr_item["SetName"] != DBNull.Value ? dr_item["SetName"].ToString() : "");
            if (key == "heiPingCanShu")
            {
                strHeiPingCanShu = (dr_item["SetValue"] != DBNull.Value ? dr_item["SetValue"].ToString() : "");
                i++;
            }
            if (key == "daPeiZhiCanShu")
            {
                strDaPeiZhiCanShu = (dr_item["SetValue"] != DBNull.Value ? dr_item["SetValue"].ToString() : "");
                i++;
            }
            if (i == 2)
            {
                CpyNo = (dr_item["UninCode"] != DBNull.Value ? dr_item["UninCode"].ToString() : "");
                UninAllName = (dr_item["UninAllName"] != DBNull.Value ? dr_item["UninAllName"].ToString() : "");
                ConfigParam CP = GetConfigParam(strHeiPingCanShu, strDaPeiZhiCanShu);
                Value = CpyNo + "#" + CP.Office + "$@@@@$" + strHeiPingCanShu + "@@@@" + strDaPeiZhiCanShu;
                if (Hid_sup.Value != "" && Hid_sup.Value == Value)
                {
                    sbSelect.AppendFormat("<option value=\"{0}\" selected=\"true\">{1}</option>\r\n", Value, UninAllName);
                }
                else
                {
                    sbSelect.AppendFormat("<option value=\"{0}\">{1}</option>\r\n", Value, UninAllName);
                }
                i = 0;
            }
        }
        sbSelect.Append("</select>");
        UserSeelct.Text = sbSelect.ToString();
    }
}