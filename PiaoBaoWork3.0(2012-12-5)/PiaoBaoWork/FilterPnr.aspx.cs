using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.ControlBase;
using DataBase;
using PbProject.Dal;
using DataBase.Data;

public partial class FilterPnr : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        BaseDataManage maange = new BaseDataManage();
        HashObject queryParamter = new HashObject();
        string strData = string.Join("|", txtPnrs.Value.Split(new string[] { "|"," ", ",", "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));

        queryParamter.Add("SourceSql", strData);
        queryParamter.Add("StrSeprate", "|");
        System.Data.DataTable table = maange.EexcProc("IsPlatformByPnr", queryParamter);
        if (table != null && table.Rows.Count > 0)
        {
            txtResult.Value = table.Rows[0][0] != DBNull.Value ? table.Rows[0][0].ToString() : "";
        }
        else
        {
            txtResult.Value = "未找到结果";
        }
    }
}