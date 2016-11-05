using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using PbProject.ConsoleServerProc.WebPromptService;
namespace ConsoleServerProc
{
    public class WebLoginManage
    {
        static WebTravelPrintService PM = new WebTravelPrintService();
        public WebLoginManage()
        {
            PM.Url = "http://webservices3.mypb.cn/WebTravelPrintService.asmx";
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="LoginPwd"></param>
        /// <param name="LoginType"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static DataSet Login(string LoginName, string LoginPwd, string LoginType, out int flag)
        {
            flag = 0;
            DataSet ds = PM.UserLogin(LoginName, LoginPwd);
            if (ds == null || ds.Tables.Count == 0)
            {
                flag = 3;
            }
            else if (ds != null)
            {

                if (ds.Tables.Count == 3 || ds.Tables.Count == 5)
                {
                    //登录成功
                    flag = 1;
                }
                else if (ds.Tables.Count == 1)
                {
                    flag = 2;
                }
                else
                {
                    flag = 3;
                }
            }
            return ds;
        }

    }
}
