using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Air_BaseCabin:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Air_BaseCabin
	{
		public Bd_Air_BaseCabin()
		{}
		#region Model
		private Guid _id;
		private string _airshortname="";
		private string _aircode="";
		private string _cabin="";
		private string _cabinname="";
		private decimal _rebate=0M;
		private DateTime _starttime= DateTime.Now;
		private DateTime _endtime=DateTime.Now.AddDays(365);
		private DateTime _addtime= DateTime.Now;
		private string _a1="";
		private string _a2="";
		private string _a3="";
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
		/// 航空公司简称
		/// </summary>
		public string AirShortName
		{
			set{ _airshortname=value;}
			get{return _airshortname;}
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
		/// 舱位代码
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
		/// 舱位折扣（百分比）
		/// </summary>
		public decimal Rebate
		{
			set{ _rebate=value;}
			get{return _rebate;}
		}
		/// <summary>
		/// 舱位生效日期
		/// </summary>
		public DateTime StartTime
		{
			set{ _starttime=value;}
			get{return _starttime;}
		}
		/// <summary>
		/// 舱位失效日期
		/// </summary>
		public DateTime EndTime
		{
			set{ _endtime=value;}
			get{return _endtime;}
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

