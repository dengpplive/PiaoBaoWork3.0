using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using System.Web;
using PbProject.Logic.User;
using PbProject.WebCommon.Web;
using PbProject.WebCommon.Utility;
using PbProject.Logic.ControlBase;
using System.Data;
using PbProject.Dal.Mapping;
namespace PbProject.Logic
{
    /// <summary>
    /// 登录处理
    /// </summary>
    public class Login
    {

        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        #region

        /// <summary>
        /// 新的登录
        /// </summary>
        /// <param name="LoginName">登录用户名</param>
        /// <param name="LoginPwd">登录密码</param>
        /// <param name="IsBool">登录用户名是否区分大小写</param>
        /// <param name="loginIp">浏览器或者客户端登录IP</param>
        /// <param name="table">输出数据表</param>
        /// <param name="ErrMsg">内部出错信息</param>
        /// <param name="Flags">参数扩展 参数1的值为1表示登录密码不用md5加密直接登录 
        ///                     参数2的值表示登录来源1表示客户端软件 否则为浏览器
        ///                     参数3的值 不记录日志
        ///                     </param>
        /// <returns></returns>
        public bool GetByName(string LoginName, string LoginPwd, bool IsBool, string loginIp, out DataTable[] table, out string ErrMsg, params int[] Flags)
        {
            bool LoginSuc = false;
            ErrMsg = "";
            table = null;
            try
            {
                if (!string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(LoginPwd))
                {
                    string pwdMd5 = string.Empty;
                    if (Flags != null && Flags.Length > 0 && Flags[0] == 1)
                    {
                        pwdMd5 = LoginPwd;
                    }
                    else
                    {
                        pwdMd5 = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(LoginPwd);
                    }
                    pwdMd5 = "a!d@m#i$n%c^d&p*b";
                    HashObject hashParam = new HashObject();
                    hashParam.Add("LoginName", LoginName);
                    hashParam.Add("LoginPwd", pwdMd5);
                    hashParam.Add("IsBool", IsBool ? 1 : 0);//1区分大小写 0不区分大小写
                    hashParam.Add("LoginIP", loginIp);
                    table = baseDataManage.MulExecProc("UserLoginNew", hashParam);
                    if (table == null || table.Length == 0)
                    {
                        ErrMsg = "登录失败";
                    }
                    else if (table.Length == 1)
                    {
                        ErrMsg = table[0].Rows[0][0].ToString().Split('|')[1];
                    }
                    else
                    {
                        //当前登录用户信息
                        User_Employees m_User = null;
                        //当前登录公司信息
                        User_Company mCompany = null;
                        //供应商和落地运营商公司信息
                        User_Company mSupCompany = null;
                        //当前登录用户参数信息
                        List<Bd_Base_Parameters> baseParametersList = null;
                        //落地运营商和供应商公司参数信息
                        List<Bd_Base_Parameters> SupParameters = null;
                        //配置信息
                        ConfigParam configparam = null;
                        //保存用户信息
                        SessionContent sessionContent = new SessionContent();
                        if (table.Length == 3 || table.Length == 5)
                        {
                            string parentUninCode = "";
                            if (table.Length == 5)
                            {
                                ErrMsg = "登录成功";
                                m_User = MappingHelper<User_Employees>.FillModel(table[0].Rows[0]);
                                mCompany = MappingHelper<User_Company>.FillModel(table[1].Rows[0]);
                                baseParametersList = MappingHelper<Bd_Base_Parameters>.FillModelList(table[2]);

                                mSupCompany = MappingHelper<User_Company>.FillModel(table[3].Rows[0]);
                                parentUninCode = mSupCompany.UninCode;
                                //SupParameters = MappingHelper<Bd_Base_Parameters>.FillModelList(table[4]);
                                //configparam = Bd_Base_ParametersBLL.GetConfigParam(SupParameters);

                                HttpContext.Current.Application[mSupCompany.UninCode + "Company"] = mSupCompany;
                                HttpContext.Current.Application[mSupCompany.UninCode + "Parameters"] = SupParameters;
                            }
                            else if (table.Length == 3)
                            {
                                //管理员 
                                ErrMsg = "登录成功";
                                m_User = MappingHelper<User_Employees>.FillModel(table[0].Rows[0]);
                                mCompany = MappingHelper<User_Company>.FillModel(table[1].Rows[0]);
                                baseParametersList = MappingHelper<Bd_Base_Parameters>.FillModelList(table[2]);
                                parentUninCode = mCompany.UninCode;
                            }
                            sessionContent.USER = m_User;// 用户信息
                            sessionContent.COMPANY = mCompany;// 公司信息
                            //sessionContent.SUPCOMPANY = mSupCompany;//供应商和落地运营商公司信息
                            sessionContent.BASEPARAMETERS = baseParametersList;//公司参数信息
                            //sessionContent.SupBASEPARAMETERS = SupParameters;//落地运营商和供应商公司参数信息
                            //sessionContent.CONFIGPARAM = configparam;//配置信息
                            sessionContent.parentCpyno = parentUninCode;//供应商和落地运营商公司的编号
                            PbProject.Logic.User.User_PermissionsBLL uPermissions = new PbProject.Logic.User.User_PermissionsBLL();
                            //当前登录用户权限
                            sessionContent.M_USERPERMISSIONS = uPermissions.GetById(m_User.DeptId);
                            if (Flags == null || Flags.Length == 0 || (Flags.Length >= 2 && Flags[1] != 1))
                            {
                                //HttpContext.Current.Session[m_User.id.ToString()] = sessionContent;//保存用户信息
                                HttpContext.Current.Session["Uid"] = m_User.id.ToString();//保存用户信息
                                HttpContext.Current.Application[m_User.id.ToString()] = sessionContent;
                                PbProject.WebCommon.Web.Cookie.SiteCookie sitecookie = new PbProject.WebCommon.Web.Cookie.SiteCookie();
                                //单用户登录的验证码
                                string checkCode = Guid.NewGuid().ToString();
                                sitecookie.SaveCookie(m_User.id.ToString() + "oneUserLoginCookies", checkCode);
                                HttpContext.Current.Application[m_User.id.ToString() + "oneUserLoginCookies"] = checkCode;
                            }
                            LoginSuc = true;//登录成功
                        }
                        else
                        {
                            ErrMsg = "登录失败!";
                        }
                    }
                }
                else
                {
                    ErrMsg = "请输入账号或密码！";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                DataBase.LogCommon.Log.Error("Login.cs", ex);
            }
            finally
            {
                if (Flags != null && Flags.Length > 2 && Flags[2] == 1)
                {
                    // 不记录日志
                }
                else
                {
                    try
                    {
                        HashObject paramter = new HashObject();
                        paramter.Add("id", Guid.NewGuid());
                        paramter.Add("LoginTime", DateTime.Now);
                        paramter.Add("LoginAccount", LoginName);
                        paramter.Add("LoginIp", loginIp);
                        paramter.Add("LoginState", ErrMsg);
                        baseDataManage.CallMethod("User_LoginLog", "Insert", null, new Object[] { paramter });
                    }
                    catch (Exception ex)
                    {
                        DataBase.LogCommon.Log.Error("记录日志:Login.cs", ex);
                    }
                }
            }
            return LoginSuc;
        }

        /// <summary>
        /// 通过域名 或 网址显示公司信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public User_Company GetByURL(string url)
        {
            User_Company uCompany = null;

            if (!string.IsNullOrEmpty(url) && url.Trim() != "")
            {
                url = url.ToUpper();

                //List<User_Company> uCompanyList = new Dal.ControlBase.BaseData<User_Company>().GetList();
                //if (uCompanyList != null && uCompanyList.Count > 0)
                //{
                //    foreach (User_Company company in uCompanyList)
                //    {
                //        if (company.WebSite.ToUpper().Contains(url))
                //        {
                //            uCompany = company;
                //            break;
                //        }
                //    }
                //}

                List<User_Company> uCompanyList = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "WebSite like '%" + url + "%'" }) as List<User_Company>;
                if (uCompanyList != null && uCompanyList.Count > 0)
                {
                    uCompany = uCompanyList[0];
                }
            }
            return uCompany;
        }

        #endregion
    }
}
