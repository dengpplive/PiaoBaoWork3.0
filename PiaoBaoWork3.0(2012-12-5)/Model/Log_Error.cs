using System;
namespace PbProject.Model
{
	/// <summary>
	/// Log_Error:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Log_Error
	{
		public Log_Error()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private DateTime _erorrtime= DateTime.Now;
		private string _page="";
		private string _loginname="";
		private string _method="";
		private string _errorcontent="";
		private string _clientip="";
		private string _devname="";
		private int _a1=0;
		private int _a2=0;
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
		/// 记录时间
		/// </summary>
		public DateTime ErorrTime
		{
			set{ _erorrtime=value;}
			get{return _erorrtime;}
		}
		/// <summary>
		/// 页面
		/// </summary>
		public string Page
		{
			set{ _page=value;}
			get{return _page;}
		}
		/// <summary>
		/// 登录帐号
		/// </summary>
		public string LoginName
		{
			set{ _loginname=value;}
			get{return _loginname;}
		}
		/// <summary>
		/// 错误方法
		/// </summary>
		public string Method
		{
			set{ _method=value;}
			get{return _method;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ErrorContent
		{
			set{ _errorcontent=value;}
			get{return _errorcontent;}
		}
		/// <summary>
		/// 客户登录IP
		/// </summary>
		public string ClientIP
		{
			set{ _clientip=value;}
			get{return _clientip;}
		}
		/// <summary>
		/// 开发人员名称
		/// </summary>
		public string DevName
		{
			set{ _devname=value;}
			get{return _devname;}
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
		public int A2
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

