using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Base_Notice:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Base_Notice
	{
		public Bd_Base_Notice()
		{}
		#region Model
		private Guid _id;
		private string _title="";
		private string _content="";
		private DateTime _releasetime= DateTime.Now;
		private string _releaseaccount="";
		private string _releasename="";
		private string _releasecpyno="";
		private string _releasecpyname="";
		private int _callboardtype=1;
		private DateTime _audittime= Convert.ToDateTime("1900-01-01");
		private string _auditaccount="";
		private string _auditaccountname="";
		private int _emergency=1;
		private int _rollflag=1;
		private int _isinternal=1;
		private int _clickcount=0;
		private DateTime _startdate= DateTime.Now;
		private DateTime _expirationdate= DateTime.Now.AddDays(365);
		private string _remark="";
		private string _attachmentfilename="";
		private byte[] _fileattachment;
		private int _a1=0;
		private int _a2=0;
		private decimal _a3=0M;
		private decimal _a4=0M;
		private DateTime _a5= Convert.ToDateTime("1900-01-01");
		private DateTime _a6= Convert.ToDateTime("1900-01-01");
		private string _a7="";
		private string _a8="";
		private string _a9="";
		private string _a10="";
		/// <summary>
		/// 主键
		/// </summary>
		public Guid id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public string Title
		{
			set{ _title=value;}
			get{return _title;}
		}
		/// <summary>
		/// 内容
		/// </summary>
		public string Content
		{
			set{ _content=value;}
			get{return _content;}
		}
		/// <summary>
		/// 发布时间
		/// </summary>
		public DateTime ReleaseTime
		{
			set{ _releasetime=value;}
			get{return _releasetime;}
		}
		/// <summary>
		/// 发布人帐户
		/// </summary>
		public string ReleaseAccount
		{
			set{ _releaseaccount=value;}
			get{return _releaseaccount;}
		}
		/// <summary>
		/// 发布人姓名
		/// </summary>
		public string ReleaseName
		{
			set{ _releasename=value;}
			get{return _releasename;}
		}
		/// <summary>
		/// 发布公司编号
		/// </summary>
		public string ReleaseCpyNo
		{
			set{ _releasecpyno=value;}
			get{return _releasecpyno;}
		}
		/// <summary>
		/// 发布公司名称
		/// </summary>
		public string ReleaseCpyName
		{
			set{ _releasecpyname=value;}
			get{return _releasecpyname;}
		}
		/// <summary>
		/// 公告状态1=已审，2=未审
		/// </summary>
		public int CallBoardType
		{
			set{ _callboardtype=value;}
			get{return _callboardtype;}
		}
		/// <summary>
		/// 审核时间
		/// </summary>
		public DateTime AuditTime
		{
			set{ _audittime=value;}
			get{return _audittime;}
		}
		/// <summary>
		/// 审核人帐户
		/// </summary>
		public string AuditAccount
		{
			set{ _auditaccount=value;}
			get{return _auditaccount;}
		}
		/// <summary>
		/// 审核人姓名
		/// </summary>
		public string AuditAccountName
		{
			set{ _auditaccountname=value;}
			get{return _auditaccountname;}
		}
		/// <summary>
		/// 紧急标志1=紧急(登录后弹屏)，2=不紧急
		/// </summary>
		public int Emergency
		{
			set{ _emergency=value;}
			get{return _emergency;}
		}
		/// <summary>
		/// 滚动标志 1=不滚动， 2=滚动
		/// </summary>
		public int RollFlag
		{
			set{ _rollflag=value;}
			get{return _rollflag;}
		}
		/// <summary>
		/// 内部标志 1.内部，2.外部，3.全部
		/// </summary>
		public int IsInternal
		{
			set{ _isinternal=value;}
			get{return _isinternal;}
		}
		/// <summary>
		/// 点击次数
		/// </summary>
		public int ClickCount
		{
			set{ _clickcount=value;}
			get{return _clickcount;}
		}
		/// <summary>
		/// 生效日期
		/// </summary>
		public DateTime StartDate
		{
			set{ _startdate=value;}
			get{return _startdate;}
		}
		/// <summary>
		/// 失效日期
		/// </summary>
		public DateTime ExpirationDate
		{
			set{ _expirationdate=value;}
			get{return _expirationdate;}
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
		/// 附件名称
		/// </summary>
		public string AttachmentFileName
		{
			set{ _attachmentfilename=value;}
			get{return _attachmentfilename;}
		}
		/// <summary>
		/// 附件文件内容
		/// </summary>
		public byte[] FileAttachment
		{
			set{ _fileattachment=value;}
			get{return _fileattachment;}
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
		public decimal A3
		{
			set{ _a3=value;}
			get{return _a3;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal A4
		{
			set{ _a4=value;}
			get{return _a4;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime A5
		{
			set{ _a5=value;}
			get{return _a5;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime A6
		{
			set{ _a6=value;}
			get{return _a6;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A7
		{
			set{ _a7=value;}
			get{return _a7;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A8
		{
			set{ _a8=value;}
			get{return _a8;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A9
		{
			set{ _a9=value;}
			get{return _a9;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string A10
		{
			set{ _a10=value;}
			get{return _a10;}
		}
		#endregion Model

	}
}

