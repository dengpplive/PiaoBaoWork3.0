using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Air_TGQProvision:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Air_TGQProvision
	{
		public Bd_Air_TGQProvision()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _carrycode="";
		private string _fromcity="";
		private string _middlecity="";
		private string _tocity="";
		private int _traveltype=1;
		private string _spaces="";
		private string _dishonoredbillprescript="";
		private string _logchangeprescript="";
		private DateTime _begindate= DateTime.Now;
		private DateTime _enddate= DateTime.Now.AddDays(365);
		private int _a1=0;
		private int _a2=0;
		private int _a3=0;
		private int _a4=0;
		private int _a5=0;
		private string _a6="";
		private string _a7="";
		private string _a8="";
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
		/// 承运人编码
		/// </summary>
		public string CarryCode
		{
			set{ _carrycode=value;}
			get{return _carrycode;}
		}
		/// <summary>
		/// 出发城市
		/// </summary>
		public string FromCity
		{
			set{ _fromcity=value;}
			get{return _fromcity;}
		}
		/// <summary>
		/// 中转城市
		/// </summary>
		public string MiddleCity
		{
			set{ _middlecity=value;}
			get{return _middlecity;}
		}
		/// <summary>
		/// 到达城市
		/// </summary>
		public string ToCity
		{
			set{ _tocity=value;}
			get{return _tocity;}
		}
		/// <summary>
		/// 单程/往返，1=单程，2=往返，3=中转
		/// </summary>
		public int TravelType
		{
			set{ _traveltype=value;}
			get{return _traveltype;}
		}
		/// <summary>
		/// 舱位(以/分割)
		/// </summary>
		public string Spaces
		{
			set{ _spaces=value;}
			get{return _spaces;}
		}
		/// <summary>
		/// 退票规定
		/// </summary>
		public string DishonoredBillPrescript
		{
			set{ _dishonoredbillprescript=value;}
			get{return _dishonoredbillprescript;}
		}
		/// <summary>
		/// 改签规定
		/// </summary>
		public string LogChangePrescript
		{
			set{ _logchangeprescript=value;}
			get{return _logchangeprescript;}
		}
		/// <summary>
		/// 起始日期
		/// </summary>
		public DateTime BeginDate
		{
			set{ _begindate=value;}
			get{return _begindate;}
		}
		/// <summary>
		/// 截止日期
		/// </summary>
		public DateTime EndDate
		{
			set{ _enddate=value;}
			get{return _enddate;}
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
		public int A3
		{
			set{ _a3=value;}
			get{return _a3;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int A4
		{
			set{ _a4=value;}
			get{return _a4;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int A5
		{
			set{ _a5=value;}
			get{return _a5;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A6
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

