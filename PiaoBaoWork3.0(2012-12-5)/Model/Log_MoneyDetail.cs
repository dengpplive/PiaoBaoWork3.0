using System;
namespace PbProject.Model
{
	/// <summary>
	/// Log_MoneyDetail:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Log_MoneyDetail
	{
		public Log_MoneyDetail()
		{}
		#region Model
		private Guid _id;
		private string _orderid="";
		private string _inpayno="";
		private string _payno="";
		private int _paytype=0;
		private string _paycpyno="";
		private int _paycpytype=0;
		private string _paycpyname="";
		private string _reccpyno="";
		private int _reccpytype=0;
		private string _reccpyname="";
		private DateTime _opertime= DateTime.Now;
		private string _operloginname="";
		private string _operusername="";
		private string _operreason="";
		private string _remark="";
		private decimal _preremainmoney=0M;
		private decimal _paymoney=0M;
		private decimal _remainmoney=0M;
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
		/// 订单编号
		/// </summary>
		public string OrderId
		{
			set{ _orderid=value;}
			get{return _orderid;}
		}
		/// <summary>
		/// 内部流水号
		/// </summary>
		public string InPayNo
		{
			set{ _inpayno=value;}
			get{return _inpayno;}
		}
		/// <summary>
		/// 支付流水号
		/// </summary>
		public string PayNo
		{
			set{ _payno=value;}
			get{return _payno;}
		}
		/// <summary>
        /// PayWay 支付方式：1支付宝、2快钱、3汇付、4财付通、5支付宝网银、6快钱网银 、7汇付网银、8财付通网银、9支付宝pos、10快钱pos、
        /// 11汇付pos、12财付通pos、13易宝pos、14账户支付、15收银、 20 欠款日志、21 销账日志
		/// </summary>
		public int PayType
		{
			set{ _paytype=value;}
			get{return _paytype;}
		}
		/// <summary>
		/// 付款方公司编号
		/// </summary>
		public string PayCpyNo
		{
			set{ _paycpyno=value;}
			get{return _paycpyno;}
		}
		/// <summary>
		/// 付款方公司类型
		/// </summary>
		public int PayCpyType
		{
			set{ _paycpytype=value;}
			get{return _paycpytype;}
		}
		/// <summary>
		/// 付款方公司名称
		/// </summary>
		public string PayCpyName
		{
			set{ _paycpyname=value;}
			get{return _paycpyname;}
		}
		/// <summary>
		/// 收款方公司编号
		/// </summary>
		public string RecCpyNo
		{
			set{ _reccpyno=value;}
			get{return _reccpyno;}
		}
		/// <summary>
		/// 收款方公司类型
		/// </summary>
		public int RecCpyType
		{
			set{ _reccpytype=value;}
			get{return _reccpytype;}
		}
		/// <summary>
		/// 收款方公司名称
		/// </summary>
		public string RecCpyName
		{
			set{ _reccpyname=value;}
			get{return _reccpyname;}
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
		/// 操作人登录名
		/// </summary>
		public string OperLoginName
		{
			set{ _operloginname=value;}
			get{return _operloginname;}
		}
		/// <summary>
		/// 操作人名称
		/// </summary>
		public string OperUserName
		{
			set{ _operusername=value;}
			get{return _operusername;}
		}
		/// <summary>
		/// 调整原因或操作描述
		/// </summary>
		public string OperReason
		{
			set{ _operreason=value;}
			get{return _operreason;}
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
		/// 之前账户余额
		/// </summary>
		public decimal PreRemainMoney
		{
			set{ _preremainmoney=value;}
			get{return _preremainmoney;}
		}
		/// <summary>
		/// 发生金额
		/// </summary>
		public decimal PayMoney
		{
			set{ _paymoney=value;}
			get{return _paymoney;}
		}
		/// <summary>
		/// 账户余额
		/// </summary>
		public decimal RemainMoney
		{
			set{ _remainmoney=value;}
			get{return _remainmoney;}
		}
		/// <summary>
		/// 备用：0 默认支付成功, 1 撤销退款
		/// </summary>
		public int A1
		{
			set{ _a1=value;}
			get{return _a1;}
		}
		/// <summary>
        ///  备用: 0.其它、1.支付、2.退款、3.充值、4.上调、5.下调、6.分润、7.欠款明细 、8.欠款销账 、9.撤销充值
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
		/// <summary>
		/// 备用
		/// </summary>
		public string A9
		{
			set{ _a9=value;}
			get{return _a9;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public string A10
		{
			set{ _a10=value;}
			get{return _a10;}
		}
		#endregion Model

	}
}

