using System;
namespace PbProject.Model
{
	/// <summary>
	/// Log_Operation:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Log_Operation
	{
		public Log_Operation()
		{}
		#region Model
		private Guid _id;
		private string _modulename="";
		private string _operatetype="";
		private string _cpyno="";
		private string _loginname="";
		private string _username="";
		private string _orderid="";
		private DateTime _createtime= DateTime.Now;
		private string _optcontent="";
		private string _clientip="";
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
		/// 模块名称
		/// </summary>
		public string ModuleName
		{
			set{ _modulename=value;}
			get{return _modulename;}
		}
		/// <summary>
		/// 操作类型
		/// </summary>
		public string OperateType
		{
			set{ _operatetype=value;}
			get{return _operatetype;}
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
		/// 操作帐号
		/// </summary>
		public string LoginName
		{
			set{ _loginname=value;}
			get{return _loginname;}
		}
		/// <summary>
		/// 操作用户名
		/// </summary>
		public string UserName
		{
			set{ _username=value;}
			get{return _username;}
		}
		/// <summary>
		/// 订单号
		/// </summary>
		public string OrderId
		{
			set{ _orderid=value;}
			get{return _orderid;}
		}
		/// <summary>
		/// 记录时间
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		/// <summary>
		/// 操作内容
		/// </summary>
		public string OptContent
		{
			set{ _optcontent=value;}
			get{return _optcontent;}
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

