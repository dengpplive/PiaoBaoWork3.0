using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using System.Data;
using DataBase.Data;

namespace PbProject.Dal.Mapping
{
    public class SqlHelper
    {
        public int QueryCount(string sql)
        {
            int count = 0;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                count = (int)db.ExecuteScalerSQL(sql);
            }
            return count;
        }
        /// <summary>
        /// 通过Sql 执行：增加
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int INSERTsql(string sql)
        {
            int count = -1;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.WRITE, 0)))
            {
                count = db.ExecuteNonQuerySQL(sql);
            }
            return count;
        }

        /// <summary>
        /// 通过Sql 执行：删除
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int DELETEsql(string sql)
        {
            int count = -1;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.DELETE, 0)))
            {
                count = db.ExecuteNonQuerySQL(sql);
            }
            return count;
        }

        /// <summary>
        /// 通过Sql 执行：改
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int UPDATEsql(string sql)
        {
            int count = -1;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.UPDATE, 0)))
            {
                count = db.ExecuteNonQuerySQL(sql);
            }
            return count;
        }

        /// <summary>
        /// 通过Sql 执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTableBySql(string sql)
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteSQL(sql);
            }
            return dt;
        }

        /// <summary>
        /// 通过Sql 执行查询
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public DataTable EexcProc(string procName, HashObject queryParamter)
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteProcedure(procName, queryParamter);
            }
            return dt;
        }
       
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public int ExecuteNonQueryProcedure(string procName, HashObject queryParamter)
        {
            int count = -1;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                count = db.ExecuteNonQueryProcedure(procName, queryParamter);
            }
            return count;
        }

        /// <summary>
        /// 通过Sql 执行查询返回多个表数据
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public DataTable[] MulExecProc(string procName, HashObject queryParamter)
        {
            DataTable[] dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteProcedureEx(procName, queryParamter);
            }
            return dt;
        }

        /// <summary>
        /// SQL事务处理
        /// </summary>
        /// <param name="SQLList">sql列表</param>
        /// <param name="ErrMsg">内部错误信息</param>
        /// <returns></returns>
        public bool ExecuteSqlTran(List<string> SQLList, out string ErrMsg)
        {
            bool result = false;
            ErrMsg = "";//将内部异常信息传出去
            if (SQLList == null || SQLList.Count == 0) { ErrMsg = "SQLList事务列表不能为空"; return result; };
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.WRITE, 0)))
            {
                try
                {
                    db.BeginTransaction();
                    foreach (string sql in SQLList)
                    {
                        db.ExecuteNonQuerySQL(sql);
                    }
                    db.CommitTransaction();
                    result = true;
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();
                    string strSQL = string.Join("\r\n", SQLList.ToArray());
                    ErrMsg = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "执行错误sql语句:" + strSQL + "============================================\r\n异常信息:" + ex.Message + ex.StackTrace + ex.Source;
                    try
                    {
                        DataBase.LogCommon.Log.Error(ErrMsg, ex);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return result;
        }
    }
}
