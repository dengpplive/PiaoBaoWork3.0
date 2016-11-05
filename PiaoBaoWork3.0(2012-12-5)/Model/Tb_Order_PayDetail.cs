using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Order_PayDetail:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Order_PayDetail
	{
		public Tb_Order_PayDetail()
		{}
		#region Model
		private Guid _id;
		private string _orderid="";
		private string _paytype="";
		private int _paymode=0;
		private string _cpyno="";
		private int _cpytype=0;
		private string _cpyname="";
		private decimal _buypoundage=0M;
		private decimal _paymoney=0M;
		private decimal _realpaymoney=0M;
		private string _inpayno="";
		private string _payno="";
		private string _returnpayno="";
		private string _payaccount="";
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
		/// 1付款，2 收款 3 分账 4 手续费
		/// </summary>
		public string PayType
		{
			set{ _paytype=value;}
			get{return _paytype;}
		}
		/// <summary>
		/// 支付方式（支付宝、预存款等）
		/// </summary>
		public int PayMode
		{
			set{ _paymode=value;}
			get{return _paymode;}
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
		/// 公司类型
		/// </summary>
		public int CpyType
		{
			set{ _cpytype=value;}
			get{return _cpytype;}
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
		/// 交易手续费
		/// </summary>
		public decimal BuyPoundage
		{
			set{ _buypoundage=value;}
			get{return _buypoundage;}
		}
		/// <summary>
		/// 交易金额（应收应付）
		/// </summary>
		public decimal PayMoney
		{
			set{ _paymoney=value;}
			get{return _paymoney;}
		}
		/// <summary>
		/// 实际交易金额（实收实付）
		/// </summary>
		public decimal RealPayMoney
		{
			set{ _realpaymoney=value;}
			get{return _realpaymoney;}
		}
		/// <summary>
		/// 内部交易流水号
		/// </summary>
		public string InPayNo
		{
			set{ _inpayno=value;}
			get{return _inpayno;}
		}
		/// <summary>
		/// 支付交易号
		/// </summary>
		public string PayNo
		{
			set{ _payno=value;}
			get{return _payno;}
		}
		/// <summary>
		/// 退款交易号
		/// </summary>
		public string ReturnPayNo
		{
			set{ _returnpayno=value;}
			get{return _returnpayno;}
		}
		/// <summary>
		/// 收支帐号
		/// </summary>
		public string PayAccount
		{
			set{ _payaccount=value;}
			get{return _payaccount;}
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

