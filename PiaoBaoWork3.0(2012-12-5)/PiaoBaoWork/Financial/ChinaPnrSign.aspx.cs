using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// 汇付签约
/// </summary>
public partial class Financial_ChinaPnrSign : System.Web.UI.Page
{
    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        string val = "";
        try
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["user"] != null && Request.QueryString["oper"] != null)
                {
                    PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
                    val = chinaPnr.Sign(Request.QueryString["user"].ToString().Trim(), Request.QueryString["oper"].ToString().Trim());
                }
            }
        }
        catch (Exception ex)
        {
          
        }
        val = !string.IsNullOrEmpty(val) ? val : "请求失败";
        Response.Write(val);
    }
}