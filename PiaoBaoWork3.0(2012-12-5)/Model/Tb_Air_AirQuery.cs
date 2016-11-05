using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Air_AirQuery:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Air_AirQuery
	{
		public Tb_Air_AirQuery()
		{}
		#region Model
		private int _id_auto;
		private Guid _id;
		private string _cpyno="";
		private string _loginname;
		private DateTime? _firstfromdate= Convert.ToDateTime("1900-01-01");
		private DateTime? _secondfromdate= Convert.ToDateTime("1900-01-01");
		private string _fromcitycode="";
		private string _tocitycode="";
		private string _carrycode="";
		private DateTime _createtime;
		private string _midelcitycode;
		/// <summary>
		/// 
		/// </summary>
		public int id_auto
		{
			set{ _id_auto=value;}
			get{return _id_auto;}
		}
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
        /// 查询人账号
		/// </summary>
		public string LoginName
		{
			set{ _loginname=value;}
			get{return _loginname;}
		}
		/// <summary>
		/// 起飞日期
		/// </summary>
		public DateTime? FirstFromDate
		{
			set{ _firstfromdate=value;}
			get{return _firstfromdate;}
		}
		/// <summary>
		/// 到达日期
		/// </summary>
		public DateTime? SecondFromDate
		{
			set{ _secondfromdate=value;}
			get{return _secondfromdate;}
		}
		/// <summary>
		/// 起飞城市三字码
		/// </summary>
		public string FromCityCode
		{
			set{ _fromcitycode=value;}
			get{return _fromcitycode;}
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
		/// 承运人代码
		/// </summary>
		public string CarryCode
		{
			set{ _carrycode=value;}
			get{return _carrycode;}
		}
		/// <summary>
		/// 查询时间
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		/// <summary>
		/// 中转城市
		/// </summary>
		public string MidelCityCode
		{
			set{ _midelcitycode=value;}
			get{return _midelcitycode;}
		}
		#endregion Model

	}
}

