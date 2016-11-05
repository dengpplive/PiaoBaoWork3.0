using System;
namespace PbProject.Model
{
	/// <summary>
	/// User_Permissions:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class User_Permissions
	{
		public User_Permissions()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _deptname="";
		private int _deptindex=0;
		private int _parentindex=0;
		private string _permissions="";
		private string _remark="";
		private int _a1=0;
		private decimal _a2=0M;
		private DateTime _a3= Convert.ToDateTime("1900-01-01");
		private string _a4="";
		private string _a5="";
		/// <summary>
		/// 主键
		/// </summary>
		public Guid id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 公司编号
		/// </summary>
		public string CpyNo
		{
			set{ _cpyno=value;}
			get{return _cpyno;}
		}
		/// <summary>
		/// 部门名称
		/// </summary>
		public string DeptName
		{
			set{ _deptname=value;}
			get{return _deptname;}
		}
		/// <summary>
		/// 部门序号
		/// </summary>
		public int DeptIndex
		{
			set{ _deptindex=value;}
			get{return _deptindex;}
		}
		/// <summary>
		/// 上级部门序号
		/// </summary>
		public int ParentIndex
		{
			set{ _parentindex=value;}
			get{return _parentindex;}
		}
		/// <summary>
		/// 权限集合
		/// </summary>
		public string Permissions
		{
			set{ _permissions=value;}
			get{return _permissions;}
		}
		/// <summary>
		/// 说明
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		/// <summary>
		///  默认值 0 管理员， 1 员工
		/// </summary>
		public int A1
		{
			set{ _a1=value;}
			get{return _a1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A2
		{
			set{ _a2=value;}
			get{return _a2;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime A3
		{
			set{ _a3=value;}
			get{return _a3;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A4
		{
			set{ _a4=value;}
			get{return _a4;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A5
		{
			set{ _a5=value;}
			get{return _a5;}
		}
		#endregion Model

	}
}

