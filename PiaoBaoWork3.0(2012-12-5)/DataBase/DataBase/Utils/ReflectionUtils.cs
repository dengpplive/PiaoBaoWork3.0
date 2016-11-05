namespace DataBase.Utils
{
    using DataBase.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public static class ReflectionUtils
    {
        private static bool DoGetIsPrimitiveType(Type type)
        {
            if ((!type.IsPrimitive && !(type == typeof(string))) && !(type == typeof(DateTime)))
            {
                return (type == typeof(decimal));
            }
            return true;
        }

        public static PropInfo FindProp(object obj, MemberInfo member)
        {
            if (member is PropertyInfo)
            {
                return new PropInfo(obj, (PropertyInfo) member);
            }
            if (member is FieldInfo)
            {
                return new PropInfo(obj, (FieldInfo) member);
            }
            return null;
        }

        public static PropInfo FindProp(object obj, string name)
        {
            if (obj != null)
            {
                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
                Type type = obj.GetType();
                PropertyInfo property = type.GetProperty(name, bindingAttr);
                if (property != null)
                {
                    return new PropInfo(obj, property);
                }
                FieldInfo field = type.GetField(name, bindingAttr);
                if (field != null)
                {
                    return new PropInfo(obj, field);
                }
            }
            return null;
        }

        public static Type GetArrayElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (typeof(ICollection).IsAssignableFrom(type))
            {
                return GetCollectionElementType(type);
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return GetEnumeratorElementType(type);
            }
            return null;
        }

        private static Type GetCollectionElementType(Type type)
        {
            PropertyInfo defaultIndexer = GetDefaultIndexer(type);
            if (defaultIndexer == null)
            {
                return null;
            }
            return defaultIndexer.PropertyType;
        }

        private static PropertyInfo GetDefaultIndexer(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return null;
            }
            MemberInfo[] defaultMembers = type.GetDefaultMembers();
            PropertyInfo info = null;
            if ((defaultMembers != null) && (defaultMembers.Length > 0))
            {
                for (Type type2 = type; type2 != null; type2 = type2.BaseType)
                {
                    for (int i = 0; i < defaultMembers.Length; i++)
                    {
                        if (defaultMembers[i] is PropertyInfo)
                        {
                            PropertyInfo info2 = (PropertyInfo) defaultMembers[i];
                            if ((info2.DeclaringType == type2) && info2.CanRead)
                            {
                                ParameterInfo[] parameters = info2.GetGetMethod().GetParameters();
                                if ((parameters.Length == 1) && (parameters[0].ParameterType == typeof(int)))
                                {
                                    info = info2;
                                    break;
                                }
                            }
                        }
                    }
                    if (info != null)
                    {
                        break;
                    }
                }
            }
            if (info == null)
            {
                throw new InvalidOperationException(string.Format("You must implement a default accessor on {0} because it inherits from ICollection.", type.FullName));
            }
            return info;
        }

        private static Type GetEnumeratorElementType(Type type)
        {
            if (type.IsGenericType && (type.GetGenericArguments().Length == 1))
            {
                return type.GetGenericArguments()[0];
            }
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }
            MethodInfo method = type.GetMethod("GetEnumerator", new Type[0]);
            if ((method == null) || !typeof(IEnumerator).IsAssignableFrom(method.ReturnType))
            {
                method = null;
                foreach (MemberInfo info2 in type.GetMember("System.Collections.Generic.IEnumerable<*", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    method = info2 as MethodInfo;
                    if ((method != null) && typeof(IEnumerator).IsAssignableFrom(method.ReturnType))
                    {
                        break;
                    }
                    method = null;
                }
                if (method == null)
                {
                    method = type.GetMethod("System.Collections.IEnumerable.GetEnumerator", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
                }
            }
            if ((method == null) || !typeof(IEnumerator).IsAssignableFrom(method.ReturnType))
            {
                return null;
            }
            PropertyInfo property = method.ReturnType.GetProperty("Current");
            Type type2 = (property == null) ? typeof(object) : property.PropertyType;
            if ((type.GetMethod("Add", new Type[] { type2 }) == null) && (type2 != typeof(object)))
            {
                type2 = typeof(object);
            }
            return type2;
        }

        public static string GetNoPropMessage(object obj, string name)
        {
            return GetNoPropMessage(obj.GetType(), name);
        }

        public static string GetNoPropMessage(Type type, string name)
        {
            return string.Format("“{0}.{1}”属性不存在。", type.Name, name);
        }

        public static PropInfo GetProp(object obj, string name)
        {
            PropInfo info = FindProp(obj, name);
            if (info == null)
            {
                throw new Exception(GetNoPropMessage(obj, name));
            }
            return info;
        }

        public static PropInfo[] GetProps(object obj)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            Type type = obj.GetType();
            List<PropInfo> list = new List<PropInfo>();
            foreach (PropertyInfo info in type.GetProperties(bindingAttr))
            {
                list.Add(new PropInfo(obj, info));
            }
            foreach (FieldInfo info2 in type.GetFields(bindingAttr))
            {
                list.Add(new PropInfo(obj, info2));
            }
            PropInfo[] array = new PropInfo[list.Count];
            list.CopyTo(array);
            return array;
        }

        public static object GetPropValue(object obj, string name)
        {
            return GetProp(obj, name).Value;
        }

        public static bool IsPrimitiveType(Type type)
        {
            return DoGetIsPrimitiveType(type);
        }

        internal static bool IsPrimitiveType(Type type, out Type underlyingType)
        {
            underlyingType = type;
            if (DoGetIsPrimitiveType(type))
            {
                return true;
            }
            if (type.IsValueType && type.IsGenericType)
            {
                underlyingType = Nullable.GetUnderlyingType(type);
                return DoGetIsPrimitiveType(underlyingType);
            }
            return false;
        }

        public static void SetPropValue(object obj, string name, object value)
        {
            GetProp(obj, name).Value = value;
        }
    }
}

