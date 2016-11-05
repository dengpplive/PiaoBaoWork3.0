using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Sms_Filters:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Sms_Filters
	{
		public Tb_Sms_Filters()
		{}
		#region Model
		private Guid _id;
		private string _smsfilcontent="";
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
		/// 过滤内容
		/// </summary>
		public string SmsFilContent
		{
			set{ _smsfilcontent=value;}
			get{return _smsfilcontent;}
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

