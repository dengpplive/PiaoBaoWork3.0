using System;
namespace PbProject.Model
{
	/// <summary>
	/// User_LoginLog:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class User_LoginLog
	{
		public User_LoginLog()
		{}
		#region Model
		private Guid _id;
		private DateTime _logintime;
		private string _loginaccount;
		private string _loginip;
		private string _loginstate;
		private string _a1;
		private string _a2;
		private string _a3;
		private string _a4;
		private string _a5;
		/// <summary>
		/// 主键
		/// </summary>
		public Guid id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 登录时间
		/// </summary>
		public DateTime LoginTime
		{
			set{ _logintime=value;}
			get{return _logintime;}
		}
		/// <summary>
		/// 登录次数
		/// </summary>
		public string LoginAccount
		{
			set{ _loginaccount=value;}
			get{return _loginaccount;}
		}
		/// <summary>
		/// 登录ip
		/// </summary>
		public string LoginIp
		{
			set{ _loginip=value;}
			get{return _loginip;}
		}
		/// <summary>
		/// 登录状态
		/// </summary>
		public string LoginState
		{
			set{ _loginstate=value;}
			get{return _loginstate;}
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

