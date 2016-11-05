using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using PbProject.Model;

public partial class Ajax_ECSearch : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strBack = string.Empty;
        string sql = string.Empty;

        try
        {
            if (Request["type"] != null && Request["cpyno"] != null)    //未有参数传过来，就直接返回提示信息
            {
                StringBuilder sb = new StringBuilder();
                strBack = "<table style=\"width:100%;\"><tbody>";
                if (Request["type"] == "1")
                {
                  //  string no = this.mCompany.UninCode;
                    sb.Append(" CpyNo = '" + Request["cpyno"] + "'");
                    if (Request["val"] != null && Request["val"] != "")
                    {
                        sb.AppendFormat(" and UserName like '%{0}%'", Context.Server.UrlDecode(Request["val"]));
                    }

                    //获取员工信息
                    IList<User_Employees> emplist = baseDataManage.CallMethod("User_Employees", "GetList", null, new Object[] { sb + " order by UserName" }) as List<User_Employees>;

                    if (emplist !=null && emplist.Count > 0)
                    {
                        for (int i = 0; i < emplist.Count; i++)
                        {
                            strBack += "<tr style=\"height:20px;\" onmouseout=\"this.bgColor='#ffffff';\" onmouseover=\"this.bgColor='#F5F5F5';\">";
                            strBack += "<td onclick=\"fnRowClick(this);\" index=\"" + emplist[i].id + "\">" + emplist[i].UserName + "</td></tr>";
                        }
                    }
                    else
                    {
                        strBack += "<tr style=\"height:20px;\" onmouseout=\"this.bgColor='#ffffff';\" onmouseover=\"this.bgColor='#F5F5F5';\">";
                        strBack += "<td>没有查询到用户</td></tr>";
                    }
                }
                else if (Request["type"] == "2")
                {
                    sb.Append(" UninCode like '" + Request["cpyno"] + "%' and Len(UninCode)=18");
                    if (Request["val"] != null && Request["val"] != "")
                    {
                        sb.AppendFormat(" and UninAllName like '%{0}%'", Context.Server.UrlDecode(Request["val"]));
                    }
                    IList<User_Company> cpylist = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { sb + " order by UninAllName" }) as List<User_Company>;
                    if (cpylist != null && cpylist.Count > 0)
                    {
                        for (int i = 0; i < cpylist.Count; i++)
                        {
                            strBack += "<tr style=\"height:20px;\" onmouseout=\"this.bgColor='#ffffff';\" onmouseover=\"this.bgColor='#F5F5F5';\">";
                            strBack += "<td onclick=\"fnRowClick(this);\" >" + cpylist[i].UninAllName + "</td></tr>";
                        }
                    }
                    else
                    {
                        strBack += "<tr style=\"height:20px;\" onmouseout=\"this.bgColor='#ffffff';\" onmouseover=\"this.bgColor='#F5F5F5';\">";
                        strBack += "<td>没有查询到用户</td></tr>";
                    }
                }

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
        Response.Clear();
        Response.Write(strBack);
        Response.End();
    }
}