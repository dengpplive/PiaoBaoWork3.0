using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Sms_User:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Sms_User
	{
		public Tb_Sms_User()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private int _smscount=0;
		private int _smsremaincount=0;
		private DateTime _smsdate= DateTime.Now;
		private int _smsusertype=0;
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
		/// 短信条数
		/// </summary>
		public int SmsCount
		{
			set{ _smscount=value;}
			get{return _smscount;}
		}
		/// <summary>
		/// 剩余条数
		/// </summary>
		public int SmsRemainCount
		{
			set{ _smsremaincount=value;}
			get{return _smsremaincount;}
		}
		/// <summary>
		/// 上次充值日期
		/// </summary>
		public DateTime SmsDate
		{
			set{ _smsdate=value;}
			get{return _smsdate;}
		}
		/// <summary>
		/// 用户类型
		/// </summary>
		public int SmsUserType
		{
			set{ _smsusertype=value;}
			get{return _smsusertype;}
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

