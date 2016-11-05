using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Financial_hf : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
        string result = getParameter("txt");
        if (!string.IsNullOrEmpty(result))
        {
            string val = chinaPnr.Sign(result, result);
            Response.Write(val);
        }
        else {
            Response.Write("参数不能为空!");
        }
    }
    private string getParameter(string key)
    {
        string returns=string.Empty;
        var result = Request[key];
        if (result != null)
            returns = result.ToString();
        return returns;
    }
}