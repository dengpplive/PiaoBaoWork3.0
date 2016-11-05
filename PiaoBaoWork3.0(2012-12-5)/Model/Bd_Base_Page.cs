using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Base_Page:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Base_Page
	{
		public Bd_Base_Page()
		{}
		#region Model
		private Guid _id;
		private int _moduleindex=0;
		private string _modulename="";
		private int _onemenuindex=0;
		private string _onemenuname="";
		private int _twomenuindex=0;
		private string _twomenuname="";
		private int _pageindex=0;
		private string _pagename="";
		private string _pageurl="";
		private string _remark="";
		private int _roletype=0;
		private int _a1=0;
		private int _a2=0;
		private decimal _a3=0M;
		private decimal _a4=0M;
		private DateTime _a5= Convert.ToDateTime("1900-01-01");
		private DateTime _a6= Convert.ToDateTime("1900-01-01");
		private string _a7="";
		private string _a8="";
		private string _a9="";
		private string _a10="";
		/// <summary>
		/// 主键
		/// </summary>
		public Guid id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 模块索引
		/// </summary>
		public int ModuleIndex
		{
			set{ _moduleindex=value;}
			get{return _moduleindex;}
		}
		/// <summary>
		/// 模块名称
		/// </summary>
		public string ModuleName
		{
			set{ _modulename=value;}
			get{return _modulename;}
		}
		/// <summary>
		/// 一级菜单索引
		/// </summary>
		public int OneMenuIndex
		{
			set{ _onemenuindex=value;}
			get{return _onemenuindex;}
		}
		/// <summary>
		/// 一级菜单名称
		/// </summary>
		public string OneMenuName
		{
			set{ _onemenuname=value;}
			get{return _onemenuname;}
		}
		/// <summary>
		/// 二级菜单索引
		/// </summary>
		public int TwoMenuIndex
		{
			set{ _twomenuindex=value;}
			get{return _twomenuindex;}
		}
		/// <summary>
		/// 二级菜单名称
		/// </summary>
		public string TwoMenuName
		{
			set{ _twomenuname=value;}
			get{return _twomenuname;}
		}
		/// <summary>
		/// 页面索引
		/// </summary>
		public int PageIndex
		{
			set{ _pageindex=value;}
			get{return _pageindex;}
		}
		/// <summary>
		/// 页面名称
		/// </summary>
		public string PageName
		{
			set{ _pagename=value;}
			get{return _pagename;}
		}
		/// <summary>
		/// 页面链接地址
		/// </summary>
		public string PageURL
		{
			set{ _pageurl=value;}
			get{return _pageurl;}
		}
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		/// <summary>
		/// 角色类型1=平台，2=运营商，3=供应商，4=分销商，5=采购商
		/// </summary>
		public int RoleType
		{
			set{ _roletype=value;}
			get{return _roletype;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int A1
		{
			set{ _a1=value;}
			get{return _a1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int A2
		{
			set{ _a2=value;}
			get{return _a2;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A3
		{
			set{ _a3=value;}
			get{return _a3;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A4
		{
			set{ _a4=value;}
			get{return _a4;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime A5
		{
			set{ _a5=value;}
			get{return _a5;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime A6
		{
			set{ _a6=value;}
			get{return _a6;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A7
		{
			set{ _a7=value;}
			get{return _a7;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A8
		{
			set{ _a8=value;}
			get{return _a8;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A9
		{
			set{ _a9=value;}
			get{return _a9;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A10
		{
			set{ _a10=value;}
			get{return _a10;}
		}
		#endregion Model

	}
}

