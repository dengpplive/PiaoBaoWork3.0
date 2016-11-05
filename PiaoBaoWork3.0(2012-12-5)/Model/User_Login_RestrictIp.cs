using System;
namespace PbProject.Model
{
	/// <summary>
	/// User_Login_RestrictIp:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class User_Login_RestrictIp
	{
		public User_Login_RestrictIp()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _restrictloginaccount="";
		private string _restrictloginip="";
		private DateTime? _opertime= DateTime.Now;
		private string _a1="";
		private string _a2="";
		private string _a3="";
		/// <summary>
		/// 
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
		/// 登录账号
		/// </summary>
		public string RestrictLoginAccount
		{
			set{ _restrictloginaccount=value;}
			get{return _restrictloginaccount;}
		}
		/// <summary>
		/// 登录允许通过的ip
		/// </summary>
		public string RestrictLoginIP
		{
			set{ _restrictloginip=value;}
			get{return _restrictloginip;}
		}
		/// <summary>
		/// 登录时间
		/// </summary>
		public DateTime? OperTime
		{
			set{ _opertime=value;}
			get{return _opertime;}
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

