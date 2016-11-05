using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_SpecialCabin:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_SpecialCabin
	{
		public Tb_SpecialCabin()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _spairshortname="";
		private string _spaircode="";
		private string _spcabin="";
		private DateTime _spstarttime= DateTime.Now;
		private DateTime _spendtime= DateTime.Now.AddDays(365);
		private DateTime _spaddtime= DateTime.Now;
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
		/// 公司编号
		/// </summary>
		public string CpyNo
		{
			set{ _cpyno=value;}
			get{return _cpyno;}
		}
		/// <summary>
		/// 航空公司简称
		/// </summary>
		public string SpAirShortName
		{
			set{ _spairshortname=value;}
			get{return _spairshortname;}
		}
		/// <summary>
		/// 航空公司二字码
		/// </summary>
		public string SpAirCode
		{
			set{ _spaircode=value;}
			get{return _spaircode;}
		}
		/// <summary>
		/// 特价仓位
		/// </summary>
		public string SpCabin
		{
			set{ _spcabin=value;}
			get{return _spcabin;}
		}
		/// <summary>
		/// 特价仓位生效期
		/// </summary>
		public DateTime SpStartTime
		{
			set{ _spstarttime=value;}
			get{return _spstarttime;}
		}
		/// <summary>
		/// 特价仓位结束期
		/// </summary>
		public DateTime SpEndTime
		{
			set{ _spendtime=value;}
			get{return _spendtime;}
		}
		/// <summary>
		/// 添加时间
		/// </summary>
		public DateTime SpAddTime
		{
			set{ _spaddtime=value;}
			get{return _spaddtime;}
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

