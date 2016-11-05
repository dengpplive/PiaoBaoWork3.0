using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Ajax_CustomerInfo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strBack = string.Empty;
        string sql = string.Empty;

        try
        {
            if (Request["cpyname"] != null)
            {


                DataTable dt = new DataTable();
                try
                {
                     dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetViewInfoByStrWhere("V_AccountInfo", "IsAdmin=0 and UninAllName='" + Context.Server.UrlDecode(Request["cpyname"]) + "'");
                }
                catch (Exception ex)
                {
                    //ds = Maticsoft.DBUtility._51cbc.Query(conn, sql);
                }

                strBack = "<table cellpadding='5' cellspacing='1' border='1' bordercolor='#000000' style='border-collapse:collapse'><tbody>";
                strBack += "<tr style='height:30px;'><td style='width:40%;'>客户名称：</td>";
                strBack += "<td style='width:60%;'>" + dt.Rows[0]["UninAllName"].ToString() + "</td></tr>";
                strBack += "<tr style='height:30px;'><td>公司名称：</td>";
                strBack += "<td>" + dt.Rows[0]["UninAllName"].ToString() + "</td></tr>";
                strBack += "<tr style='height:30px;'><td>公司电话：</td>";
                strBack += "<td>" + dt.Rows[0]["Tel"].ToString() + "</td></tr></tr>";

                strBack += "<tr style='height:30px;'><td>联系人：</td>";
                strBack += "<td>" + dt.Rows[0]["ContactUser"].ToString() + "</td></tr>";
                strBack += "<tr style='height:30px;'><td>联系人电话：</td>";
                strBack += "<td>" + dt.Rows[0]["ContactTel"].ToString() + "</td></tr>";

                strBack += "<tr style='height:30px;'><td>客户账号：</td>";
                strBack += "<td>" + dt.Rows[0]["LoginName"].ToString() + "</td></tr>";

                strBack += "</tbody></table>";
            }
            else
            {
                strBack = "无信息";
            }
        }
        catch (Exception ex)
        {
            strBack = ex.Message;
        }

        //回传数据
        Response.Write(strBack);
        Response.End();
    }
}