using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Sms_RateSet:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Sms_RateSet
	{
		public Tb_Sms_RateSet()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private decimal _ratesmoney=0M;
		private int _ratescount=0;
		private decimal _ratesunitprice=0M;
		private string _ratesremark="";
		private int _ratesstate=0;
		private DateTime _ratesdatetime= Convert.ToDateTime("1900-01-01");
		private string _a1="";
		private string _a2="";
		private string _a3="";
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
		/// 金额
		/// </summary>
		public decimal RatesMoney
		{
			set{ _ratesmoney=value;}
			get{return _ratesmoney;}
		}
		/// <summary>
		/// 短信条数
		/// </summary>
		public int RatesCount
		{
			set{ _ratescount=value;}
			get{return _ratescount;}
		}
		/// <summary>
		/// 单价
		/// </summary>
		public decimal RatesUnitPrice
		{
			set{ _ratesunitprice=value;}
			get{return _ratesunitprice;}
		}
		/// <summary>
		/// 备注
		/// </summary>
		public string RatesRemark
		{
			set{ _ratesremark=value;}
			get{return _ratesremark;}
		}
		/// <summary>
		/// 状态 0=禁用，1=启用
		/// </summary>
		public int RatesState
		{
			set{ _ratesstate=value;}
			get{return _ratesstate;}
		}
		/// <summary>
		/// 时间
		/// </summary>
		public DateTime RatesDateTime
		{
			set{ _ratesdatetime=value;}
			get{return _ratesdatetime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A1
		{
			set{ _a1=value;}
			get{return _a1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A2
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
		#endregion Model

	}
}

