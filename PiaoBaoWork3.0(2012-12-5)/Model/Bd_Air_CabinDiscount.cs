using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Air_CabinDiscount:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Air_CabinDiscount
	{
		public Bd_Air_CabinDiscount()
		{}
		#region Model
		private Guid _id;
		private string _aircode="";
		private string _airname="";
		private string _cabin="";
		private string _cabinname="";
		private decimal _cabinprice=0M;
		private DateTime _adddate= DateTime.Now;
		private string _fromcity="";
		private string _fromcitycode="";
		private string _tocity="";
		private string _tocitycode="";
		private DateTime _begintime= DateTime.Now;
		private DateTime _endtime= DateTime.Now.AddDays(365);
		private int _isgn=1;
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
		/// 航空公司二字码
		/// </summary>
		public string AirCode
		{
			set{ _aircode=value;}
			get{return _aircode;}
		}
		/// <summary>
		/// 航空公司名称
		/// </summary>
		public string AirName
		{
			set{ _airname=value;}
			get{return _airname;}
		}
		/// <summary>
		/// 舱位
		/// </summary>
		public string Cabin
		{
			set{ _cabin=value;}
			get{return _cabin;}
		}
		/// <summary>
		/// 舱位名称（头等舱、商务舱、全价、经济舱）
		/// </summary>
		public string CabinName
		{
			set{ _cabinname=value;}
			get{return _cabinname;}
		}
		/// <summary>
		/// 舱位价格
		/// </summary>
		public decimal CabinPrice
		{
			set{ _cabinprice=value;}
			get{return _cabinprice;}
		}
		/// <summary>
		/// 添加时间
		/// </summary>
		public DateTime AddDate
		{
			set{ _adddate=value;}
			get{return _adddate;}
		}
		/// <summary>
		/// 出发城市名称
		/// </summary>
		public string FromCity
		{
			set{ _fromcity=value;}
			get{return _fromcity;}
		}
		/// <summary>
		/// 出发城市3字码
		/// </summary>
		public string FromCityCode
		{
			set{ _fromcitycode=value;}
			get{return _fromcitycode;}
		}
		/// <summary>
		/// 到达城市名称
		/// </summary>
		public string ToCity
		{
			set{ _tocity=value;}
			get{return _tocity;}
		}
		/// <summary>
		/// 到达城市3字码
		/// </summary>
		public string ToCityCode
		{
			set{ _tocitycode=value;}
			get{return _tocitycode;}
		}
		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime BeginTime
		{
			set{ _begintime=value;}
			get{return _begintime;}
		}
		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime EndTime
		{
			set{ _endtime=value;}
			get{return _endtime;}
		}
		/// <summary>
		/// 是否国内（1=国内，0=国际）
		/// </summary>
		public int IsGN
		{
			set{ _isgn=value;}
			get{return _isgn;}
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

