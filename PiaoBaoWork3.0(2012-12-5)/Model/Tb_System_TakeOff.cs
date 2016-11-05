using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_System_TakeOff:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_System_TakeOff
	{
		public Tb_System_TakeOff()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _cpyname="";
		private int _cpytype=0;
		private DateTime _opertime= DateTime.Now;
		private string _operloginname="";
		private string _operusername="";
		private int _producttype=0;
		private int _ordertype=0;
		private decimal _policypoint=0M;
		private decimal _policymoney=0M;
		private int _a1=0;
		private int _a2=0;
		private decimal _a3=0M;
		private decimal _a4=0M;
		private DateTime _a5= Convert.ToDateTime("1900-01-01");
		private DateTime _a6= Convert.ToDateTime("1900-01-01");
		private string _a7="";
		private string _a8="";
		/// <summary>
		/// 
		/// </summary>
		public Guid id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CpyNo
		{
			set{ _cpyno=value;}
			get{return _cpyno;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CpyName
		{
			set{ _cpyname=value;}
			get{return _cpyname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int CpyType
		{
			set{ _cpytype=value;}
			get{return _cpytype;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime OperTime
		{
			set{ _opertime=value;}
			get{return _opertime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string OperLoginName
		{
			set{ _operloginname=value;}
			get{return _operloginname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string OperUserName
		{
			set{ _operusername=value;}
			get{return _operusername;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int ProductType
		{
			set{ _producttype=value;}
			get{return _producttype;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int OrderType
		{
			set{ _ordertype=value;}
			get{return _ordertype;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal PolicyPoint
		{
			set{ _policypoint=value;}
			get{return _policypoint;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal PolicyMoney
		{
			set{ _policymoney=value;}
			get{return _policymoney;}
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
		#endregion Model

	}
}

