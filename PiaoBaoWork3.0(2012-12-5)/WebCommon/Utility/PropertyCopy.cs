using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PbProject.WebCommon.Utility
{
    public class PropertyCopy<D, S>
        where D : class,new()
        where S : class,new()
    {
        /// <summary>
        ///  具有相同属性的类赋值
        /// </summary>
        /// <param name="d">接收赋值的实体 目标实体</param>
        /// <param name="s">获取数据的实体 源实体</param>
        /// <returns></returns>
        public D Cpoy(D d, S s)
        {
            Type st = s.GetType();
            Type dt = d.GetType();
            PropertyInfo[] st_properties = st.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            PropertyInfo[] dt_properties = dt.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);

            foreach (PropertyInfo dp in dt_properties)
            {
                if (dp != null && dp.CanWrite)
                {
                    foreach (PropertyInfo sp in st_properties)
                    {
                        if (sp != null && sp.CanRead)
                        {
                            if (sp.Name == dp.Name && dp.PropertyType == sp.PropertyType)
                            {
                                dp.SetValue(d, Convert.ChangeType(sp.GetValue(s, null), dp.PropertyType), null);
                                break;
                            }
                        }
                    }
                }
            }
            return d;
        }
    }
}
