using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using System.Data;
using DataBase.Data;
using PbProject.Model;
using PbProject.Dal.Mapping;

namespace PbProject.Dal.SQLEXDAL
{
    public class SQLEXDAL_Base : SqlHelper
    {
        /// <summary>
        /// 开户添加数据
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="parameter">执行类型</param>
        public int uporinAccount(IHashObject parameter, int type)
        {
            int result = 0;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.WRITE, 0)))
            {
                try
                {
                    db.BeginTransaction();
                    if (type == 0)
                    {
                        db.ExecuteNonQuerySQL(parameter["User_Company"].ToString());
                        db.ExecuteNonQuerySQL(parameter["User_Employees"].ToString());
                        db.ExecuteNonQuerySQL(parameter["User_Permissions"].ToString());

                    }
                    if (type == 1)
                    {
                        db.ExecuteNonQuerySQL(parameter["User_Company"].ToString());
                        db.ExecuteNonQuerySQL(parameter["User_Employees"].ToString());
                        db.ExecuteNonQuerySQL(parameter["User_Permissions"].ToString());
                        db.ExecuteNonQuerySQL(parameter["Bd_Base_Parameters"].ToString());

                    }
                    //db.ExecuteNonQuerySQL(parameter["Bd_Base_Parameters"].ToString());
                    db.CommitTransaction();
                    //if (type == 0)
                    //{
                    //    bool acbUser1 = new ControlBase.BaseData<User_Company>().RefreshCache();
                    //    bool acbUser2 = new ControlBase.BaseData<User_Employees>().RefreshCache();
                    //    bool acbUser3 = new ControlBase.BaseData<User_Permissions>().RefreshCache();
                    //}
                    //if (type == 1)
                    //{
                    //    bool acbUser1 = new ControlBase.BaseData<User_Company>().RefreshCache();
                    //    bool acbUser2 = new ControlBase.BaseData<User_Employees>().RefreshCache();
                    //    bool acbUser3 = new ControlBase.BaseData<User_Permissions>().RefreshCache();
                    //    bool acbUser4 = new ControlBase.BaseData<Bd_Base_Parameters>().RefreshCache();
                    //}




                    result = 1;
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取下级公司编号
        /// </summary>
        /// <param name="UninCode"></param>
        /// <returns></returns>
        public string GetUninCode(string UninCode)
        {
            string resutl = "";
            HashObject paramno = new HashObject();
            paramno.Add("cpyno", UninCode);

            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteProcedure("GetNewChildCpyNo", paramno);
                if (dt != null)
                {
                    resutl = dt.Rows[0][0].ToString();
                }
            }
            return resutl;
        }

        /// <summary>
        /// 查询开户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetViewInfoByStrWhere(string viewname, string sqlwhere)
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                if (!string.IsNullOrEmpty(sqlwhere))
                {
                    sqlwhere = " where " + sqlwhere;
                }
                dt = db.ExecuteSQL("select * from " + viewname + sqlwhere);

            }
            return dt;
        }

        /// <summary>
        /// 查询 上次登录信息
        /// </summary>
        /// <param name="loginAccount">登录账号</param>
        /// <returns></returns>
        public User_LoginLog GetByLoginAccount(string loginAccount)
        {
            User_LoginLog model = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                db.AddParameter("@LoginAccount", loginAccount);

                DataRow dr = db.ExecuteFirstRowSQL("select top 1 * from User_LoginLog where loginState='成功' and LoginAccount=@LoginAccount order by LoginTime desc");
                model = MappingHelper<User_LoginLog>.FillModel(dr);
            }
            return model;
        }

        /// <summary>
        /// 获取数据库所有表名 用户表
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public DataTable GetSystemTableName()
        {

            string sql = "SELECT name FROM SysObjects Where XType='U' ORDER BY Name";
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteSQL(sql);
            }
            return dt;
        }

        /// <summary>
        /// 通过角色类型，更新所有对应角色用户的页面权限
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <returns></returns>
        public int Update_User_Permissions(int roleType)
        {
            int result = -1;
            try
            {
                PbProject.Dal.ControlBase.BaseData<User_Permissions> bd = new ControlBase.BaseData<User_Permissions>();
                IHashObject iho = new HashObject();
                iho.Add("RoleType", roleType);
                result = bd.ExecuteNonQueryProcedure("Proc_Update_User_Permissions", iho);
            }
            catch (Exception)
            {
            }
            return result;
        }
        /// <summary>
        /// 导入政策
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool ImportPolicy(IHashObject parameter)
        {
            bool rs = false;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.WRITE, 0)))
            {
                try
                {
                    db.BeginTransaction();
                    for (int i = 0; i < parameter.Count; i++)
                    {
                        db.ExecuteNonQuerySQL(parameter[i.ToString()].ToString());
                    }
                    db.CommitTransaction();
                    rs = true;
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                }
            }
            return rs;
        }
        /// <summary>
        /// 获取政策excel
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DataTable GetPolicyExcelByStrWhere(string StrWhere)
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteSQL("select * from V_PolicyExcel where " + StrWhere);
            }
            return dt;
        }

        /// <summary>
        /// 获取直属于下级
        /// </summary>
        /// <param name="StrWhere"></param>
        /// <returns></returns>
        public DataTable GetUser_Company_Empolyees(string StrWhere)
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteSQL("select * from V_User_Company_Empolyees where " + StrWhere);
            }
            return dt;
        }
        /// <summary>
        /// 获取供应商和落地运营商所有下级 不含员工
        /// </summary>
        /// <param name="StrWhere">供应商和落地运营商公司编号</param>
        /// <returns></returns>
        public DataTable GetGYLowerEmpolyees(string GYCpyNo)
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteSQL(string.Format("select  u.LoginName,u.CpyNo,u.id Uid,c.UninAllName,c.UninCode,c.id Cid from dbo.User_Employees u,User_Company c where left(u.CpyNo,12)='{0}' and u.CpyNo<>'{0}' and  u.CpyNo=c.UninCode and u.IsAdmin=0 order by u.LoginName", GYCpyNo));
            }
            return dt;
        }
        /// <summary>
        /// 获取供应商和落地运营商
        /// </summary>       
        /// <returns></returns>
        public DataTable GetGYEmpolyees()
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteSQL("select  u.LoginName,u.UserName,u.CpyNo,u.id Uid,c.UninAllName,c.UninCode,c.id Cid,c.RoleType RoleType from dbo.User_Employees u,User_Company c where u.CpyNo=c.UninCode and  c.RoleType in(2,3) and u.IsAdmin=0 and c.AccountState=1 order by c.UninAllName");
            }
            return dt;
        }
        /// <summary>
        /// 获取订单编号
        /// </summary>
        /// <param name="no">类型:0 机票支付 ,1 充值支付 ，2 短信支付</param>
        /// <returns></returns>
        public string GetNewOrderId(string no)
        {
            string result = "";
            try
            {
                HashObject paramno = new HashObject();
                paramno.Add("Number", no);

                DataTable dt = null;
                using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
                {
                    dt = db.ExecuteProcedure("GetNewOrderId", paramno);
                    if (dt != null)
                    {
                        result = dt.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteStrSQL(string sql)
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteSQL(sql);
            }
            return dt;
        }
        /// <summary>
        /// 执行增删改语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ExecuteNonQuerySQLInfo(string sql)
        {
            bool rs = false;
            try
            {
                using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.DELETE, 0)))
                {
                    rs = db.ExecuteNonQuerySQL(sql) > 0 ? true : false; ;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return rs;
        }

        /// <summary>
        /// 获取落地运营商和供应
        /// </summary>
        /// <returns></returns>
        public DataTable GetLDGY()
        {
            DataTable table = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                string sql = "select U.LoginName,U.UserName, C.UninAllName,C.UninAllName+'['+U.LoginName+']['+U.UserName+']' ShowText,  C.UninCode CpyNo,C.RoleType RoleType,AccountState,C.RobInnerTime,C.RobSetting  from User_Company C,User_Employees U  where U.CpyNo=C.UninCode and C.RoleType in(2,3) and IsAdmin=0 and AccountState=1 ";
                table = db.ExecuteSQL(sql);
            }
            return table;
        }
        /// <summary>
        /// 根据订单号获取管理员账号和公司信息
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public DataTable[] GetUserInfoByOrderId(string OrderId)
        {
            HashObject queryParamter = new HashObject();
            queryParamter.Add("OrderId", OrderId);
            return MulExecProc("GetUserInfoByOrderId", queryParamter);
        }

        /// <summary>
        /// 获取落地运营商配置信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetCompanyConfigInfo()
        {
            DataTable table = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                string sql = "select P.SetName,P.SetValue,C.* from User_Company C,Bd_Base_Parameters P where  C.RoleType in(2) and C.AccountState=1 and P.CpyNo=C.UninCode and P.SetName in('heiPingCanShu','daPeiZhiCanShu') ";
                table = db.ExecuteSQL(sql);
            }
            return table;
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="ProcName">存储过程名</param>
        /// <param name="PageIndex">分页当前索引 </param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="TableSQL">SQL语句 查询出来的的一张表</param>
        /// <param name="strWhere">过滤条件</param>
        /// <param name="Sort">排序字段</param>
        /// <param name="TotalCount">返回数据总条数</param>
        /// <returns></returns>
        public DataTable ExecProcPager(string ProcName, int PageIndex, int PageSize, string TableSQL, string strWhere, string Sort, out int TotalCount)
        {
            DataTable dt = null;
            TotalCount = 0;
            IHashObject InputParam = new HashObject();
            IHashObject outputParam = new HashObject();

            InputParam.Add("PageIndex", PageIndex);
            InputParam.Add("pageSize", PageSize);
            InputParam.Add("tableSQL", TableSQL);
            InputParam.Add("SqlWhere", string.IsNullOrEmpty(strWhere) ? "" : strWhere);
            InputParam.Add("SortOrder", Sort);
            InputParam.Add("PageTotal", 0);
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteProcedureAndOutputParameter(ProcName, out outputParam, InputParam);
                TotalCount = outputParam.GetValue<int>("PageTotal");
            }
            return dt;
        }
    }
}
