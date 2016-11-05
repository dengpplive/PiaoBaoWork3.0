using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Dal.ControlBase;
using System.Reflection;
using DataBase.Data;
using System.Collections;
using System.Data;
using PbProject.Dal.SQLEXDAL;
using DataBase;
using PbProject.Dal;
namespace PbProject.Logic.ControlBase
{
    /// <summary>
    /// 基础数据管理类
    /// </summary>
    public class BaseDataManage : SQLEXDAL_Base
    {
        private Type baseType = null;
        private string DalPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "bin\\Dal.dll";
        public BaseDataManage()
        {
            Assembly asm = null;
            if (!System.IO.File.Exists(DalPath))
            {
                DalPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Dal.dll";
            }
            else
            {
                asm = Assembly.LoadFrom(DalPath);
            }
            asm = Assembly.LoadFrom(DalPath);
            string BaseDataPath = "PbProject.Dal.ControlBase.BaseData`1";
            baseType = asm.GetType(BaseDataPath);
        }

        public BaseDataManage(string tmpDalPath)
        {
            Assembly asm = Assembly.LoadFrom(tmpDalPath);
            string BaseDataPath = "PbProject.Dal.ControlBase.BaseData`1";
            baseType = asm.GetType(BaseDataPath);
        }
        /// <summary>
        /// 调用某个泛型指定方法名的方法
        /// </summary>
        /// <param name="tableName">泛型中T即表名也是Model中的类型名</param>
        /// <param name="MothedName">BaseData中的方法名</param>
        /// <param name="Params">输出参数 从里面返回数据 指定参数类型 key为参数索引从1开始 value为引用类型 out ref</param>        
        /// <param name="Params">调用方法的参数</param>
        /// <returns></returns>
        public object CallMethod(string tableName, string MethodName, IHashObject OutParams, params object[] InputParams)
        {
            object reObj = null;
            //构造泛型类
            Type Type_tableName = baseType.MakeGenericType(Type.GetType("PbProject.Model." + tableName + ",Model"));
            ConstructorInfo Contruct = Type_tableName.GetConstructor(new Type[0]);
            //实例化
            object instance = Contruct.Invoke(new object[] { });
            if (InputParams.Length > 0)
            {
                List<Type> lstType = new List<Type>();
                Type t = null;
                for (int i = 0; i < InputParams.Length; i++)
                {
                    t = InputParams[i].GetType();
                    if (OutParams != null && OutParams.Keys.Count > 0 && OutParams.Keys.Contains((i + 1).ToString()))
                    {
                        lstType.Add(t.MakeByRefType());
                    }
                    else
                    {
                        lstType.Add(t);
                    }
                }
                //调用方法
                reObj = Type_tableName.GetMethod(MethodName, lstType.ToArray()).Invoke(instance, InputParams);

                for (int i = 0; i < InputParams.Length; i++)
                {
                    if (OutParams != null && OutParams.Keys.Count > 0 && OutParams.Keys.Contains((i + 1).ToString()))
                        OutParams[(i + 1).ToString()] = InputParams[i];
                }
            }
            else
            {
                reObj = Type_tableName.GetMethod(MethodName).Invoke(instance, InputParams);
            }
            return reObj;
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="TotalCount">输出查询条件查出来的总记录数</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="PageIndex">当前页</param>      
        /// <param name="Fileds">筛选字段 默认*表示所有字段</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="OrderStr">排序字符串 如[a desc]</param>       
        /// <returns></returns>
        public DataTable GetBasePagerDataTable(string tableName, out int num, int PageSize, int PageIndex, string Fileds, string strWhere, string OrderStr)
        {
            DataTable dt = null;
            IHashObject InputParam = new HashObject();
            IHashObject outputParam = new HashObject();
            if (string.IsNullOrEmpty(OrderStr))
            {
                OrderStr = string.Format(" {0} desc","id");
            }
            else
            {
                if (OrderStr != "" && !OrderStr.Trim().ToLower().StartsWith("order by"))
                {
                    OrderStr = OrderStr.ToLower().Replace("order by", " ");
                }
            }
            InputParam.Add("PageIndex", PageIndex);
            InputParam.Add("pageSize", PageSize);
            InputParam.Add("tableName", tableName);
            InputParam.Add("selFields", string.IsNullOrEmpty(Fileds) ? "*" : Fileds);
            InputParam.Add("SqlWhere", string.IsNullOrEmpty(strWhere) ? "" : strWhere);
            InputParam.Add("SortOrder", OrderStr);
            InputParam.Add("PageTotal", 0);
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteProcedureAndOutputParameter("Proc_Page", out outputParam, InputParam);
                num = outputParam.GetValue<int>("PageTotal");
            }
            return dt;
        }
        /// <summary>
        /// 批量调用多个泛型实例的某个方法 返回调用方法结果集合
        /// </summary>
        /// <param name="tableNameList">泛型实例列表名 也是Model中的类型名</param>
        /// <param name="MothedName">BaseData中的方法名</param>
        /// <param name="Params">输出参数 从里面返回数据 指定参数类型 key为参数索引从1开始 value为引用类型 out ref</param>     
        /// <param name="Params">调用方法的参数</param>
        /// <returns></returns>
        public SortedList<string, Object> PatchCallMethod(string[] tableNameList, string MothedName, HashObject OutParams, params object[] Params)
        {
            SortedList<string, Object> ReObjlist = new SortedList<string, Object>();
            foreach (string item in tableNameList)
            {
                if (!ReObjlist.ContainsKey(item))
                    ReObjlist.Add(item, CallMethod(item, MothedName, OutParams, Params));
            }
            return ReObjlist;
        }
    }
}
