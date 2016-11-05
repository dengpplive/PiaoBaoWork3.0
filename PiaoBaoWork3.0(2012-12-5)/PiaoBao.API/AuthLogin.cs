using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using System.Collections;
using PbProject.Model;
using PbProject.Dal.Mapping;
using PbProject.Logic.ControlBase;
using PbProject.Logic;
using DataBase.Data;
using System.Data;
using PbProject.Model.definitionParam;

namespace PiaoBao.API
{
    public class AuthLogin : IAuthenticate
    {
        private static Hashtable userInfoHashtable = new Hashtable();

        public bool Authenticate(string username, string password)
        {
            //登录验证
            UserLoginInfo userInfo = GetUserInfo(username, password, HttpContext.Current.Request.UserHostAddress);
            if (userInfo != null)
            {
                userInfoHashtable[username] = userInfo;
            }
            return userInfo != null;
        }

        /// <summary>
        /// 获取用户信息对象
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static UserLoginInfo GetUserInfo(string username)
        {
            return userInfoHashtable[username] as UserLoginInfo;
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="LoginName">登录用户名</param>
        /// <param name="LoginPwd">登录密码</param>
        /// <param name="loginIp">浏览器或者客户端登录IP</param>
        /// <param name="ErrMsg">内部出错信息</param>
        /// <returns></returns>
        private UserLoginInfo GetUserInfo(string loginName, string password, string loginIp)
        {
            UserLoginInfo userInfo = null;
            string errmsg = string.Empty;
            DataTable[] userTableArray;
            try
            {
                if (!string.IsNullOrEmpty(loginName) && !string.IsNullOrEmpty(password))
                {
                    string pwdMD5 = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(password);
                    userTableArray = GetUserTableArray(loginName, pwdMD5, loginIp);//获取用户信息表
                    if (userTableArray == null || userTableArray.Length == 0)
                    {
                        errmsg = "登录失败";
                        throw new Exception(errmsg);
                    }
                    else if (userTableArray.Length == 1)
                    {
                        errmsg = userTableArray[0].Rows[0][0].ToString().Split('|')[1];
                        throw new Exception(errmsg);
                    }
                    else
                    {
                        if (userTableArray.Length == 3 || userTableArray.Length == 5)
                        {
                            userInfo = GetLoginUserInfo(userTableArray);//获取登录用户信息对象
                        }
                        else
                        {
                            errmsg = "登录失败!";
                            throw new Exception(errmsg);
                        }
                    }
                }
                else
                {
                    errmsg = "请输入账号或密码！";
                    throw new Exception(errmsg);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return userInfo;
        }

        /// <summary>
        /// 获取用户信息表
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="pwdMD5">MD5密码</param>
        /// <param name="loginIp">客户端IP</param>
        /// <returns>返回用户信息表</returns>
        private DataTable[] GetUserTableArray(string loginName, string pwdMD5, string loginIp)
        {
            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            DataTable[] userTableArray;
            HashObject hashParam = new HashObject();

            hashParam.Add("LoginName", loginName);
            hashParam.Add("LoginPwd", pwdMD5);
            hashParam.Add("IsBool", 1);//区分大小写，写死------------------------------
            hashParam.Add("LoginIP", loginIp);
            userTableArray = baseDataManage.MulExecProc("UserLogin", hashParam);
            return userTableArray;
        }

        /// <summary>
        /// 获取登录用户信息对象
        /// </summary>
        /// <param name="userTableArray">用户信息表</param>
        /// <returns>返回用户信息对象</returns>
        private UserLoginInfo GetLoginUserInfo(DataTable[] userTableArray)
        {
            //当前登录用户信息
            User_Employees user = MappingHelper<User_Employees>.FillModel(userTableArray[0].Rows[0]);
            //当前登录公司信息
            User_Company company = MappingHelper<User_Company>.FillModel(userTableArray[1].Rows[0]);
            //当前登录用户公司参数信息
            List<Bd_Base_Parameters> baseParametersList = MappingHelper<Bd_Base_Parameters>.FillModelList(userTableArray[2]);
            var mSupCompany = MappingHelper<User_Company>.FillModel(userTableArray[3].Rows[0]);
            var supParameters = MappingHelper<Bd_Base_Parameters>.FillModelList(userTableArray[4]);
            var configparam = Bd_Base_ParametersBLL.GetConfigParam(supParameters);
            var FQP=PbProject.WebCommon.Utility.BaseParams.getParams(supParameters);
            //用户信息对象
            UserLoginInfo userInfo = new UserLoginInfo()
            {
                User = user,
                Company = company,
                BaseParametersList = baseParametersList,
                mSupCompany = mSupCompany,
                Configparam = configparam,
                SupParameters = supParameters,
                FQP = FQP
            };
            return userInfo;
        }
    }
      
    public class UserLoginInfo
    {
        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        public User_Employees User { get; set; }
        /// <summary>
        /// 当前登录公司信息
        /// </summary>
        public User_Company Company { get; set; }
        /// <summary>
        /// 当前登录用户参数信息
        /// </summary>
        public List<Bd_Base_Parameters> BaseParametersList { get; set; }
        /// <summary>
        /// 供应商和落地运营商公司信息
        /// </summary>
        public User_Company mSupCompany { get; set; }
        /// <summary>
        /// 落地运营商和供应商公司参数信息
        /// </summary>
        public List<Bd_Base_Parameters> SupParameters { get; set; }
        /// <summary>
        /// 配置信息
        /// </summary>
        public ConfigParam Configparam { get; set; }

        public BaseSwitch FQP { get; set; }
    }
}