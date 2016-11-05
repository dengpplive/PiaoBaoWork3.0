using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using DataBase.LogCommon;
using PbProject.WebCommon.Web.Cookie;
using PbProject.WebCommon.Utility.Script;
using PbProject.WebCommon.Utility;
using System.Text.RegularExpressions;
using System.Text;
using PbProject.Model;
using DataBase.Data;
using PbProject.Logic.ControlBase;
using System.Web.Security;
using PbProject.Logic;
using System.IO;
using PbProject.Logic.PID;
public class BasePage : SignPage, IObject
{
    #region 参数
    /// <summary>
    /// 用户个人信息model
    /// </summary>
    public PbProject.Model.User_Employees mUser;
    /// <summary>
    /// 当前登录用户公司信息model
    /// </summary>
    public PbProject.Model.User_Company mCompany;
    /// <summary>
    /// 供应商和落地运营商公司信息model
    /// </summary>
    public PbProject.Model.User_Company mSupCompany;
    /// <summary>
    /// 当前登录用户公司参数信息
    /// </summary>
    public List<PbProject.Model.Bd_Base_Parameters> baseParametersList;
    /// <summary>
    /// 落地运营商和供应商公司参数信息
    /// </summary>
    public List<PbProject.Model.Bd_Base_Parameters> supBaseParametersList;
    /// <summary>
    /// 配置信息
    /// </summary>
    public PbProject.Model.ConfigParam configparam;

    

    /// <summary>
    /// 公共操作
    /// </summary>
    public PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();


    /// <summary>
    /// 当前登录用户权限
    /// </summary>
    public User_Permissions m_UserPermissions;


    /// <summary>
    /// 页面之间需要传递的对象 可以使用这个
    /// </summary>
    public virtual Object PageObj { get; set; }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        LoadSession();
    }
    /// <summary>
    /// 原先设计用的是Session现在改为Application,但是方法名称未改变,避免过多改动引发系统不稳定.YYY 2013-6-17
    /// </summary>
    public void LoadSession()
    {

        try
        {
            SessionContent sessionContent = new SessionContent();
            string currentuserid = System.Web.HttpContext.Current.Request["currentuserid"] ?? string.Empty;

            if (!string.IsNullOrEmpty(currentuserid))
            {
                //if (Session[currentuserid] == null)
                if (Application[currentuserid] == null)
                //if (HttpContext.Current.Application[currentuserid] == null)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                else
                {
                    //sessionContent = Session[currentuserid] as SessionContent;
                    //sessionContent = HttpContext.Current.Application[currentuserid] as SessionContent;
                    sessionContent = Application[currentuserid] as SessionContent;
                    mUser = sessionContent.USER;
                    mCompany = sessionContent.COMPANY;
                   
                    //mSupCompany = sessionContent.SUPCOMPANY;

                    //如果保存的落地运营商的<公司信息>全局变量为空,则重新读取一次数据库
                    if (Application[sessionContent.parentCpyno + "Company"] == null)
                    {
                        string strwhere = "1=1 and unincode='" + sessionContent.parentCpyno + "'";
                        List<User_Company> listUser_Company = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { strwhere }) as List<User_Company>;
                        if (listUser_Company != null && listUser_Company.Count > 0)
                        {
                            Application[sessionContent.parentCpyno + "Company"] = listUser_Company[0];
                        }

                    }
                    mSupCompany = Application[sessionContent.parentCpyno + "Company"] as User_Company;
                    baseParametersList = sessionContent.BASEPARAMETERS;
                    //如果保存落地运营商的全局变量为空,则重新读取一次数据库
                    if (Application[sessionContent.parentCpyno + "Parameters"] == null)
                    {
                        string strwhere = "1=1 and cpyno='" + sessionContent.parentCpyno + "'";
                        List<Bd_Base_Parameters> listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { strwhere }) as List<Bd_Base_Parameters>;
                        if (listParameters != null)
                        {
                            Application[sessionContent.parentCpyno + "Parameters"] = listParameters;
                        }

                    }
                    supBaseParametersList =  Application[sessionContent.parentCpyno + "Parameters"]as List<Bd_Base_Parameters>;//落地运营商和供应商公司参数信息
                    //supBaseParametersList = sessionContent.SupBASEPARAMETERS;//落地运营商和供应商公司参数信息
                    configparam = Bd_Base_ParametersBLL.GetConfigParam(supBaseParametersList);
                    
                    //configparam = sessionContent.CONFIGPARAM;


                    //当前登录用户权限
                    m_UserPermissions = sessionContent.M_USERPERMISSIONS;
                    // Limits();
                }
            }
            else
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            /*
            if (Session[sessionContent.USERLOGIN] == null)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            else
            {
                sessionContent = Session[sessionContent.USERLOGIN] as SessionContent;
                mUser = sessionContent.USER;
                mCompany = sessionContent.COMPANY;
                mSupCompany = sessionContent.SUPCOMPANY;
                baseParametersList = sessionContent.BASEPARAMETERS;
                supBaseParametersList = sessionContent.SupBASEPARAMETERS;//落地运营商和供应商公司参数信息
                configparam = sessionContent.CONFIGPARAM;
                //当前登录用户权限
                m_UserPermissions = sessionContent.M_USERPERMISSIONS;
                //权限验证
                Limits();
            }*/
            //}
        }
        catch
        {
            //  跳转登陆页
        }
    }

    /// <summary>
    /// OnError
    /// </summary>
    /// <param name="e"></param>
    protected override void OnError(EventArgs e)
    {
        Exception ex = Server.GetLastError();
        Log.Error(ex.Message, ex);
        Server.ClearError();
        ScriptUtils.ShowMsg("提示", ex.Message);
    }

    #region 页面权限

    /// <summary>
    /// 页面权限
    /// </summary>
    private void Limits()
    {
        try
        {
            string Url = Request.AppRelativeCurrentExecutionFilePath != null ? Request.AppRelativeCurrentExecutionFilePath : "";
            Url = Url.Replace("~/", "").ToUpper();

            //判断公共页面
            foreach (string sTempComm in CommonUrl())
            {
                if (Url.Contains(sTempComm.ToUpper()))
                {
                    return;
                }
            }

            Bd_Base_PageBLL bd_Base_PageBLL = new Bd_Base_PageBLL();
            PbProject.Logic.User.User_PermissionsBLL uPermissionsBLL = new PbProject.Logic.User.User_PermissionsBLL();
            IList<Bd_Base_Page> iPostResult = new List<Bd_Base_Page>();

            List<Bd_Base_Page> pageList = bd_Base_PageBLL.GetList();

            User_Permissions userPermissions = uPermissionsBLL.GetById(mUser.DeptId);
            string valuePermissions = "," + userPermissions.Permissions + ",";
            string temp = "";
            int pageListCount = pageList != null ? pageList.Count : 0;

            //得到权限
            for (int j = 0; j < pageListCount; j++)
            {
                temp = "," + pageList[j].PageIndex.ToString() + ",";
                if (valuePermissions.Contains(temp) && pageList[j].RoleType == mCompany.RoleType)
                {
                    iPostResult.Add(pageList[j]);
                }
            }


            int x = 0;
            for (int i = 0; i < iPostResult.Count; i++)
            {
                if (iPostResult[i].PageURL.ToUpper().Contains(Url.ToUpper()))
                {
                    x++;
                    break;
                }
            }
            if (x == 0)
            {
                string[] str = Url.Split('/');
                if (str.Length > 2)
                {
                    //无权限提示
                    Response.Redirect("../../Power.htm", true);
                }
                if (Url.Contains("/"))
                {
                    //无权限提示
                    Response.Redirect("../Power.htm", true);
                }
                else
                {
                    //无权限提示
                    Response.Redirect("Power.htm", true);
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 公共页面
    /// </summary>
    /// <returns></returns>
    private string[] CommonUrl()
    {
        string[] str = new string[31];
        str[0] = "Default.aspx";
        str[1] = "Left.aspx";
        str[2] = "Top.aspx";
        str[3] = "Middle.aspx";
        str[4] = "UpdatePwd.aspx";
        str[5] = "Sys/NewBulletinList.aspx";
        str[6] = "Air/Buy/FlyerList.aspx";
        str[7] = "Ajax/BlackData.aspx";
        str[8] = "index.aspx";
        str[9] = "Ajax/AjaxTicketSet.aspx";
        str[10] = "Ajax/UserZh.aspx";
        str[11] = "Ajax/FlightAjax.aspx";
        str[12] = "Ajax/HotelData.aspx";
        str[13] = "Ajax/OrderInfo.aspx";
        str[14] = "Ajax/PolicyAjax.aspx";
        str[15] = "Ajax/Positiondata.aspx";
        str[16] = "Ajax/Positiondata2.aspx";
        str[17] = "Ajax/Positiondata3.aspx";
        str[18] = "Company/EmployeesEdit.aspx";
        str[19] = "Pay/Pay.aspx";
        str[20] = "Pay/BaiTuoPay.aspx";
        str[21] = "Financial/Pay.aspx";
        str[22] = "Ajax/SelStopsAjax.aspx";
        str[23] = "Ajax/FlightAjax.aspx";
        str[24] = "Ajax/SpPatAjax.aspx";
        str[25] = "Air/Buy/noticeList.aspx";
        str[26] = "Top2.aspx";
        str[27] = "Ajax/RtAjax.aspx";
        str[28] = "Ajax/RtAjax2.aspx";
        str[29] = "Ajax/TripAjax.aspx";
        str[30] = "Bottom.aspx";

        return str;
    }

    #endregion

    #region 基本方法

    /// <summary>
    /// 替换除了\r|\n|\t外的不可见字符 
    /// </summary>
    /// <param name="strData"></param>
    /// <returns></returns>
    public string RemoveHideChar(string strData)
    {
        //替换除了\r|\n|\t外的不可见字符
        strData = Regex.Replace(strData, @"[^A-Z0-9a-z|\x21-\x7E|\u4e00-\u9fa5|\r|\n|\t]", " ");
        return strData;
    }

    /// <summary>
    /// 后台模拟Js方法escape对字符编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string escape(string s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            StringBuilder sb = new StringBuilder();
            char[] charr = s.ToCharArray();
            string abc = "-_.*+/@";//不编码的符号
            for (int i = 0; i < charr.Length; i++)
            {
                byte[] ba = System.Text.Encoding.UTF8.GetBytes(charr[i].ToString());
                if (ba.Length == 1)
                {
                    //是否为字母或数字
                    bool bb1 = char.IsLetterOrDigit(charr[i]) || abc.Contains(charr[i].ToString());
                    if (bb1)
                    {
                        sb.Append(charr[i].ToString());
                    }
                    else
                    {
                        string ss = "%" + System.Text.Encoding.ASCII.GetBytes(new char[] { charr[i] })[0].ToString("X2");
                        sb.Append(ss);
                    }
                }
                if (ba.Length >= 2)
                {
                    byte[] bb = System.Text.Encoding.Unicode.GetBytes(charr[i].ToString());
                    if (bb.Length == 2)
                    {
                        sb.Append("%u");
                        sb.Append(bb[1].ToString("X2"));
                        sb.Append(bb[0].ToString("X2"));
                    }
                }
            }
            s = sb.ToString();
        }
        return s;
    }

    /// <summary>
    /// 构造用户发送指令的配置信息
    /// </summary>
    /// <param name="baseParametersList"></param>
    /// <returns></returns>
    public ConfigParam GetConfigParam(string strHeiPingCanShu, string strDaPeiZhiCanShu)
    {
        ConfigParam configparam = new ConfigParam();
        try
        {
            string[] HeiPingCanShu = strHeiPingCanShu.Split('|');
            string[] DaPeiZhiCanShu = strDaPeiZhiCanShu.Split('|');
            //大配置信息
            configparam.BigCfgIP = DaPeiZhiCanShu[0];
            configparam.BigCfgPort = DaPeiZhiCanShu[1];
            configparam.BigCfgOffice = DaPeiZhiCanShu[2];
            configparam.BigCfgAccount = DaPeiZhiCanShu[3];
            //小配置信息
            configparam.WebBlackIP = HeiPingCanShu[0];
            configparam.WebBlackPort = HeiPingCanShu[1];
            configparam.WhiteScreenIP = HeiPingCanShu[2];
            configparam.WhiteScreenPort = HeiPingCanShu[3];
            configparam.Office = HeiPingCanShu[4];
            configparam.WebBlackAccount = HeiPingCanShu[5];
            configparam.WebBlackPwd = HeiPingCanShu[6];
            configparam.ECPort = HeiPingCanShu[7];
            configparam.TicketCompany = HeiPingCanShu[8];
            configparam.IataCode = HeiPingCanShu[9];
        }
        catch (Exception)
        {
        }
        return configparam;
    }
    #endregion

    /// <summary>
    /// 通过字典表类型 id 获取字典表数据
    /// </summary>
    /// <param name="parentId">字典表类型id</param>
    /// <returns></returns>
    public List<Bd_Base_Dictionary> GetDictionaryList(string parentId)
    {
        return new Bd_Base_DictionaryBLL().GetListByParentID(int.Parse(parentId));
    }

    /// <summary>
    /// 通过字典表类型 id 获取字典表数据
    /// </summary>
    /// <param name="parentId">字典表类型id</param>
    /// <returns></returns>
    public string GetDictionaryName(string parentId, string childId)
    {
        return new Bd_Base_DictionaryBLL().GetDictionaryName(parentId, childId);
    }
    /// <summary>
    /// 获取图片路径
    /// </summary>
    /// <param name="cpyname"></param>
    /// <param name="imgname"></param>
    /// <returns></returns>
    public string GetimgUrl(string imgname)
    {
        string url = "";
        try
        {
            string cpyno = mCompany.UninCode;
            string strwhere = "", cpywhere = "", cpyname="";
            if (cpyno.Length>=18)
            {
                switch (cpyno.Length)
                {
                    case 30:
                        strwhere = string.Format("CpyNo in ('{0}','{1}') and SetName='isShowDuLiInfo'", cpyno.Substring(0, 24), cpyno.Substring(0, 18));
                        break;
                    case 24:
                        strwhere = string.Format("CpyNo in ('{0}','{1}') and SetName='isShowDuLiInfo'", cpyno, cpyno.Substring(0, 18));
                        break;
                    default:
                        strwhere = string.Format("CpyNo = '{0}' and SetName='isShowDuLiInfo'", cpyno);
                        break;
                }

                List<Bd_Base_Parameters> listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { strwhere }) as List<Bd_Base_Parameters>;
                if (listParameters != null && listParameters.Count > 0)
                {
                    foreach (var item in listParameters)
                    {
                        if (item.SetValue == "1")
                        {
                            //cpywhere = string.Format("UninCode='{0}'", item.CpyNo);
                            cpywhere = item.CpyNo;
                            break;
                        }
                    }
                    if (cpywhere.Length ==0)
                    {
                        //cpyname = BaseParams.getParams(supBaseParametersList).CssURL;//运营商logo
                        cpyname = mSupCompany.UninCode;
                    }
                }
                else
                {
                    //cpyname = BaseParams.getParams(supBaseParametersList).CssURL;//运营商logo
                    cpyname = mSupCompany.UninCode;
                }
            }
            else if (cpyno.Length==12)
            {
                //cpyname = BaseParams.getParams(supBaseParametersList).CssURL;//运营商logo
                cpyname = mSupCompany.UninCode;
            }
            
            if (!string.IsNullOrEmpty(cpywhere))//显示独立分销logo
            {
                //List<User_Company> listcpy = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { cpywhere }) as List<User_Company>;
                //if (listcpy != null && listcpy.Count > 0)
                //{
                //    cpyname = listcpy[0].UninCode;
                //}
                cpyname = cpywhere;
            }
           
            if (!string.IsNullOrEmpty(cpyname))
            {
                
                url = "~/../Images/" + cpyname + "/" + imgname;
                url = File.Exists(Server.MapPath("Images/" + cpyname + "/" + imgname)) == true ? url : "~/../Images/" + imgname;
            }
            else
            {
                url = "~/../Images/" + imgname;
            }
        }
        catch (Exception)
        {
            url = "~/../Images/" + imgname;
            throw;
        }

        return url;
    }


    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string KongZhiXiTong
    {
        get
        {
            return (supBaseParametersList == null || supBaseParametersList.Count == 0) ? "" : BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
        }
    }

    /// <summary>
    /// 供应控制分销开关 
    /// </summary>
    public string GongYingKongZhiFenXiao
    {
        get
        {
            return (supBaseParametersList == null || supBaseParametersList.Count == 0) ? "" : BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
        }
    }
}
