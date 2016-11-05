using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Sms_SendInfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Sms_SendInfo
	{
		public Tb_Sms_SendInfo()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private int _smsinterfacetype=0;
		private string _smsacceptmobilephone="";
		private string _smssendcontent="";
		private string _smssuffix="";
		private string _smsunit="";
		private int _smsusercount=0;
		private int _smssendstate=0;
		private DateTime _smscreatedate= DateTime.Now;
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
		/// 短信接口（用于存在多个短信接口时进行标识）
		/// </summary>
		public int SmsInterfaceType
		{
			set{ _smsinterfacetype=value;}
			get{return _smsinterfacetype;}
		}
		/// <summary>
		/// 接收手机号码
		/// </summary>
		public string SmsAcceptMobilePhone
		{
			set{ _smsacceptmobilephone=value;}
			get{return _smsacceptmobilephone;}
		}
		/// <summary>
		/// 发送内容
		/// </summary>
		public string SmsSendContent
		{
			set{ _smssendcontent=value;}
			get{return _smssendcontent;}
		}
		/// <summary>
		/// 短信尾巴
		/// </summary>
		public string SmsSuffix
		{
			set{ _smssuffix=value;}
			get{return _smssuffix;}
		}
		/// <summary>
		/// 短信单位
		/// </summary>
		public string SmsUnit
		{
			set{ _smsunit=value;}
			get{return _smsunit;}
		}
		/// <summary>
		/// 短信条数
		/// </summary>
		public int SmsUserCount
		{
			set{ _smsusercount=value;}
			get{return _smsusercount;}
		}
		/// <summary>
		/// 发送状态 0=未发送，1=已发
		/// </summary>
		public int SmsSendState
		{
			set{ _smssendstate=value;}
			get{return _smssendstate;}
		}
		/// <summary>
		/// 发送时间
		/// </summary>
		public DateTime SmsCreateDate
		{
			set{ _smscreatedate=value;}
			get{return _smscreatedate;}
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

