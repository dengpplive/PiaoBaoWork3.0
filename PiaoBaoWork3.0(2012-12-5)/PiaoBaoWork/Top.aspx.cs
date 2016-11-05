using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PbProject.Model;
using PbProject.WebCommon.Utility;
using DataBase.Data;

/// <summary>
/// Top
/// </summary>
public partial class Top : BasePage
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
                logimg.InnerHtml = "<img src='" + GetimgUrl("logto.png") + "'/>";
                //string lastTime = "";
                //string lastIP = "";
                //User_LoginLog uLoginLog = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetByLoginAccount(mUser.LoginName);
                //if (uLoginLog != null)
                //{
                //    lastTime = uLoginLog.LoginTime.ToString();
                //    lastIP = uLoginLog.LoginIp;
                //}
                //lblShow.Text = "欢迎您：" + mUser.UserName + "，您上次登录的时间为：" + lastTime + ",IP:" + lastIP;

                if (mCompany.RoleType == 1)
                {
                    lblIBEsel.Visible = true;
                    lblClearHF.Visible = true;
                    lblPTword.Visible = true;

                    lblOldVersion.Visible = false;
                    lilbntIsShow.Visible = false;
                }

                if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                {
                    lblShow.Text = string.Format("欢迎您：{0},当前账户余额:<span id='AccountMoney'></span><a href='javascript:void(0)' id='AccountMoneyRef' title='点击更新余额'>(刷新)</a>", mUser.UserName);
                }
                else
                {
                    lblShow.Text = "欢迎您：" + mUser.UserName;
                }
                if (mCompany.UninCode!="100001000049000932")
                {
                    lblCompayName.Text = mCompany.UninAllName.ToString();
                }
                lbtnKeGuiS.Visible = true;
                lbntIsShow.Text = mUser.UserPower.Contains("|2|") ? "显" : "隐";

                GetContact();
                // menuData();
                GetPagePermissions();
            }
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void GetPagePermissions()
    {
        try
        {
            hid_RoleType.Value = mCompany.RoleType.ToString();

            User_Permissions mPost = null;
            if (m_UserPermissions != null)
            {
                //Session中获取登录用户页面权限
                mPost = m_UserPermissions;
            }
            else
            {
                //数据库中读取用户权限
                mPost = new PbProject.Logic.User.User_PermissionsBLL().GetById(mUser.DeptId);
            }

            if (mPost != null && !string.IsNullOrEmpty(mPost.Permissions))
            {

                string strValue = "," + mPost.Permissions.Replace("，", ",") + ",";
                string strIndex = ",";

                //缓存获取菜单页面
                List<Bd_Base_Page> iPostList = new PbProject.Logic.ControlBase.Bd_Base_PageBLL().GetListByCache(mCompany.RoleType);

                foreach (Bd_Base_Page item in iPostList)
                {
                    if (strValue.Contains("," + item.PageIndex + ",") && !strIndex.Contains("," + item.ModuleIndex + ","))
                        strIndex += item.ModuleIndex + ",";
                }

                hid_ShowModuleIndex.Value = strIndex;
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void GetContact()
    {
        try
        {
            string cpyNo = this.mCompany.UninCode;
            string queryWhere = string.Format(" id=(select SetValue from Bd_Base_Parameters where CpyNo='{0}' and SetName='suoShuYeWuYuan')", cpyNo);
            List<User_Employees> list = baseDataManage.CallMethod("User_Employees", "GetList", null, new object[] { queryWhere }) as List<User_Employees>;
            if (list != null && list.Count > 0)
            {
                var model = list[0];
                if (model != null)
                    lbl_contact.Text = string.Format("您的客户经理是:{0} 联系电话:{1} QQ:{2}", model.UserName, model.Tel, model.QQ);
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 退出
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnOut_Click(object sender, EventArgs e)
    {

        try
        {
            //PiaoBao.BLLLogic.Black.ControlProcess cp = new PiaoBao.BLLLogic.Black.ControlProcess(mManageConfig, mSupperConfig);
            //cp.OutBlack(mUser.BlankUser, mUser.BlankPassword);
            //new PiaoBao.BLLLogic.Black.Log_BlackCode().ReadTXT(mUser.LoginName)
            string currentuserid = Request["currentuserid"] ?? string.Empty;
            if (string.IsNullOrEmpty(currentuserid))
            {
                Session[currentuserid] = null;
                Session[currentuserid] = null;
                Session.Clear();
                Session.Abandon();
                Application[currentuserid] = null;
                Application[currentuserid + "oneUserLoginCookies"] = null;
                System.Web.Security.FormsAuthentication.RedirectToLoginPage();
            }
            else
            {
                Response.Redirect("~/Login.aspx");
            }
        }
        catch
        {

        }
        finally
        {
            //Response.Write("<script language='javascript' type='text/javascript'>top.location.href='Login.aspx';</script>");
        }
    }

    /// <summary>
    /// 返回公告条数
    /// </summary>
    /// <returns></returns>
    public int getGongaoCount()
    {
        int count = 0;
        try
        {

        }
        catch (Exception)
        {

        }
        return count;
    }

    /// <summary>
    /// 返回旧版本
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbntOld_Click(object sender, EventArgs e)
    {
        string url = "";
        try
        {
            string uninCode = mCompany.UninCode;

            if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
            {
                #region 供应

                switch (uninCode)
                {
                    case "100001000034"://重庆
                        url = "http://210.14.138.26:178/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;

                    case "100001000035": //山西
                        url = "http://210.14.138.26:192/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000038": //海口
                        url = "http://124.172.237.134:92/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000041"://昆明华航---------------------
                        url = "http://116.52.250.78:94/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000045": //深圳
                        url = "http://210.14.138.26:302/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000049": //成都快捷通
                        url = "http://210.14.138.26:252/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000050"://贵阳启明航空服务有限公司
                        url = "http://210.14.138.26:224/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000051"://兰州
                        url = "http://210.14.138.26:222/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000052"://新疆
                        url = "http://210.14.138.26:122/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    default:
                        url = "";// 没有的
                        break;
                }

                #endregion
            }
            else if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
            {
                #region 分销


                uninCode = mCompany.UninCode.Substring(0, 12);

                switch (uninCode)
                {
                    case "100001000034"://重庆
                        url = "http://210.14.138.26:177/Login.aspx?uname="
                             + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000035"://山西
                        url = "http://210.14.138.26:191/Login.aspx?uname="
                             + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000038": //海口
                        url = "http://124.172.237.134:91/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000041"://昆明华航
                        url = "http://116.52.250.78:91/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000045": //深圳
                        url = "http://210.14.138.26:301/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000049": //成都快捷通
                        url = "http://210.14.138.26:251/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000050"://贵阳启明航空服务有限公司
                        url = "http://210.14.138.26:223/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000051": //兰州
                        url = "http://210.14.138.26:221/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    case "100001000052"://新疆
                        url = "http://210.14.138.26:121/Login.aspx?uname="
                            + mUser.LoginName + "&upwd=" + mUser.LoginPassWord.ToUpper() + "&ctdyppbe=" + DateTime.Now.Ticks.ToString(); break;
                    default:
                        url = "";// 没有的
                        break;
                }
                #endregion
            }

        }
        catch (Exception)
        {

        }
        ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "window.open('" + url + "', '_blank');", true);
    }

    /// <summary>
    /// 隐藏 和 显示 返点
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbntIsShow_Click(object sender, EventArgs e)
    {
        try
        {
            int tmpiShow = (lbntIsShow.Text == "隐") ? 1 : 0;
            lbntIsShow.Text = (lbntIsShow.Text == "隐") ? "显" : "隐";

            if (tmpiShow == 0)
            {
                if (mUser.UserPower == "|2|")
                {
                    mUser.UserPower = "";
                }
                else
                {
                    mUser.UserPower = mUser.UserPower.Replace("|2|", "|");
                }
            }
            else
            {
                if (!mUser.UserPower.Contains("|2|"))
                {
                    if (mUser.UserPower.Trim() == "")
                    {
                        mUser.UserPower = mUser.UserPower + "|2|";
                    }
                    else
                    {
                        mUser.UserPower = mUser.UserPower + "2|";
                    }
                }
            }

            HashObject parameter = new HashObject();
            parameter.Add("id", mUser.id);
            parameter.Add("UserPower", mUser.UserPower);
            bool retulst = new PbProject.Logic.User.User_EmployeesBLL().UpdateById(parameter);
        }
        catch (Exception ex)
        {
            //OnErrorNew(0, ex.ToString(), "lbntIsShow_Click");
        }
    }

    public string DefaultUrl
    {
        get
        {
            string url = (Request["ourl"] != null && Request["ourl"].ToString() != "") ? "1" : " index.aspx?currentuserid=" + mUser.id.ToString();
            return url;
        }
    }
}