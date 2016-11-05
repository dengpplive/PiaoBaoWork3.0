using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using DataBase;
using PbProject.Dal.Mapping;
using System.Data;
using PbProject.Cache;
using DataBase.Data;
using System.Reflection;
using System.Data.SqlClient;
using DataBase.Unique;
namespace PbProject.Dal.ControlBase
{
    /// <summary>
    /// 基础数据操作类 只适用于单表操作
    /// </summary>
    public class BaseData<T> where T : class,new()
    {
        #region 私有变量
        /// <summary>
        /// 缓存前缀
        /// </summary>
        private string m_CachePix = "cache_";
        /// <summary>
        /// 表中唯一标识列Id名 默认id
        /// </summary>
        public string m_PrimaryFiledKey = "id";
        #endregion

        #region 构造函数
        public BaseData()
        { }
        public BaseData(string CachePix)
            : this()
        {
            m_CachePix = CachePix;
        }
        public BaseData(string CachePix, string PrimaryFiledKey)
            : this(CachePix)
        {
            m_PrimaryFiledKey = PrimaryFiledKey;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 通过Id查询 直接从数据库查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private T GetNotCacheById(string id)
        {
            T model = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                db.AddParameter(string.Format("@{0}", this.m_PrimaryFiledKey), id);
                DataRow dr = db.ExecuteFirstRowSQL(string.Format("select * from {0} where {1} = @{1}", typeof(T).Name, this.m_PrimaryFiledKey));
                model = MappingHelper<T>.FillModel(dr);
            }
            return model;
        }
        /// <summary>
        /// 深复制对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private object Copy(object obj)
        {
            //返回新的对象
            Object targetDeepCopyObj;
            Type targetType = obj.GetType();
            if (targetType.IsValueType)
            {
                //值类型  
                targetDeepCopyObj = obj;
            }
            else
            {
                //引用类型   
                //创建一个实例
                targetDeepCopyObj = System.Activator.CreateInstance(targetType);
                //获取该实例的所有成员
                System.Reflection.MemberInfo[] memberCollection = obj.GetType().GetMembers();
                foreach (System.Reflection.MemberInfo member in memberCollection)
                {
                    //成员类型为字段时
                    if (member.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        System.Reflection.FieldInfo field = (System.Reflection.FieldInfo)member;
                        Object fieldValue = field.GetValue(obj);
                        if (fieldValue is ICloneable)
                        {
                            field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                        }
                        else
                        {
                            field.SetValue(targetDeepCopyObj, Copy(fieldValue));
                        }
                    }
                    //成员类型为属性时
                    else if (member.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        System.Reflection.PropertyInfo myProperty = (System.Reflection.PropertyInfo)member;
                        MethodInfo info = myProperty.GetSetMethod(false);
                        if (info != null)
                        {
                            object propertyValue = myProperty.GetValue(obj, null);
                            if (propertyValue is ICloneable)
                            {
                                myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
                            }
                            else
                            {
                                myProperty.SetValue(targetDeepCopyObj, Copy(propertyValue), null);
                            }
                        }
                    }
                }
            }
            return targetDeepCopyObj;
        }
        #endregion

        #region 刷新缓存

        /// <summary>
        /// 刷新该表缓存
        /// </summary>
        /// <returns></returns>
        //public bool RefreshCache()
        //{
        //    bool result = false;
        //    T[] modelArray = null;
        //    IMemcacheAgent cachePool = MemcacheControl.GetBasicMemcachePool(0);           
        //    using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
        //    {
        //        DataTable dt = db.ExecuteSQL(string.Format("select * from {0}", typeof(T).Name));
        //        modelArray = MappingHelper<T>.FillModelArray(dt);
        //        if (modelArray != null)
        //        {
        //            result = cachePool.SetValToCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0, modelArray);
        //        }
        //        else
        //        {
        //            result = true;
        //        }
        //    }
        //    return result;
        //}
        #endregion

        /// <summary>
        /// 返回Array
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public T[] GetArray(string sqlwhere)
        {
            return GetArray("", sqlwhere);
        }
        /// <summary>
        /// 返回Array
        /// </summary>
        /// <param name="fileds">返回具体字段名数据, 为空表示* </param>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public T[] GetArray(string fileds, string sqlwhere)
        {
            if (string.IsNullOrEmpty(fileds))
            {
                fileds = " * ";
            }
            if (!string.IsNullOrEmpty(sqlwhere))
            {
                sqlwhere = " where " + sqlwhere;
            }
            T[] modelArray = null;
            if (modelArray == null)
            {
                using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
                {
                    DataTable dt = db.ExecuteSQL(string.Format("select {0} from {1} {2}", fileds, typeof(T).Name, sqlwhere));
                    modelArray = MappingHelper<T>.FillModelArray(dt);
                }
            }
            return modelArray;
        }
        /// <summary>
        /// 返回List
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public List<T> GetList(string sqlwhere)
        {
            return GetList("", sqlwhere);
        }
        /// <summary>
        /// 返回List
        /// </summary>
        /// <param name="columnlist">返回具体字段列 为空返回所有</param>
        /// <param name="sqlwhere">筛选条件</param>
        /// <returns></returns>
        public List<T> GetList(string columnlist, string sqlwhere)
        {
            if (string.IsNullOrEmpty(columnlist))
            {
                columnlist = " * ";
            }
            if (!string.IsNullOrEmpty(sqlwhere))
            {
                sqlwhere = " where " + sqlwhere;
            }
            List<T> modelList = new List<T>();
            T[] modelArray = null;
            if (modelArray == null)
            {
                using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
                {
                    DataTable dt = db.ExecuteSQL(string.Format("select {0} from {1} {2}", columnlist, typeof(T).Name, sqlwhere));
                    modelArray = MappingHelper<T>.FillModelList(dt).ToArray();
                }
            }
            if (modelArray != null)
            {
                modelList.AddRange(modelArray);
            }
            return modelList;
        }

        /// <summary>
        /// 是否存在指定条件的数据
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <returns></returns>
        public bool IsExist(string sqlWhere)
        {
            bool IsExist = false;
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sqlWhere = " where " + sqlWhere;
            }
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                int data = db.ExecuteIntSQL(string.Format("select count(*) from {0} {1}", typeof(T).Name, sqlWhere));
                if (data > 0)
                {
                    IsExist = true;
                }
            }
            return IsExist;
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="sqlwhere"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sqlwhere)
        {
            return GetDataTable("", sqlwhere);
        }
        /// <summary>
        /// 返回DataTable 
        /// </summary>
        /// <param name="columnlist">返回指定数据列</param>
        /// <param name="sqlwhere">筛选条件</param>
        /// <returns></returns>
        public DataTable GetDataTable(string columnlist, string sqlwhere)
        {
            if (string.IsNullOrEmpty(columnlist))
            {
                columnlist = " * ";
            }
            if (!string.IsNullOrEmpty(sqlwhere))
            {
                sqlwhere = " where " + sqlwhere;
            }
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteSQL(string.Format("select {0} from {1} {2}", columnlist, typeof(T).Name, sqlwhere));
            }
            return dt;
        }
        /// <summary>
        /// List集合参数运算符 and
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="ListSource">集合来源 处理的数据</param>
        /// <returns></returns>
        public List<T> GetListByAndParam(IHashObject parameter, List<T> ListSource)
        {
            List<T> reList = new List<T>();
            List<T> tempList = ListSource;
            int IsAdd = 0;
            Type type = null;
            PropertyInfo property = null;
            if (parameter != null && parameter.Count > 0)
            {
                foreach (T item in tempList)
                {
                    type = item.GetType();
                    IsAdd = 0;
                    foreach (KeyValuePair<string, object> data in parameter)
                    {
                        property = type.GetProperty(data.Key);
                        if (property != null && property.GetValue(item, null) != null && property.GetValue(item, null).ToString().Trim().Equals(data.Value.ToString().Trim()))
                        {
                            IsAdd++;
                        }
                    }
                    if (IsAdd == parameter.Count)
                    {
                        reList.Add(item);
                    }
                }
            }
            else
            {
                reList = tempList;
            }
            return reList;
        }

        /// <summary>
        ///  List集合参数运算符 Or
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="ListSource">集合来源处理的数据</param>
        /// <returns></returns>
        public List<T> GetListByOrParam(IHashObject parameter, List<T> ListSource)
        {
            List<T> reList = new List<T>();
            List<T> tempList = ListSource;
            bool IsAdd = false;
            Type type = null;
            PropertyInfo property = null;
            if (parameter != null && parameter.Count > 0)
            {
                foreach (T item in tempList)
                {
                    IsAdd = false;
                    type = item.GetType();
                    foreach (KeyValuePair<string, object> data in parameter)
                    {
                        property = type.GetProperty(data.Key);
                        if (property != null && property.GetValue(item, null) != null && property.GetValue(item, null).ToString().Trim().Equals(data.Value.ToString().Trim()))
                        {
                            IsAdd = true;
                            break;
                        }
                    }
                    if (IsAdd)
                    {
                        reList.Add(item);
                    }
                }
            }
            else
            {
                reList = tempList;
            }
            return reList;
        }

        /// <summary>
        /// SQL事务处理
        /// </summary>
        /// <param name="SQLList"></param>
        /// <returns></returns>
        public bool ExecuteSqlTran(List<string> SQLList)
        {
            bool result = false;
            if (SQLList == null || SQLList.Count == 0) return result;
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
                    try
                    {
                        DataBase.LogCommon.Log.Error(string.Join("\r\n ", SQLList.ToArray()), ex);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="outputParam">输入参数</param>
        /// <param name="InputParam">输出参数</param>
        /// <returns></returns>
        public DataTable ExecProcedure(string ProcedureName, IHashObject InputParam, IHashObject outputParam)
        {
            DataTable dt = null;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                dt = db.ExecuteProcedureAndOutputParameter(ProcedureName, out outputParam, InputParam);
            }
            return dt;
        }

        /// <summary>
        /// 执行存储过程 带有返回值
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="outputParam">输入参数</param>
        /// <returns></returns>
        public int ExecProcedureUpdate(string ProcedureName, IHashObject InputParam)
        {
            int result = -1;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.UPDATE, 0)))
            {
                result = db.ExecuteReturnValueProcedure(ProcedureName, InputParam);
            }
            return result;
        }
        /// <summary>
        /// 执行存储过程 操作 返回受影响的函数
        /// </summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="outputParam">输入参数</param>
        /// <returns></returns>
        public int ExecuteNonQueryProcedure(string ProcedureName, IHashObject InputParam)
        {
            int result = -1;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.UPDATE, 0)))
            {
                result = db.ExecuteNonQueryProcedure(ProcedureName, InputParam);
            }
            return result;
        }
        /// <summary>
        /// 通过Id查询 缓存中有直接获取   缓存中没有 数据库中有 需要更新缓存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById(string id)
        {
            T model = null;
            //T[] TList = GetArray();
            //object objGuid = null;
            //foreach (T t in TList)
            //{
            //    objGuid = t.GetType().GetProperty(this.m_PrimaryFiledKey).GetValue(t, null);
            //    if (objGuid != null && objGuid.ToString() == id)
            //    {
            //        model = t;
            //        break;
            //    }
            //}
            //if (model == null)
            //{
            model = GetNotCacheById(id);
            //}
            return model;
        }
        /// <summary>
        ///  添加数据库 并且添加到缓存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Insert(T model)
        {
            IHashObject parameter = ModelToHashObject(model);
            bool InsertSuc = Insert(parameter);
            string id = parameter.GetValue<string>(this.m_PrimaryFiledKey);
            PropertyInfo PI = model.GetType().GetProperty(this.m_PrimaryFiledKey);
            object obj = null;
            if (PI.PropertyType.FullName == "System.Guid")
            {
                obj = Guid.Parse(id);
            }
            else
            {
                obj = Convert.ChangeType(id, PI.PropertyType);
            }
            PI.SetValue(model, obj, null);
            return InsertSuc;
        }

        /// <summary>
        /// 添加数据库 并且添加到缓存
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Insert(IHashObject parameter)
        {
            bool result = false;
            int count = 0;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.WRITE, 0)))
            {
                parameter[this.m_PrimaryFiledKey] = Guid.NewGuid().ToString();
                try
                {
                    count = db.Insert(typeof(T).Name, parameter);
                }
                catch (Exception)
                {
                }
                if (count > 0)
                {
                    result = true;
                    /*
                        string id = parameter.GetValue<string>(this.m_PrimaryFiledKey);
                        T m_model = GetNotCacheById(id);
                        //添加成功 添加到缓存中
                        IMemcacheAgent cachePool = MemcacheControl.GetBasicMemcachePool(0);
                        if (m_model != null)
                        {
                            List<T> list = new List<T>();
                            T[] modelArray = cachePool.GetValFromCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0);
                            if (modelArray != null && modelArray.Length > 0)
                            {
                                list.AddRange(modelArray);
                            }
                            list.Add(m_model);
                            result = cachePool.SetValToCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0, list.ToArray());
                        }
                        //刷新缓存
                        //result = RefreshCache();
                     * */
                }
            }
            return result;
        }


        /// <summary>
        /// 添加数据库 不缓存数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        //public bool NotCacheInsert(IHashObject parameter)
        //{
        //    bool result = false;
        //    using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.WRITE, 0)))
        //    {
        //        parameter[this.m_PrimaryFiledKey] = Guid.NewGuid().ToString();
        //        int count = db.Insert(typeof(T).Name, parameter);
        //        if (count > 0)
        //        {
        //            result = true;
        //        }
        //    }
        //    return result;
        //}
        /// <summary>
        /// 添加数据库 不缓存数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public bool NotCacheInsert(T model)
        //{
        //    IHashObject parameter = ModelToHashObject(model);
        //    return NotCacheInsert(parameter);
        //}

        /// <summary>
        /// 将实体转换为IHashObject
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHashObject ModelToHashObject(T model)
        {
            IHashObject param = new HashObject();
            Type t = model.GetType();
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (PropertyInfo p in properties)
            {
                if (p != null)
                {
                    object[] customAttributes = p.GetCustomAttributes(typeof(SetModelProperty), false);
                    if (customAttributes != null && customAttributes.Length > 0)
                    {
                        SetModelProperty Pty = customAttributes[0] as SetModelProperty;
                        //数据库中不存在的字段不添加
                        if (!Pty.DBIsExistFiled)
                        {
                            continue;
                        }
                    }
                }
                param.Add(p.Name, p.GetValue(model, null));
            }
            return param;
        }
        /// <summary>
        /// 字符串显示该实体所有数据值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="strSplit"></param>
        /// <returns></returns>
        public string ModelToString(T model, string strSplit)
        {
            string strReData = string.Empty;
            if (model != null)
            {
                List<string> listData = new List<string>();
                Type t = model.GetType();
                PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                object obj = null;
                foreach (PropertyInfo p in properties)
                {
                    obj = p.GetValue(model, null);
                    listData.Add(p.Name + "=" + (obj == null ? "null" : obj));
                }
                if (listData.Count > 0)
                {
                    strReData = string.Join(strSplit, listData.ToArray());
                }
            }
            return strReData;
        }
        /// <summary>
        /// 指定规则的字符串转换到对应的实体
        /// </summary>
        /// <param name="strSplit"></param>
        /// <returns></returns>
        public T StringToModel(string Data, string strSplit)
        {
            string[] strArr = Data.Split(new string[] { strSplit }, StringSplitOptions.None);
            Type targetType = typeof(T);
            T Model = System.Activator.CreateInstance(targetType) as T;
            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            string[] strArr1 = null;
            foreach (PropertyInfo p in properties)
            {
                foreach (string item in strArr)
                {
                    strArr1 = item.Split('=');
                    if (strArr1.Length == 2 && strArr1[0] == p.Name)
                    {
                        p.SetValue(Model, Convert.ChangeType(strArr1[0], p.PropertyType), null);
                        break;
                    }
                }
            }
            return Model;
        }
        /// <summary>
        /// 显示两个实例中数据不相等的字符串显示值
        /// </summary>
        /// <param name="oldModel">原实例</param>
        /// <param name="newModel">修改后的实例</param>
        /// <param name="strSplit">数据之间的分隔符</param>
        /// <returns></returns>
        public string ChangeDataToString(T oldModel, T newModel, string strSplit)
        {
            string strReData = string.Empty;
            if (oldModel != null && newModel != null)
            {
                List<string> listData = new List<string>();
                Type oldT = oldModel.GetType();
                Type newT = newModel.GetType();
                PropertyInfo[] oldTProperties = oldT.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                PropertyInfo[] newTProperties = newT.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                if (oldTProperties.Length == newTProperties.Length)
                {
                    object oldObj = null;
                    object newObj = null;
                    foreach (PropertyInfo p in oldTProperties)
                    {
                        oldObj = p.GetValue(oldModel, null);
                        newObj = newT.GetProperty(p.Name).GetValue(newModel, null);
                        if (oldObj != newObj)
                        {
                            listData.Add(string.Format("{0}:{1}=>{2}", p.Name, oldObj == null ? "null" : oldObj, newObj == null ? "null" : newObj));
                        }
                    }
                    if (listData.Count > 0)
                    {
                        strReData = string.Join(strSplit, listData.ToArray());
                    }
                }
            }
            return strReData;
        }

        /// <summary>
        /// 复制一个实例
        /// </summary>
        /// <param name="oldModel"></param>
        /// <returns></returns>
        public T CopyModel(T oldModel)
        {
            if (oldModel == null) return null;
            return Copy(oldModel) as T;
        }



        /// <summary>
        /// 整个实体 更新数据库 并且更新缓存
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Update(T model)
        {
            IHashObject parameter = ModelToHashObject(model);
            return Update(parameter);
        }

        /// <summary>
        /// 对指定参数进行更新
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Update(IHashObject parameter)
        {
            bool result = false;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.UPDATE, 0)))
            {
                int count = db.Update(typeof(T).Name, this.m_PrimaryFiledKey, parameter);
                if (count > 0)
                {
                    /*
                        //更新缓存
                        string id = parameter.GetValue<object>(this.m_PrimaryFiledKey).ToString();
                        T m_model = GetNotCacheById(id);
                        IMemcacheAgent cachePool = MemcacheControl.GetBasicMemcachePool(0);
                        List<T> list = new List<T>();
                        T[] modelArray = cachePool.GetValFromCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0);
                        if (m_model != null)
                        {
                            if (modelArray != null && modelArray.Length > 0)
                            {
                                list = modelArray.ToList<T>();
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].GetType().GetProperty(this.m_PrimaryFiledKey).GetValue(list[i], null).ToString().Trim() == id.Trim())
                                    {
                                        //移除
                                        list.Remove(list[i]);
                                        break;
                                    }
                                }
                            }
                            //添加
                            list.Add(m_model);
                            result = cachePool.SetValToCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0, list.ToArray());
                        }
                        //刷新缓存
                        //result = RefreshCache();
                     * */
                    result = true;
                }
                return result;
            }
        }


        /// <summary>
        /// 对指定参数进行更新 不操作缓存数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public bool NotCacheUpdate(T model)
        //{
        //    IHashObject parameter = ModelToHashObject(model);
        //    return NotCacheUpdate(parameter);
        //}

        /// <summary>
        /// 对指定参数进行更新 不操作缓存数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        //public bool NotCacheUpdate(IHashObject parameter)
        //{
        //    bool result = false;
        //    using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.UPDATE, 0)))
        //    {
        //        int count = db.Update(typeof(T).Name, this.m_PrimaryFiledKey, parameter);
        //        if (count > 0)
        //        {
        //            result = true;
        //        }
        //        return result;
        //    }
        //}

        /// <summary>
        /// 删除数据并且清除缓存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(string id)
        {
            bool result = false;
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.DELETE, 0)))
            {
                //删除数据库中的数据
                int count = db.Delete(typeof(T).Name, this.m_PrimaryFiledKey, id);
                if (count > 0)
                {
                    result = true;
                    /*
                        //清除缓存中的数据
                        IMemcacheAgent cachePool = MemcacheControl.GetBasicMemcachePool(0);
                        List<T> list = new List<T>();
                        T[] modelArray = cachePool.GetValFromCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0);
                        if (modelArray != null && modelArray.Length > 0)
                        {
                            list = modelArray.ToList<T>();
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i].GetType().GetProperty(this.m_PrimaryFiledKey).GetValue(list[i], null).ToString().Trim() == id.Trim())
                                {
                                    //移除
                                    list.Remove(list[i]);
                                    break;
                                }
                            }
                        }
                        result = cachePool.SetValToCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0, list.ToArray());
                        //刷新缓存
                        //result = RefreshCache();
                     * */
                }
                return result;
            }
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="UpdateFileds">更新字段字符串 如:"A='a',B=1"等</param>
        /// <returns></returns>
        public bool UpdateByIds(List<string> Ids, string UpdateFileds)
        {
            bool result = false;
            if (Ids == null || Ids.Count == 0 || string.IsNullOrEmpty(UpdateFileds))
            {
                return result;
            }
            List<string> lstList = new List<string>();
            for (int i = 0; i < Ids.Count; i++)
            {
                if (!Ids[i].ToString().Trim().StartsWith("'") && !Ids[i].ToString().Trim().EndsWith("'"))
                {
                    lstList.Add("'" + Ids[i].ToString().Trim(new char[] { '\'' }) + "'");
                }
                else
                {
                    lstList.Add(Ids[i].ToString());
                }
            }
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.DELETE, 0)))
            {
                string selectSQL = string.Format(" update  {0} set {1} where {2} in({3})", typeof(T).Name, UpdateFileds, this.m_PrimaryFiledKey, string.Join(",", lstList.ToArray()));
                int UpdateRow = db.ExecuteNonQuerySQL(selectSQL);
                if (UpdateRow > 0)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据ID数据删除 并清除缓存 输出删除的实体列表
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="delList"></param>
        /// <returns></returns>
        public bool DeleteByIds(List<string> Ids, out List<T> delList)
        {
            bool result = false;
            delList = new List<T>();
            List<string> lstList = new List<string>();
            if (Ids == null || Ids.Count == 0)
            {
                return result;
            }
            for (int i = 0; i < Ids.Count; i++)
            {
                if (!Ids[i].ToString().Trim().StartsWith("'") && !Ids[i].ToString().Trim().EndsWith("'"))
                {
                    lstList.Add("'" + Ids[i].ToString().Trim(new char[] { '\'' }) + "'");
                }
                else
                {
                    lstList.Add(Ids[i].ToString());
                }
            }

            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.DELETE, 0)))
            {
                string selectSQL = string.Format(" select * from {0} where {1} in({2})", typeof(T).Name, this.m_PrimaryFiledKey, string.Join(",", lstList.ToArray()));
                DataTable dt = db.ExecuteSQL(selectSQL);
                delList = MappingHelper<T>.FillModelList(dt).ToList();
                //删除数据库中的数据
                int count = db.ExecuteNonQuerySQL(string.Format(" delete from {0} where {1} in({2})", typeof(T).Name, this.m_PrimaryFiledKey, string.Join(",", lstList.ToArray())));
                lstList.Clear();
                if (count > 0)
                {
                    /*
                        //清除缓存中的数据
                        IMemcacheAgent cachePool = MemcacheControl.GetBasicMemcachePool(0);
                        List<T> list = new List<T>();
                        T[] modelArray = cachePool.GetValFromCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0);
                        if (modelArray != null && modelArray.Length > 0)
                        {
                            list = modelArray.ToList<T>();
                            foreach (string id in Ids)
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].GetType().GetProperty(this.m_PrimaryFiledKey).GetValue(list[i], null).ToString().Trim() == id.Trim(new char[] { '\'' }))
                                    {
                                        delList.Add(CopyModel(list[i]));
                                        //移除
                                        list.Remove(list[i]);
                                        break;
                                    }
                                }
                            }
                        }
                        result = cachePool.SetValToCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0, list.ToArray());
                        //刷新缓存
                        //result = RefreshCache();
                     * */
                    result = true;
                }
                return result;
            }
        }

        /// <summary>
        /// 根据SQL语句删除 并清除缓存
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public bool DeleteBySQL(string SQL)
        {
            bool result = false;


            DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.DELETE, 0));

            //查询数据集
            //string selectSQL = string.Format(" select * from {0} where {1} ", typeof(T).Name, SQL);
            //DataTable dt = db.ExecuteSQL(selectSQL);


            //删除数据库中的数据
            int count = db.ExecuteNonQuerySQL(string.Format(" delete from {0} where {1} ", typeof(T).Name, SQL));
            if (count > 0)
            {
                /*
                    //清除缓存中的数据
                    IMemcacheAgent cachePool = MemcacheControl.GetBasicMemcachePool(0);
                    List<T> list = new List<T>();
                    T[] modelArray = cachePool.GetValFromCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0);
                    if (modelArray != null && modelArray.Length > 0)
                    {
                        list = modelArray.ToList<T>();
                        foreach (string id in Ids)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i].GetType().GetProperty(this.m_PrimaryFiledKey).GetValue(list[i], null).ToString().Trim() == id.Trim(new char[] { '\'' }))
                                {
                                    delList.Add(CopyModel(list[i]));
                                    //移除
                                    list.Remove(list[i]);
                                    break;
                                }
                            }
                        }
                    }
                    result = cachePool.SetValToCache<T[]>(m_CachePix + typeof(T).Name.ToLower(), 0, list.ToArray());
                    //刷新缓存
                    //result = RefreshCache();
                    * */
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 基础数据普通分页 
        /// </summary>
        /// <param name="TotalCount">输出查询条件查出来的总记录数</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="PageIndex">当前页</param>      
        /// <param name="Fileds">筛选字段 默认*表示所有字段</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="OrderStr">排序字符串 如[a desc]</param>       
        /// <returns></returns>
        public List<T> GetBasePager(out int TotalCount, int PageSize, int PageIndex, string Fileds, string strWhere, string OrderStr)
        {
            List<T> list = null;
            TotalCount = 0;
            IHashObject InputParam = new HashObject();
            IHashObject outputParam = new HashObject();
            if (string.IsNullOrEmpty(OrderStr))
            {
                OrderStr = string.Format(" {0} desc", this.m_PrimaryFiledKey);
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
            InputParam.Add("tableName", typeof(T).Name);
            InputParam.Add("selFields", string.IsNullOrEmpty(Fileds) ? "*" : Fileds);
            InputParam.Add("SqlWhere", string.IsNullOrEmpty(strWhere) ? "" : strWhere);
            InputParam.Add("SortOrder", OrderStr);
            InputParam.Add("PageTotal", 0);
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                DataTable dt = db.ExecuteProcedureAndOutputParameter("Proc_Page", out outputParam, InputParam);
                TotalCount = outputParam.GetValue<int>("PageTotal");
                list = MappingHelper<T>.FillModelList(dt);
            }
            return list;
        }

        /// <summary>
        /// 高性能基础数据分页 
        /// </summary>
        /// <param name="TotalCount">输出查询条件查出来的总记录数</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="PageIndex">当前页</param>      
        /// <param name="Fileds">筛选字段 默认*表示所有字段</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="OrderStr">排序字符串 如[a desc]</param>       
        /// <returns></returns>
        public List<T> GetBasePager1(out int TotalCount, int PageSize, int PageIndex, string Fileds, string strWhere, string OrderStr)
        {
            List<T> list = null;
            TotalCount = 0;
            IHashObject InputParam = new HashObject();
            IHashObject outputParam = new HashObject();
            if (string.IsNullOrEmpty(OrderStr))
            {
                OrderStr = string.Format(" {0} desc", this.m_PrimaryFiledKey);
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
            InputParam.Add("tableName", typeof(T).Name);
            InputParam.Add("selFields", string.IsNullOrEmpty(Fileds) ? "*" : Fileds);
            InputParam.Add("SqlWhere", string.IsNullOrEmpty(strWhere) ? "" : strWhere);
            InputParam.Add("SortOrder", OrderStr);
            InputParam.Add("PrimaryKey", this.m_PrimaryFiledKey);
            InputParam.Add("GroupBy", "");
            InputParam.Add("PageTotal", 0);
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.SELECT, 0)))
            {
                DataTable dt = db.ExecuteProcedureAndOutputParameter("Proc_NewPage", out outputParam, InputParam);
                TotalCount = outputParam.GetValue<int>("PageTotal");
                list = MappingHelper<T>.FillModelList(dt);
            }
            return list;
        }
    }
}

