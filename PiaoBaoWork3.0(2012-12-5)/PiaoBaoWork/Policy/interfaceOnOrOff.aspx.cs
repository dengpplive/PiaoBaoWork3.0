using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

public partial class Policy_interfaceOnOrOff : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string sql1 = " 1=1 and CpyNo='" + mCompany.UninCode.Substring(0, 12) + "' and SetName='" + PbProject.Model.definitionParam.paramsName.yunYingQuanXian + "'";
                List<Bd_Base_Parameters> listcpy = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { sql1 }) as List<Bd_Base_Parameters>;
                ViewState["id"] = "";
                if (listcpy != null && listcpy.Count > 0)
                {
                    ImportanterGongying.ImportantMarkStr = listcpy[0].SetValue;
                    ViewState["id"] = listcpy[0].id;
                }
            }
            catch (Exception ex)
            {
                ViewState["id"] = ""; 
            }
          
        }

    }
    protected void lbtnOk_Click(object sender, EventArgs e)
    {
        string msg = "更新失败";
        if (ViewState["id"].ToString().Trim() != "")
        {
            HashObject parametercpy = new HashObject();
            parametercpy.Add("id", ViewState["id"].ToString());
            parametercpy.Add("SetValue", ImportanterGongying.ImportantMarkStr);
            bool rs = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Update", null, new object[] { parametercpy });
            if (rs)
            {
                msg = "接口开关更新成功";
            }
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialogmsg('" + msg + "');", true);
    }
}