using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using DataBase;
using PbProject.Model;
namespace PbProject.Dal.Mapping
{
    public class MappingHelper<T> where T : class,new()
    {
        #region 实体映射
        /// <summary>
        /// DataRow映射到实体
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T FillModel(DataRow dr)
        {
            if (dr == null)
            {
                return default(T);
            }

            T model = new T();

            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                if (propertyInfo != null && dr[i] != DBNull.Value)
                {
                    object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(SetModelProperty), false);
                    if (customAttributes != null && customAttributes.Length > 0)
                    {
                        SetModelProperty Pty = customAttributes[0] as SetModelProperty;
                        //是否用于查询
                        if (!Pty.IsSelect)
                        {
                            continue;
                        }
                    }
                    propertyInfo.SetValue(model, dr[i], null);
                }
            }
            return model;
        }

        /// <summary>
        /// DataTable映射到实体数组
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T[] FillModelArray(DataTable dt)
        {
            T[] result = null;
            List<T> modelList = FillModelList(dt);
            if (modelList != null && modelList.Count > 0)
            {
                result = new T[modelList.Count];
                modelList.CopyTo(result);
            }
            return result;
        }

        /// <summary>
        /// DataTable映射到实体
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> FillModelList(DataTable dt)
        {
            List<T> modelList = new List<T>();

            if (dt == null || dt.Rows.Count == 0)
            {
                return modelList;
            }

            modelList = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T model = FillModel(dr);
                if (model != null)
                    modelList.Add(model);
            }

            return modelList;
        }


        #endregion

        #region 创建Sql
        /// <summary>
        /// 清除SQL中的特殊字符
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static string ClearSQLKeyChar(object Data)
        {
            string TempData = "";
            if (Data != null)
            {
                TempData = Data.ToString();
            }
            List<string> list = new List<string>();
            list.Add("'");//添加关键字或者特殊字符
            for (int i = 0; i < list.Count; i++)
            {
                TempData = TempData.Replace(list[i], "");
            }
            return TempData.Trim();
        }

        /// <summary>
        /// 根据数据类型获取值
        /// </summary>
        /// <returns></returns>
        public static object GetValueByValType(T model, PropertyInfo property)
        {
            object result = "";
            if (property.PropertyType == typeof(DateTime))
            {
                DateTime dt = (DateTime)property.GetValue(model, null);
                result = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            else
            {
                result = property.GetValue(model, null);
            }
            return result;
        }

        /// <summary>
        /// 创建实体数据插入数据库的sql语句
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        ///[Obsolete("不再允许使用此方法", true)]
        public static string CreateInsertModelSql(T model)
        {
            Type t;
            PropertyInfo[] properties;
            CheckModel(model, out t, out properties);
            string modelName = t.Name;
            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(modelName);
            sql.Append(" (");

            StringBuilder valueSql = new StringBuilder(" values(");
            foreach (PropertyInfo property in properties)
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(SetModelProperty), false);
                if (customAttributes != null && customAttributes.Length > 0)
                {
                    SetModelProperty Pty = customAttributes[0] as SetModelProperty;
                    //数据库中不存在或者设置不添加该字段
                    if (!Pty.DBIsExistFiled || !Pty.IsInsert)
                    {
                        continue;
                    }
                }

                object value = property.GetValue(model, null);
                if (value == null
                        || (property.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase)
                            &&
                            (value.ToString() == "0" || value.ToString() == "")
                        )
                    )
                    continue;

                sql.Append(property.Name);
                sql.Append(",");

                valueSql.Append("'");
                valueSql.Append(ClearSQLKeyChar(GetValueByValType(model, property)));
                valueSql.Append("'");
                valueSql.Append(",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");

            valueSql.Remove(valueSql.Length - 1, 1);
            valueSql.Append(")");

            sql.Append(valueSql);

            return sql.ToString();
        }

        /// <summary>
        /// 创建实体数据插入数据库的sql语句 指定字段名不用添加数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        ///[Obsolete("不再允许使用此方法", true)]
        public static string CreateInsertModelSql(T model, List<string> RemoveFiledsList)
        {
            List<string> tempList = new List<string>();
            if (RemoveFiledsList != null && RemoveFiledsList.Count > 0)
            {
                foreach (string item in RemoveFiledsList)
                {
                    tempList.Add(item.ToLower());
                }
            }

            Type t;
            PropertyInfo[] properties;
            CheckModel(model, out t, out properties);
            string modelName = t.Name;
            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(modelName);
            sql.Append(" (");

            StringBuilder valueSql = new StringBuilder(" values(");
            foreach (PropertyInfo property in properties)
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(SetModelProperty), false);
                if (customAttributes != null && customAttributes.Length > 0)
                {
                    SetModelProperty Pty = customAttributes[0] as SetModelProperty;
                    //数据库中不存在或者设置不添加该字段
                    if (!Pty.DBIsExistFiled || !Pty.IsInsert)
                    {
                        continue;
                    }
                }

                object value = property.GetValue(model, null);
                if (value == null
                        || (property.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase)
                            &&
                            (value.ToString() == "0" || value.ToString() == "")
                        )
                    )
                    continue;
                if (!tempList.Contains(property.Name.ToLower()))
                {
                    sql.Append(property.Name);
                    sql.Append(",");

                    valueSql.Append("'");
                    valueSql.Append(ClearSQLKeyChar(GetValueByValType(model, property)));
                    valueSql.Append("'");
                    valueSql.Append(",");
                }
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");

            valueSql.Remove(valueSql.Length - 1, 1);
            valueSql.Append(")");

            sql.Append(valueSql);

            return sql.ToString();
        }

        /// <summary>
        /// 替换指定字段名数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="FiledValue"></param>
        /// <returns></returns>
        public static string CreateInsertModelSql(T model, SortedList<string, string> FiledValue)
        {
            SortedList<string, string> TempFiledValue = new SortedList<string, string>();
            foreach (KeyValuePair<string, string> item in FiledValue)
            {
                if (!TempFiledValue.ContainsKey(item.Key.ToLower()))
                {
                    TempFiledValue.Add(item.Key.ToLower(), item.Value);
                }
            }
            Type t;
            PropertyInfo[] properties;
            CheckModel(model, out t, out properties);
            string modelName = t.Name;
            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(modelName);
            sql.Append(" (");

            StringBuilder valueSql = new StringBuilder(" values(");
            foreach (PropertyInfo property in properties)
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(SetModelProperty), false);
                if (customAttributes != null && customAttributes.Length > 0)
                {
                    SetModelProperty Pty = customAttributes[0] as SetModelProperty;
                    //数据库中不存在或者设置不添加该字段
                    if (!Pty.DBIsExistFiled || !Pty.IsInsert)
                    {
                        continue;
                    }
                }

                object value = property.GetValue(model, null);
                if (value == null
                        || (property.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase)
                            &&
                            (value.ToString() == "0" || value.ToString() == "")
                        )
                    )
                    continue;

                if (TempFiledValue.Count > 0 & TempFiledValue.ContainsKey(property.Name.ToLower()))
                {
                    sql.Append(property.Name);
                    sql.Append(",");
                    valueSql.Append(TempFiledValue[property.Name.ToLower()]);
                    valueSql.Append(",");
                }
                else
                {
                    sql.Append(property.Name);
                    sql.Append(",");

                    valueSql.Append("'");
                    valueSql.Append(ClearSQLKeyChar(GetValueByValType(model, property)));
                    valueSql.Append("'");
                    valueSql.Append(",");
                }

            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");

            valueSql.Remove(valueSql.Length - 1, 1);
            valueSql.Append(")");

            sql.Append(valueSql);

            return sql.ToString();
        }

       
        /// <summary>
        /// 创建实体数据更新数据库的sql语句
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="fieldNames">指定条件字段名</param>
        /// <returns></returns>
        ///[Obsolete("不再允许使用此方法", true)]
        public static string CreateUpdateModelSql(T model, params string[] fieldNames)
        {
            Type t;
            PropertyInfo[] properties;
            CheckModel(model, out t, out properties);
            string modelName = t.Name;
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(modelName);
            sql.Append(" SET ");

            StringBuilder whereSql = new StringBuilder();
            foreach (PropertyInfo property in properties)
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(SetModelProperty), false);
                if (customAttributes != null && customAttributes.Length > 0)
                {
                    SetModelProperty Pty = customAttributes[0] as SetModelProperty;
                    //数据库中不存在或者设置不修改该字段
                    if (!Pty.DBIsExistFiled || !Pty.IsUpdate)
                    {
                        continue;
                    }
                }

                object value = property.GetValue(model, null);
                if (GetWhereSql(whereSql, fieldNames, property, value))
                    continue;

                if (value == null)
                    continue;

                sql.Append(property.Name);
                sql.Append("=");

                sql.Append("'");
                sql.Append(ClearSQLKeyChar(value));
                sql.Append("'");
                sql.Append(",");
            }
            sql.Remove(sql.Length - 1, 1);

            if (whereSql.Length > 0)
                whereSql.Remove(whereSql.Length - 5, 5);

            sql.Append(whereSql);

            return sql.ToString();
        }
       
       
        /// <summary>
        /// 创建实体数据更新数据库的sql语句 指定更新那些字段
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="UpdateFileds">需要修改的字段名</param>
        /// <param name="fieldNames">指定条件字段名</param>
        /// <returns></returns>        
        public static string CreateUpdateModelSql(T model, List<string> UpdateFileds, params string[] fieldNames)
        {
            List<string> tempList = new List<string>();
            foreach (string item in UpdateFileds)
            {
                tempList.Add(item.ToLower().Trim());
            }
            Type t;
            PropertyInfo[] properties;
            CheckModel(model, out t, out properties);
            string modelName = t.Name;
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(modelName);
            sql.Append(" SET ");

            StringBuilder whereSql = new StringBuilder();
            foreach (PropertyInfo property in properties)
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(SetModelProperty), false);
                if (customAttributes != null && customAttributes.Length > 0)
                {
                    SetModelProperty Pty = customAttributes[0] as SetModelProperty;
                    //数据库中不存在或者设置不修改该字段
                    if (!Pty.DBIsExistFiled || !Pty.IsUpdate)
                    {
                        continue;
                    }
                }

                object value = property.GetValue(model, null);
                if (GetWhereSql(whereSql, fieldNames, property, value))
                    continue;

                if (value == null)
                    continue;
                if (tempList != null && tempList.Count > 0 && tempList.Contains(property.Name.ToLower()))
                {
                    sql.Append(property.Name);
                    sql.Append("=");

                    sql.Append("'");
                    sql.Append(ClearSQLKeyChar(value));
                    sql.Append("'");
                    sql.Append(",");
                }
                else
                {
                    sql.Append(property.Name);
                    sql.Append("=");

                    sql.Append("'");
                    sql.Append(ClearSQLKeyChar(value));
                    sql.Append("'");
                    sql.Append(",");
                }
            }
            sql.Remove(sql.Length - 1, 1);

            if (whereSql.Length > 0)
                whereSql.Remove(whereSql.Length - 5, 5);

            sql.Append(whereSql);

            return sql.ToString();
        }
      
       /// <summary>
       /// 创建实体数据删除数据库的sql语句
       /// </summary>
       /// <param name="model">实体类</param>
       /// <param name="fieldNames">指定条件字段名</param>
       /// <returns></returns>
       ///[Obsolete("不再允许使用此方法", true)]
       public static string CreateDeleteModelSql(T model, params string[] fieldNames)
       {
           Type t;
           PropertyInfo[] properties;
           CheckModel(model, out t, out properties);
           string modelName = t.Name;
           StringBuilder sql = new StringBuilder("DELETE FROM ");
           sql.Append(modelName);

           StringBuilder whereSql = new StringBuilder();
           foreach (PropertyInfo property in properties)
           {
               object[] customAttributes = property.GetCustomAttributes(typeof(SetModelProperty), false);
               if (customAttributes != null && customAttributes.Length > 0)
               {
                   SetModelProperty Pty = customAttributes[0] as SetModelProperty;
                   //数据库中不存在或者设置不通过该字段座位条件删除
                   if (!Pty.DBIsExistFiled || !Pty.IsDelete)
                   {
                       continue;
                   }
               }

               object value = property.GetValue(model, null);
               if (GetWhereSql(whereSql, fieldNames, property, value))
                   continue;
           }

           if (whereSql.Length > 0)
               whereSql.Remove(whereSql.Length - 5, 5);

           sql.Append(whereSql);

           return sql.ToString();

       }
       
        //检查实体类数据是否正常
        private static void CheckModel(T model, out Type t, out PropertyInfo[] properties)
        {
            if (model == null)
            {
                throw new Exception("实体类不能为空");
            }
            t = model.GetType();
            properties = t.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            if (properties.Length == 0)
            {
                throw new Exception("实体类无属性字段");
            }
        }

        //获取实体条件语句(where语句)
        private static bool GetWhereSql(StringBuilder whereSql, string[] fieldNames, PropertyInfo property, object value)
        {
            foreach (string fieldName in fieldNames)
            {
                if (property.Name.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (value == null || value.ToString() == "")
                        throw new Exception("条件值不能为空");
                    if (whereSql.Length == 0)
                        whereSql.Append(" WHERE ");
                    whereSql.Append(fieldName);
                    whereSql.Append(" = ");
                    whereSql.Append("'");
                    whereSql.Append(ClearSQLKeyChar(value));
                    whereSql.Append("'");
                    whereSql.Append(" AND ");
                    return true;
                }
            }
            return false;
        }
        #endregion

      
        /// <summary>
        /// 通过Sql 执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> GetListBySql(string sql)
        {
            List<T> list = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                DataTable dt = db.ExecuteSQL(sql);
                list = MappingHelper<T>.FillModelList(dt);
            }
            return list;
        }
    }

    /// <summary>
    /// 用于创建数据库连接的字符串
    /// </summary>
    public class MappingSetting
    {
        /// <summary>
        /// 用于创建数据库连接的字符串
        /// </summary>
        public static string ConnectionString { get; set; }
    }
}
