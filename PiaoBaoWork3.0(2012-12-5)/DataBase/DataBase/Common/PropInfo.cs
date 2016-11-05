namespace DataBase.Common
{
    using System;
    using System.Reflection;

    public sealed class PropInfo
    {
        private FieldInfo field;
        private MemberInfo member;
        private object obj;
        private PropertyInfo prop;

        internal PropInfo(object obj, FieldInfo field) : this(obj, (MemberInfo) field)
        {
            this.field = field;
        }

        private PropInfo(object obj, MemberInfo member)
        {
            this.obj = obj;
            this.member = member;
        }

        internal PropInfo(object obj, PropertyInfo prop) : this(obj, (MemberInfo) prop)
        {
            this.prop = prop;
        }

        public string Name
        {
            get
            {
                return this.member.Name;
            }
        }

        public System.Type Type
        {
            get
            {
                if (this.prop == null)
                {
                    return this.field.FieldType;
                }
                return this.prop.PropertyType;
            }
        }

        public object Value
        {
            get
            {
                if (this.prop != null)
                {
                    return this.prop.GetValue(this.obj, null);
                }
                if (this.field == null)
                {
                    throw new InvalidOperationException();
                }
                return this.field.GetValue(this.obj);
            }
            set
            {
                if (this.prop != null)
                {
                    this.prop.SetValue(this.obj, value, null);
                }
                else if (this.field != null)
                {
                    this.field.SetValue(this.obj, value);
                }
            }
        }
    }
}

