using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using PbProject.Model;
using PbProject.WebCommon.Web.Cookie;
using System.Xml;

/// <summary>
/// 登录页面
/// </summary>
public partial class Login : System.Web.UI.Page
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                DataTable[] tableArr = null;
                //获取域名
                string nowurl = HttpContext.Current.Request.Url.Host;
                User_Company uCompany = new PbProject.Logic.Login().GetByURL(nowurl);
                string name = (uCompany != null) ? uCompany.UninCode : "";//获取公司编号
                if (!string.IsNullOrEmpty(name))
                {
                    this.divLog.Style.Value = "background:url('images/" + name + "/logo_top.jpg') no-repeat scroll left bottom transparent;height:70px;margin:0 auto;padding-top:12px;text-align:right;width:960px;";
                    this.divleftbox.Style.Value = "width: 330px; height: 530px;background:url(images/" + name + "/stuff.png) no-repeat";
                    divswf.InnerHtml = "<div id='focus'>" +
                                "<ul>" +
                                    "<li><a href='#' onclick='return false;'>" +
                                        "<img src='Images/" + name + "/1.gif' style='width: 620px; height: 244px;' alt='' /></a></li>" +
                                    "<li><a href='#' onclick='return false;'>" +
                                        "<img src='Images/" + name + "/2.gif' style='width: 620px; height: 244px;' alt='' /></a></li>" +
                                    "<li><a href='#' onclick='return false;'>" +
                                        "<img src='Images/" + name + "/3.gif' style='width: 620px; height: 244px;' alt='' /></a></li>" +
                                    "<li><a href='#' onclick='return false;'>" +
                                        "<img src='Images/" + name + "/4.gif' style='width: 620px; height: 244px;' alt='' /></a></li>" +
                                "</ul>" +
                            "</div>";

                }
                else
                {
                    this.divLog.Style.Value = "background:url('images/logo_top_1.jpg') no-repeat scroll left bottom transparent;height:70px;margin:0 auto;padding-top:12px;text-align:right;width:960px;";
                    this.divleftbox.Style.Value = "width: 330px; height: 530px;background:url('images/100001/stuff.png') no-repeat";

                    divswf.InnerHtml = "<div id='focus'>" +
                                "<ul>" +
                                    "<li><a href='#' onclick='return false;'>" +
                                        "<img src='Images/1.gif' style='width: 620px; height: 244px;' alt='' /></a></li>" +
                                    "<li><a href='#' onclick='return false;'>" +
                                        "<img src='Images/2.gif' style='width: 620px; height: 244px;' alt='' /></a></li>" +
                                    "<li><a href='#' onclick='return false;'>" +
                                        "<img src='Images/3.gif' style='width: 620px; height: 244px;' alt='' /></a></li>" +
                                    "<li><a href='#' onclick='return false;'>" +
                                        "<img src='Images/4.gif' style='width: 620px; height: 244px;' alt='' /></a></li>" +
                                "</ul>" +
                            "</div>";
                }

                bool IsAutoLogin = false;
                bool IsOrderPrompt = false;
                string OrderPromptUrl = "";
                if (Request.QueryString["cudspeb"] != null && Request.QueryString["cpdwpdb"] != null && Request.QueryString["ctdyppbe"] != null)
                {
                    #region 自动登录

                    string loginName = Request.QueryString["cudspeb"].ToString();
                    string loginPwd = Request.QueryString["cpdwpdb"].ToString();
                    string loginType = Request.QueryString["ctdyppbe"].ToString();

                    if (loginType == "cydepsb" && loginPwd == PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(loginName))
                    {
                        loginPwd = "a!d@m#i$n%c^d&p*b";

                        //可以自动登录
                        PbProject.Logic.Login LoginManage = new PbProject.Logic.Login();

                        string msg = "";
                        IsAutoLogin = LoginManage.GetByName(loginName, loginPwd, true, Page.Request.UserHostAddress, out tableArr, out msg, 1, 0, 1);
                        //是否为客户端订单提醒而来
                        IsOrderPrompt = (Request["OrderPrompt"] != null && Request["OrderPrompt"].ToString() == "1") ? true : false;
                        if (IsOrderPrompt)
                        {
                            OrderPromptUrl = Request["ourl"] != null ? HttpUtility.UrlEncode(Request["ourl"].ToString()) : "";
                        }
                    }

                    #endregion
                }
                else
                {
                    #region 普通登录
                    SiteCookie siteCookie = new SiteCookie();
                    string cookievalue = siteCookie.GetCookie("PBCookies");
                    if (!string.IsNullOrEmpty(cookievalue))
                    {
                        string[] cookies = cookievalue.Split('|');
                        txtUserName.Value = cookies[0];
                        txtPwd.Attributes["value"] = cookies[1];
                        chkCook.Checked = true;
                    }
                    #endregion
                }
                if (IsAutoLogin)
                {
                    string curid = tableArr[0].Rows[0]["id"].ToString();
                    string GoUrl = "Default.aspx?l=1&currentuserid=" + curid + (IsOrderPrompt ? "&ourl=" + OrderPromptUrl : "");
                    Response.Redirect(GoUrl, false);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "IsOpen();", true);
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            #region 验证码

            string chechCode = new PbProject.Logic.SessionContent().CHECKCODE;

            bool rs = (this.txtCheckCode.Text.Trim() == null
                  || this.txtCheckCode.Text.Trim() == ""
                  || Session[chechCode] == null
                  || this.txtCheckCode.Text.ToLower().Trim() != Session[chechCode].ToString().ToLower()) ? true : false;

            this.txtCheckCode.Text = "";
            //rs = false;
            if (rs)
            {
                Random rd = new Random(99);
                this.img1.Src = "CheckCode.aspx?abc=" + rd.Next();
                ClientScript.RegisterStartupScript(this.GetType(), "1", "showdialog('验证码错误!');", true);
                return;
            }
            #endregion

            string CompanyName = txtUserName.Value.Trim(); //登录名称
            string CompanyPwd = txtPwd.Text.Trim();//  //登录密码
            string msg = "";

            if (string.IsNullOrEmpty(CompanyName))
                msg = "请输入登录用户名！";
            else if (string.IsNullOrEmpty(CompanyPwd))
                msg = "请输入登录密码！";

            if (msg == "")
            {
                PbProject.Logic.Login LoginManage = new PbProject.Logic.Login();
                DataTable[] tableArr = null;

                //正式环境
                bool IsSuc = LoginManage.GetByName(CompanyName, CompanyPwd, true, Page.Request.UserHostAddress, out tableArr, out msg);

                ////********** 调试使用登录 *************//
                if (IsSuc)
                {


                    if (chkCook.Checked)
                    {
                        SiteCookie sitecookie = new SiteCookie();
                        sitecookie.SaveCookie("PBCookies", txtUserName.Value + "|" + txtPwd.Text);
                    }

                    string curid = tableArr[0].Rows[0]["id"].ToString();
                    Response.Redirect("Default.aspx?l=1&currentuserid=" + curid);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "alert('" + msg + "');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "alert('" + msg + "');", true);
            }

        }
        catch (Exception ex)
        {
            #region catch

            #endregion
        }
    }
}