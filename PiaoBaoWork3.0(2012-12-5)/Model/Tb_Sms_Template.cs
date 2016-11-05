using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Sms_Template:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Sms_Template
	{
		public Tb_Sms_Template()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private int _smstptype=0;
		private string _smstpname="";
		private string _smstpcontent="";
		private DateTime _smstpdate= DateTime.Now;
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
		/// 模板类型 0=标准，1=自定义
		/// </summary>
		public int SmsTpType
		{
			set{ _smstptype=value;}
			get{return _smstptype;}
		}
		/// <summary>
		/// 模板名称
		/// </summary>
		public string SmsTpName
		{
			set{ _smstpname=value;}
			get{return _smstpname;}
		}
		/// <summary>
		/// 内容
		/// </summary>
		public string SmsTpContent
		{
			set{ _smstpcontent=value;}
			get{return _smstpcontent;}
		}
		/// <summary>
		/// 操作时间
		/// </summary>
		public DateTime SmsTpDate
		{
			set{ _smstpdate=value;}
			get{return _smstpdate;}
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

