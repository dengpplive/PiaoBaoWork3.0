using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Base_Parameters:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Base_Parameters
	{
		public Bd_Base_Parameters()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _setname="";
		private string _setvalue="";
		private string _setdescription="";
		private DateTime _startdate= Convert.ToDateTime("1900-01-01");
		private DateTime _enddate= Convert.ToDateTime("1900-01-01");
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
		/// 公司编号
		/// </summary>
		public string CpyNo
		{
			set{ _cpyno=value;}
			get{return _cpyno;}
		}
		/// <summary>
		/// 参数名称
		/// </summary>
		public string SetName
		{
			set{ _setname=value;}
			get{return _setname;}
		}
		/// <summary>
		/// 参数值
		/// </summary>
		public string SetValue
		{
			set{ _setvalue=value;}
			get{return _setvalue;}
		}
		/// <summary>
		/// 参数描述
		/// </summary>
		public string SetDescription
		{
			set{ _setdescription=value;}
			get{return _setdescription;}
		}
		/// <summary>
		/// 有效起始日期
		/// </summary>
		public DateTime StartDate
		{
			set{ _startdate=value;}
			get{return _startdate;}
		}
		/// <summary>
		/// 有效截止日期
		/// </summary>
		public DateTime EndDate
		{
			set{ _enddate=value;}
			get{return _enddate;}
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

