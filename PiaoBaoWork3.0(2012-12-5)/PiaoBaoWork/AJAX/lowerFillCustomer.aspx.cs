using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class AJAX_lowerFillCustomer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sret = string.Empty;
        string sql = string.Empty;

        try
        {
            if (Request["cpyno"] != null)
            {


                DataTable dt = new DataTable();
                try
                {
                    dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetViewInfoByStrWhere("V_LowerFillInfo", "UninCode='" + Context.Server.UrlDecode(Request["cpyno"]) + "'");
                }
                catch (Exception ex)
                {
                    //ds = Maticsoft.DBUtility._51cbc.Query(conn, sql);
                    sret = "无信息";
                }

                sret = "<table cellpadding='5' cellspacing='1' border='1' bordercolor='#000000' style='border-collapse:collapse'><tbody>";
                sret += "<tr style='height:30px;'><td style='width:40%;'>客户名称：</td>";
                sret += "<td style='width:60%;'>" + dt.Rows[0]["UninAllName"] + "</td></tr>";
                sret += "<tr style='height:30px;'><td>公司名称：</td>";
                sret += "<td>" + dt.Rows[0]["UninAllName"] + "</td></tr>";
                sret += "<tr style='height:30px;'><td>公司电话：</td>";
                sret += "<td>" + dt.Rows[0]["Tel"].ToString() + "</td></tr></tr>";

                sret += "<tr style='height:30px;'><td>联系人：</td>";
                sret += "<td>" + dt.Rows[0]["ContactUser"].ToString() + "</td></tr>";
                sret += "<tr style='height:30px;'><td>联系人电话：</td>";
                sret += "<td>" + dt.Rows[0]["ContactTel"].ToString() + "</td></tr>";


                sret += "<tr style='height:30px;'><td>客户账号：</td>";
                sret += "<td>" + dt.Rows[0]["LoginName"].ToString() + "</td></tr>";
                sret += "<tr style='height:30px;'><td>所属业务员：</td>";
                sret += "<td>" + dt.Rows[0]["ywy"].ToString() + "</td></tr>";
                sret += "<tr style='height:30px;'><td>POS机号：</td>";
                sret += "<td>" + dt.Rows[0]["posno"] + "</td></tr>";
                sret += "<tr style='height:30px;'><td>费率：</td>";
                sret += "<td>" + dt.Rows[0]["posrate"] + "</td></tr>";
                sret += "</tbody></table>";
            }
            else
            {
                sret = "无信息";
            }
        }
        catch (Exception ex)
        {
            sret = ex.Message;
        }

        //回传数据
        Response.Write(sret);
        Response.End();
    }
}