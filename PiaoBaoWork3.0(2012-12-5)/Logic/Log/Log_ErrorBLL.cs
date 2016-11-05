using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using System.Web;
using PbProject.Dal.Log;

namespace PbProject.Logic.Log
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Log_ErrorBLL
    {
        /// <summary>
        /// 记录日志,数据库错误日志
        /// </summary>
        /// <param name="logRrror"></param>
        public int InsertLog_Error(Log_Error logRrror)
        {
            #region 记录错误日志
            int count = 0;
            try
            {
                HashObject paramter = new HashObject();
                if (!string.IsNullOrEmpty(logRrror.Page))
                    paramter.Add("Page", logRrror.Page);
                if (!string.IsNullOrEmpty(logRrror.CpyNo))
                    paramter.Add("CpyNo", logRrror.CpyNo);
                if (!string.IsNullOrEmpty(logRrror.LoginName))
                    paramter.Add("LoginName", logRrror.LoginName);
                if (!string.IsNullOrEmpty(logRrror.Method))
                    paramter.Add("Method", logRrror.Method);
                if (!string.IsNullOrEmpty(logRrror.ErrorContent))
                    paramter.Add("ErrorContent", logRrror.ErrorContent);
                if (!string.IsNullOrEmpty(logRrror.DevName))
                    paramter.Add("DevName", logRrror.DevName);
                paramter.Add("ClientIP", System.Web.HttpContext.Current.Request.UserHostAddress);
                paramter.Add("ErorrTime", Convert.ToDateTime(DateTime.Now));
                paramter.Add("A1", logRrror.A1);
                paramter.Add("A2", logRrror.A2);
                if (!string.IsNullOrEmpty(logRrror.A3))
                    paramter.Add("A3", logRrror.A3);
                if (!string.IsNullOrEmpty(logRrror.A3))
                    paramter.Add("A4", logRrror.A3);
                if (!string.IsNullOrEmpty(logRrror.A3))
                    paramter.Add("A5", logRrror.A3);
                count = Log_ErrorDal.Insert(paramter);
            }
            catch (Exception ex)
            {
                PbProject.WebCommon.Log.Log.RecordLog("Log_ErrorBll", ex.Message, true, null);
            }
            return count;
            #endregion
        }

    }
}
