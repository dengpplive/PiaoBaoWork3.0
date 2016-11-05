using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Air_AirPort:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Air_AirPort
	{
		public Bd_Air_AirPort()
		{}
		#region Model
		private Guid _id;
		private string _cityname="";
		private string _cityquanpin="";
		private string _cityjianpin="";
		private string _citycodeword="";
		private string _cityothercodeword="";
		private string _airportname="";
		private string _country="中国";
		private string _continents="亚洲";
		private int _isdomestic=1;
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
		/// 城市名称
		/// </summary>
		public string CityName
		{
			set{ _cityname=value;}
			get{return _cityname;}
		}
		/// <summary>
		/// 城市全拼
		/// </summary>
		public string CityQuanPin
		{
			set{ _cityquanpin=value;}
			get{return _cityquanpin;}
		}
		/// <summary>
		/// 城市简拼
		/// </summary>
		public string CityJianPin
		{
			set{ _cityjianpin=value;}
			get{return _cityjianpin;}
		}
		/// <summary>
		/// 城市三字码
		/// </summary>
		public string CityCodeWord
		{
			set{ _citycodeword=value;}
			get{return _citycodeword;}
		}
		/// <summary>
		/// 城市其它三字码
		/// </summary>
		public string CityOtherCodeWord
		{
			set{ _cityothercodeword=value;}
			get{return _cityothercodeword;}
		}
		/// <summary>
		/// 机场名称
		/// </summary>
		public string AirPortName
		{
			set{ _airportname=value;}
			get{return _airportname;}
		}
		/// <summary>
		/// 所在国家
		/// </summary>
		public string Country
		{
			set{ _country=value;}
			get{return _country;}
		}
		/// <summary>
		/// 所在洲
		/// </summary>
		public string Continents
		{
			set{ _continents=value;}
			get{return _continents;}
		}
		/// <summary>
		/// 是否国内 1.是，2.否
		/// </summary>
		public int IsDomestic
		{
			set{ _isdomestic=value;}
			get{return _isdomestic;}
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

