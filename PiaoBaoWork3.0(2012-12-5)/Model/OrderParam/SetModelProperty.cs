using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model
{
    /// <summary>
    /// 为实体属性添加特性功能描述类 用于修饰实体属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SetModelProperty : Attribute
    {
        private bool m_dbIsExistFiled = false;
        private bool m_IsInsert = false;
        private bool m_IsSelect = false;
        private bool m_IsUpdate = false;
        private bool m_IsDelete = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="IsDBExistFiled">数据库中是否存在该字段 不存在 即生成相应字段的SQL语句</param>
        /// <param name="IsInsert">生成添加SQL语句是否添加该字段</param>
        /// <param name="IsDelete">生成删除SQL语句是否添加该字段</param>
        /// <param name="IsUpdate">生成更新SQL语句是否添加该字段</param>
        /// <param name="IsSelect">生成查询SQL语句是否添加该字段</param>
        public SetModelProperty(bool IsDBExistFiled, bool IsInsert, bool IsDelete, bool IsUpdate, bool IsSelect)
        {
            m_dbIsExistFiled = IsDBExistFiled;
            m_IsInsert = IsInsert;
            m_IsSelect = IsSelect;
            m_IsDelete = IsDelete;
            m_IsUpdate = IsUpdate;
        }

        /// <summary>
        /// 数据库表中书否存在该字段
        /// </summary>
        public bool DBIsExistFiled
        {
            get
            {
                return m_dbIsExistFiled;
            }
            set
            {
                m_dbIsExistFiled = value;
            }
        }
        /// <summary>
        /// 生成Insert添加SQL时是否需要该字段 true需要 false不需要
        /// </summary>
        public bool IsInsert
        {
            get
            {
                return m_IsInsert;
            }
            set
            {
                m_IsInsert = value;
            }
        }
        /// <summary>
        /// 生成查询SQL语句时是否需要该字段 true需要 false不需要
        /// </summary>
        public bool IsSelect
        {
            get
            {
                return m_IsSelect;
            }
            set
            {
                m_IsSelect = value;
            }
        }
        /// <summary>
        /// 生成修改SQL语句时是否需要该字段 true需要 false不需要
        /// </summary>
        public bool IsUpdate
        {
            get
            {
                return m_IsUpdate;
            }
            set
            {
                m_IsUpdate = value;
            }
        }
        /// <summary>
        /// 生成删除SQL语句时是否需要该字段 true需要 false不需要
        /// </summary>
        public bool IsDelete
        {
            get
            {
                return m_IsDelete;
            }
            set
            {
                m_IsDelete = value;
            }
        }
    }
}
