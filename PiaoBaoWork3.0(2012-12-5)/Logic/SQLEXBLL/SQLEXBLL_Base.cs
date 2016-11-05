using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Dal.SQLEXDAL;
using DataBase.Data;
using System.Data;
using PbProject.Model;

namespace PbProject.Logic.SQLEXBLL
{
    public class SQLEXBLL_Base
    {
        SQLEXDAL_Base sqlEXDAL_Base = new SQLEXDAL_Base();

        /// <summary>
        /// 开户添加数据
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="parameter">执行类型</param>
        public int uporinAccount(IHashObject parameter,int type)
        {
            return sqlEXDAL_Base.uporinAccount(parameter, type);
        }

        /// <summary>
        /// 获取下级公司编号
        /// </summary>
        /// <param name="UninCode"></param>
        /// <returns></returns>
        public string GetUninCode(string UninCode)
        {
            return sqlEXDAL_Base.GetUninCode(UninCode);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetViewInfoByStrWhere(string viewname,string sqlwhere)
        {
            return sqlEXDAL_Base.GetViewInfoByStrWhere(viewname, sqlwhere);
        }

        /// <summary>
        /// 查询 上次登录信息
        /// </summary>
        /// <param name="loginAccount">登录账号</param>
        /// <returns></returns>
        public User_LoginLog GetByLoginAccount(string loginAccount)
        {
            return sqlEXDAL_Base.GetByLoginAccount(loginAccount);
        }

        /// <summary>
        /// 获取所有用户表名
        /// </summary>
        /// <returns></returns>
        public DataTable GetSystemTableName()
        {
            return sqlEXDAL_Base.GetSystemTableName();
        }

        /// <summary>
        /// 导入政策
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool ImportPolicy(IHashObject parameter)
        {
            return sqlEXDAL_Base.ImportPolicy(parameter);
        }

        /// <summary>
        /// 获取政策excel
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DataTable GetPolicyExcelByStrWhere(string StrWhere)
        {
            return sqlEXDAL_Base.GetPolicyExcelByStrWhere(StrWhere);
        }

        /// <summary>
        /// 创建订单编号
        /// </summary>
        /// <param name="no">类型:0 机票支付 ,1 充值支付 ，2 短信支付</param>
        /// <returns></returns>
        public string GetNewOrderId(string no)
        {
            return sqlEXDAL_Base.GetNewOrderId(no);
        }

         /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteStrSQL(string sql)
        {
            return sqlEXDAL_Base.ExecuteStrSQL(sql);
        }

         /// <summary>
        /// 执行增删改语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ExecuteNonQuerySQLInfo(string sql)
        {
            return sqlEXDAL_Base.ExecuteNonQuerySQLInfo(sql);
        }

    }
}
