using System;
namespace PbProject.Model
{
    /// <summary>
    /// Bd_Base_Dictionary:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Bd_Base_Dictionary
    {
        public Bd_Base_Dictionary()
        { }
        #region Model
        private Guid _id;
        private int _parentid = 0;
        private string _parentname = "";
        private int _childid = 0;
        private string _childname = "";
        private string _childdescription = "";
        private string _remark = "";
        private int _a1 = 0;
        private DateTime _a2 = Convert.ToDateTime("1900-01-01");
        private string _a3 = "";
        private string _a4 = "";
        /// <summary>
        /// 主键
        /// </summary>
        public Guid id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 父级序号
        /// </summary>
        public int ParentID
        {
            set { _parentid = value; }
            get { return _parentid; }
        }
        /// <summary>
        /// 父级名称
        /// </summary>
        public string ParentName
        {
            set { _parentname = value; }
            get { return _parentname; }
        }
        /// <summary>
        /// 子级序号
        /// </summary>
        public int ChildID
        {
            set { _childid = value; }
            get { return _childid; }
        }
        /// <summary>
        /// 子级名称
        /// </summary>
        public string ChildName
        {
            set { _childname = value; }
            get { return _childname; }
        }
        /// <summary>
        /// 子级描述
        /// </summary>
        public string ChildDescription
        {
            set { _childdescription = value; }
            get { return _childdescription; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 0--（供应控制分销权限）
        /// 1--（控制系统权限）
        /// 2--（个人权限）

        /// </summary>
        public int A1
        {
            set { _a1 = value; }
            get { return _a1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime A2
        {
            set { _a2 = value; }
            get { return _a2; }
        }
        /// <summary>
        /// 退费票理由 是否需要操作编码(取消或者分离编码) 1是 其他不需要操作
        /// </summary>
        public string A3
        {
            set { _a3 = value; }
            get { return _a3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A4
        {
            set { _a4 = value; }
            get { return _a4; }
        }
        #endregion Model

    }
}

