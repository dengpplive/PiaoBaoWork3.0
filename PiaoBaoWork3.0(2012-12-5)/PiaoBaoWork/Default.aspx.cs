using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections;
using System.Web.Security;

public partial class Default : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["l"] == null)
        {
            Response.Redirect("~/SignOut.aspx");
        }
    }

    public string DefaultUrl
    {
        get
        {
            string url = (Request["ourl"] != null && Request["ourl"].ToString() != "") ? Request["ourl"].ToString() : ("index.aspx?currentuserid=" + sessionuserid);
            return url;
        }
    }

    public string TopUrl
    {
        get
        {
            string url = (Request["ourl"] != null && Request["ourl"].ToString() != "") ? ("Top.aspx?currentuserid=" + sessionuserid + "&ourl=" + HttpUtility.UrlEncode(Request["ourl"].ToString())) : ("Top.aspx?currentuserid=" + sessionuserid);
            return url;
        }
    }

    public string sessionuserid
    {
        get
        {
            if (mUser != null)
            {
                return mUser.id.ToString();
            }
            return "";
        }
    }

    public string Title
    {
        get
        {
            if (mCompany != null)
            {
                return mCompany.UninAllName;
            }
            else
            {
                return "";
            }
        }
    }
}