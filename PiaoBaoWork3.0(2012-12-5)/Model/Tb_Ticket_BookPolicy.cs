using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Ticket_BookPolicy:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Ticket_BookPolicy
	{
		public Tb_Ticket_BookPolicy()
		{}
		#region Model
		private Guid _id;
		private int _policysource=0;
		private string _policysourcename="";
		private string _policyid="";
		private int _policystate=0;
		private string _fromcity="";
		private string _tocity="";
		private string _airlines="";
		private int _triptype=0;
		private string _flight="";
		private string _noflight="";
		private string _etclimit="";
		private int _tickettype=0;
		private string _shipping="";
		private decimal _preturn=0M;
		private string _gtickettimee="";
		private DateTime _effectivedate= Convert.ToDateTime("1900-01-01");
		private DateTime _expirydate= Convert.ToDateTime("1900-01-01");
		private string _gyhxnumber="";
		private DateTime _insertdate= Convert.ToDateTime("1900-01-01");
		private DateTime _updatedate= Convert.ToDateTime("1900-01-01");
		private string _gyptnumber="";
		private int _isaugticket=0;
		private string _remark="";
		private string _providerworktime="";
		private DateTime _addtime= DateTime.Now;
		private string _addcpyno="";
		private string _addcpyname="";
		private int _a1=0;
		private int _a2=0;
		private int _a3=0;
		private int _a4=0;
		private int _a5=0;
		private int _a6=0;
		private string _a7="";
		private string _a8="";
		private string _a9="";
		private string _a10="";
		private string _a11="";
		private string _a12="";
		private string _a13="";
		private string _a14="";
		private decimal _a15=0M;
		private decimal _a16=0M;
		private decimal _a17=0M;
		private decimal _a18=0M;
		private DateTime _a19= Convert.ToDateTime("1900-01-01");
		private DateTime _a20= Convert.ToDateTime("1900-01-01");
		/// <summary>
		/// 主键
		/// </summary>
		public Guid id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 政策来源
		/// </summary>
		public int PolicySource
		{
			set{ _policysource=value;}
			get{return _policysource;}
		}
		/// <summary>
		/// 政策来源名称
		/// </summary>
		public string PolicySourceName
		{
			set{ _policysourcename=value;}
			get{return _policysourcename;}
		}
		/// <summary>
		/// 政策编号
		/// </summary>
		public string PolicyId
		{
			set{ _policyid=value;}
			get{return _policyid;}
		}
		/// <summary>
		/// 政策状态
		/// </summary>
		public int PolicyState
		{
			set{ _policystate=value;}
			get{return _policystate;}
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
		/// 到达城市
		/// </summary>
		public string ToCity
		{
			set{ _tocity=value;}
			get{return _tocity;}
		}
		/// <summary>
		/// 航空公司
		/// </summary>
		public string Airlines
		{
			set{ _airlines=value;}
			get{return _airlines;}
		}
		/// <summary>
		/// 行程类型
		/// </summary>
		public int TripType
		{
			set{ _triptype=value;}
			get{return _triptype;}
		}
		/// <summary>
		/// 航班
		/// </summary>
		public string Flight
		{
			set{ _flight=value;}
			get{return _flight;}
		}
		/// <summary>
		/// 不适用航班
		/// </summary>
		public string NoFlight
		{
			set{ _noflight=value;}
			get{return _noflight;}
		}
		/// <summary>
		/// 班期限制
		/// </summary>
		public string EtcLimit
		{
			set{ _etclimit=value;}
			get{return _etclimit;}
		}
		/// <summary>
		/// 票证类型
		/// </summary>
		public int TicketType
		{
			set{ _tickettype=value;}
			get{return _tickettype;}
		}
		/// <summary>
		/// 舱位
		/// </summary>
		public string Shipping
		{
			set{ _shipping=value;}
			get{return _shipping;}
		}
		/// <summary>
		/// 政策返点
		/// </summary>
		public decimal PReturn
		{
			set{ _preturn=value;}
			get{return _preturn;}
		}
		/// <summary>
		/// 出票时限
		/// </summary>
		public string GTicketTimeE
		{
			set{ _gtickettimee=value;}
			get{return _gtickettimee;}
		}
		/// <summary>
		/// 生效时间
		/// </summary>
		public DateTime EffectiveDate
		{
			set{ _effectivedate=value;}
			get{return _effectivedate;}
		}
		/// <summary>
		/// 失效时间
		/// </summary>
		public DateTime ExpiryDate
		{
			set{ _expirydate=value;}
			get{return _expirydate;}
		}
		/// <summary>
		/// 供应商航信代号
		/// </summary>
		public string GYHXNumber
		{
			set{ _gyhxnumber=value;}
			get{return _gyhxnumber;}
		}
		/// <summary>
		/// 创建日期
		/// </summary>
		public DateTime InsertDate
		{
			set{ _insertdate=value;}
			get{return _insertdate;}
		}
		/// <summary>
		/// 修改日期
		/// </summary>
		public DateTime UpdateDate
		{
			set{ _updatedate=value;}
			get{return _updatedate;}
		}
		/// <summary>
		/// 供应商平台代号
		/// </summary>
		public string GYPTNumber
		{
			set{ _gyptnumber=value;}
			get{return _gyptnumber;}
		}
		/// <summary>
		/// 创建日期
		/// </summary>
		public int IsAuGTicket
		{
			set{ _isaugticket=value;}
			get{return _isaugticket;}
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
		/// 提供商工作时间
		/// </summary>
		public string ProviderWorkTime
		{
			set{ _providerworktime=value;}
			get{return _providerworktime;}
		}
		/// <summary>
		/// 添加时间
		/// </summary>
		public DateTime AddTime
		{
			set{ _addtime=value;}
			get{return _addtime;}
		}
		/// <summary>
		/// 添加人所属公司编号
		/// </summary>
		public string AddCpyNo
		{
			set{ _addcpyno=value;}
			get{return _addcpyno;}
		}
		/// <summary>
		/// 添加人所属公司名称
		/// </summary>
		public string AddCpyName
		{
			set{ _addcpyname=value;}
			get{return _addcpyname;}
		}
		/// <summary>
		/// 是否特价(0不是,1是)
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
		public int A6
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
		/// <summary>
		/// 
		/// </summary>
		public string A11
		{
			set{ _a11=value;}
			get{return _a11;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A12
		{
			set{ _a12=value;}
			get{return _a12;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A13
		{
			set{ _a13=value;}
			get{return _a13;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A14
		{
			set{ _a14=value;}
			get{return _a14;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A15
		{
			set{ _a15=value;}
			get{return _a15;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A16
		{
			set{ _a16=value;}
			get{return _a16;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A17
		{
			set{ _a17=value;}
			get{return _a17;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A18
		{
			set{ _a18=value;}
			get{return _a18;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime A19
		{
			set{ _a19=value;}
			get{return _a19;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime A20
		{
			set{ _a20=value;}
			get{return _a20;}
		}
		#endregion Model

	}
}

