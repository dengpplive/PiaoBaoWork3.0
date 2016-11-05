using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Air_Fares:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Air_Fares
	{
		public Bd_Air_Fares()
		{}
		#region Model
		private Guid _id;
		private string _fromcityname="";
		private string _fromcitycode="";
		private string _tocityname="";
		private string _tocitycode="";
		private decimal _farefee=0M;
		private int _mileage=0;
		private int _isdomestic=1;
		private string _carrycode="";
		private DateTime _effecttime= DateTime.Now;
		private DateTime _invalidtime= DateTime.Now.AddDays(365);
		private string _remark="";
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
		/// 出发城市
		/// </summary>
		public string FromCityName
		{
			set{ _fromcityname=value;}
			get{return _fromcityname;}
		}
		/// <summary>
		/// 出发城市三字码
		/// </summary>
		public string FromCityCode
		{
			set{ _fromcitycode=value;}
			get{return _fromcitycode;}
		}
		/// <summary>
		/// 到达城市
		/// </summary>
		public string ToCityName
		{
			set{ _tocityname=value;}
			get{return _tocityname;}
		}
		/// <summary>
		/// 到达城市三字码
		/// </summary>
		public string ToCityCode
		{
			set{ _tocitycode=value;}
			get{return _tocitycode;}
		}
		/// <summary>
		/// 票价
		/// </summary>
		public decimal FareFee
		{
			set{ _farefee=value;}
			get{return _farefee;}
		}
		/// <summary>
		/// 里程
		/// </summary>
		public int Mileage
		{
			set{ _mileage=value;}
			get{return _mileage;}
		}
		/// <summary>
		/// 是否国内  1=国内，0=国际
		/// </summary>
		public int IsDomestic
		{
			set{ _isdomestic=value;}
			get{return _isdomestic;}
		}
		/// <summary>
		/// 承运人代码
		/// </summary>
		public string CarryCode
		{
			set{ _carrycode=value;}
			get{return _carrycode;}
		}
		/// <summary>
		/// 生效日期
		/// </summary>
		public DateTime EffectTime
		{
			set{ _effecttime=value;}
			get{return _effecttime;}
		}
		/// <summary>
		/// 失效日期
		/// </summary>
		public DateTime InvalidTime
		{
			set{ _invalidtime=value;}
			get{return _invalidtime;}
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

