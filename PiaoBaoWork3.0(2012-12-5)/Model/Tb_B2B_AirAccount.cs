using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_B2B_AirAccount:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_B2B_AirAccount
	{
		public Tb_B2B_AirAccount()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _cpyname="";
		private int _cpytype=0;
		private DateTime _opertime= DateTime.Now;
		private string _operloginname="";
		private string _operusername="";
		private string _aircode="";
		private string _account="";
		private string _accountpwd="";
		private int _autopaysource=0;
		private int _autopaytype=0;
		private int _a1=0;
		private int _a2=0;
		private decimal _a3=0M;
		private decimal _a4=0M;
		private DateTime _a5= Convert.ToDateTime("1900-01-01");
		private DateTime _a6= Convert.ToDateTime("1900-01-01");
		private string _a7="";
		private string _a8="";
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
		/// 公司名称
		/// </summary>
		public string CpyName
		{
			set{ _cpyname=value;}
			get{return _cpyname;}
		}
		/// <summary>
		/// 公司类型
		/// </summary>
		public int CpyType
		{
			set{ _cpytype=value;}
			get{return _cpytype;}
		}
		/// <summary>
		/// 操作时间
		/// </summary>
		public DateTime OperTime
		{
			set{ _opertime=value;}
			get{return _opertime;}
		}
		/// <summary>
		/// 操作员登录名
		/// </summary>
		public string OperLoginName
		{
			set{ _operloginname=value;}
			get{return _operloginname;}
		}
		/// <summary>
		/// 操作员名称
		/// </summary>
		public string OperUserName
		{
			set{ _operusername=value;}
			get{return _operusername;}
		}
		/// <summary>
		/// 航空公司代码
		/// </summary>
		public string AirCode
		{
			set{ _aircode=value;}
			get{return _aircode;}
		}
		/// <summary>
		/// 账号
		/// </summary>
		public string Account
		{
			set{ _account=value;}
			get{return _account;}
		}
		/// <summary>
		/// 密码
		/// </summary>
		public string AccountPWD
		{
			set{ _accountpwd=value;}
			get{return _accountpwd;}
		}
		/// <summary>
		/// 自动出票代付类型（1=支付宝，2=汇付天下）
		/// </summary>
		public int AutoPaySource
		{
			set{ _autopaysource=value;}
			get{return _autopaysource;}
		}
		/// <summary>
		/// 支付方式（1=信用账户支付，2=付款账户支付）
		/// </summary>
		public int AutoPayType
		{
			set{ _autopaytype=value;}
			get{return _autopaytype;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public int A1
		{
			set{ _a1=value;}
			get{return _a1;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public int A2
		{
			set{ _a2=value;}
			get{return _a2;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public decimal A3
		{
			set{ _a3=value;}
			get{return _a3;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public decimal A4
		{
			set{ _a4=value;}
			get{return _a4;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public DateTime A5
		{
			set{ _a5=value;}
			get{return _a5;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public DateTime A6
		{
			set{ _a6=value;}
			get{return _a6;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public string A7
		{
			set{ _a7=value;}
			get{return _a7;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public string A8
		{
			set{ _a8=value;}
			get{return _a8;}
		}
		#endregion Model

	}
}

