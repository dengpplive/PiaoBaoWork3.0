using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Air_Carrier:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Air_Carrier
	{
		public Bd_Air_Carrier()
		{}
		#region Model
		private Guid _id;
		private string _airname="";
		private string _code="";
		private string _shortname="";
		private int _type=1;
		private int _saleflag=1;
		private string _settlecode="";
		private decimal _a1=0M;
		private DateTime _a2= Convert.ToDateTime("1900-01-01");
		private string _a3="";
		private string _a4="";
		private string _a5="";
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
		/// 航空公司全称
		/// </summary>
		public string AirName
		{
			set{ _airname=value;}
			get{return _airname;}
		}
		/// <summary>
		/// 航空公司二字码
		/// </summary>
		public string Code
		{
			set{ _code=value;}
			get{return _code;}
		}
		/// <summary>
		/// 航空公司简称
		/// </summary>
		public string ShortName
		{
			set{ _shortname=value;}
			get{return _shortname;}
		}
		/// <summary>
		/// 国家类型，1=国内，0=国际
		/// </summary>
		public int Type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// 是否销售该航空公司 0不销售 1销售 默认0或null
		/// </summary>
		public int SaleFlag
		{
			set{ _saleflag=value;}
			get{return _saleflag;}
		}
		/// <summary>
		/// 结算代码
		/// </summary>
		public string SettleCode
		{
			set{ _settlecode=value;}
			get{return _settlecode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A1
		{
			set{ _a1=value;}
			get{return _a1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime A2
		{
			set{ _a2=value;}
			get{return _a2;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A3
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

