using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using DataBase.Data;
using PbProject.Dal.Log;

namespace PbProject.Logic.Log
{
    public class Log_OperationBLL
    {
        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="logOperation"></param>
        /// <returns></returns>
        public int InsertLog_Operation(Log_Operation logOperation)
        {
            #region 记录操作日志
            int count = 0;
            try
            {
                IHashObject paramter = new HashObject();
                if (!string.IsNullOrEmpty(logOperation.ModuleName))
                    paramter.Add("ModuleName", logOperation.ModuleName);
                if (!string.IsNullOrEmpty(logOperation.OperateType))
                    paramter.Add("OperateType", logOperation.OperateType);
                if (!string.IsNullOrEmpty(logOperation.LoginName))
                    paramter.Add("LoginName", logOperation.LoginName);
                if (!string.IsNullOrEmpty(logOperation.CpyNo))
                    paramter.Add("CpyNo", logOperation.CpyNo);
                if (!string.IsNullOrEmpty(logOperation.OptContent))
                    paramter.Add("OptContent", logOperation.OptContent);
                if (!string.IsNullOrEmpty(logOperation.UserName))
                    paramter.Add("UserName", logOperation.UserName);
                if (!string.IsNullOrEmpty(logOperation.OrderId))
                    paramter.Add("OrderId", logOperation.OrderId);
                paramter.Add("ClientIP", System.Web.HttpContext.Current.Request.UserHostAddress);
                paramter.Add("CreateTime", Convert.ToDateTime(DateTime.Now));
                paramter.Add("A1", logOperation.A1);
                paramter.Add("A2", logOperation.A2);
                if (!string.IsNullOrEmpty(logOperation.A3))
                    paramter.Add("A3", logOperation.A3);
                if (!string.IsNullOrEmpty(logOperation.A3))
                    paramter.Add("A4", logOperation.A3);
                if (!string.IsNullOrEmpty(logOperation.A3))
                    paramter.Add("A5", logOperation.A3);
                count = Log_OperationDal.Insert(paramter);
            }
            catch (Exception ex)
            {
                PbProject.WebCommon.Log.Log.RecordLog("Log_OperationBLL", ex.Message, true, null);
            }
            return count;
            #endregion
        }
    }
}
