using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PbProject.Model;

/// <summary>
/// 自动登录使用
/// </summary>
public partial class AutoLogin : BasePage
//public partial class AutoLogin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 自动登录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_Click(object sender, EventArgs e)
    {
        try
        {
            if (mCompany.RoleType == 1)
            {
                string url = "";

                //使用自动登录
                string CompanyName = txtLogin.Text.Trim();
                string CompanyPwd = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(CompanyName);

                if (rblType.SelectedValue == "91")
                {
                    #region 正式站
                    string tempWhere = " UninCode in(select CpyNo from User_Employees where LoginName='" + CompanyName + "') ";
                    List<User_Company> payCompanyList = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere(tempWhere);

                    if (payCompanyList != null && payCompanyList.Count > 0)
                    {
                        if (string.IsNullOrEmpty(payCompanyList[0].WebSite))
                        {
                            string uninCode = payCompanyList[0].UninCode;

                            if (uninCode.Length >= 12)
                            {
                                tempWhere = " UninCode ='" + uninCode.Substring(0, 12) + "' ";
                                List<User_Company> payCompanyList2 = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere(tempWhere);

                                if (payCompanyList2 != null && payCompanyList2.Count > 0)
                                    url = payCompanyList2[0].WebSite;
                            }
                        }
                        else
                        {
                            url = payCompanyList[0].WebSite;
                        }
                    }
                    else
                    {
                        // 没有用户
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        string[] str = url.Split('|');
                        url = str[0];
                        url = url.Trim();

                        if (!url.Contains("http://"))
                            url = "http://" + url;
                    }

                    #endregion
                }
                else
                {
                    //测试站
                    url = "http://210.14.138.26:204";
                }

                //自动登录地址
                url = string.IsNullOrEmpty(url) ? "http://210.14.138.26:91" : url;

                //url = "http://210.14.138.26:91";
                url += "/Login.aspx?cudspeb=" + CompanyName + "&cpdwpdb=" + CompanyPwd + "&ctdyppbe=cydepsb&ctdipmbe=" + DateTime.Now.Ticks.ToString();
                //Response.Write(url);
                txtUrl.Text = url;
                ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "AutoLogin('" + url + "')", true);
            }
        }
        catch (Exception)
        {

        }
    }
}