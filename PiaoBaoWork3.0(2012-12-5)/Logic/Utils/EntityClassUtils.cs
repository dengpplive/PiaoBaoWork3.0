using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PbProject.ConsoleServerProc.Utils
{
    public class EntityClassUtils<T> where T : class, new()
    {
        /// <summary>
        /// 获取实体类属性名称集合
        /// </summary>
        /// <param name="tObj"></param>
        /// <returns></returns>
        public static List<string> GetField(T tObj)
        {
            List<string> result = new List<string>();
            PropertyInfo[] properites = typeof(T).GetProperties();//得到实体类属性的集合
            foreach (PropertyInfo propertyInfo in properites)//遍历数组
            {
                result.Add(propertyInfo.Name);
            }
            return result;
        }

        /// <summary>
        /// 获取实体类属性值集合
        /// </summary>
        /// <param name="tObj"></param>
        /// <returns></returns>
        public static List<string> GetValue(T tObj)
        {
            List<string> result = new List<string>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                result.Add(propertyInfo.GetValue(tObj, null) == null ? "" : propertyInfo.GetValue(tObj, null).ToString());
            }
            return result;
        }
    }
}
