using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Sms_ReCharge:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Sms_ReCharge
	{
		public Tb_Sms_ReCharge()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _orderid="";
		private string _inpayno="";
		private string _payno="";
		private decimal _rechargemoney=0M;
		private int _rechargecount=0;
		private int _rechargestate=0;
		private DateTime _rechargedate= Convert.ToDateTime("1900-01-01");
		private DateTime _paydate= Convert.ToDateTime("1900-01-01");
		private int _paytype=0;
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
		/// 订单编号
		/// </summary>
		public string OrderId
		{
			set{ _orderid=value;}
			get{return _orderid;}
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
		/// 在线交易流水号
		/// </summary>
		public string PayNo
		{
			set{ _payno=value;}
			get{return _payno;}
		}
		/// <summary>
		/// 金额
		/// </summary>
		public decimal ReChargeMoney
		{
			set{ _rechargemoney=value;}
			get{return _rechargemoney;}
		}
		/// <summary>
		/// 条数
		/// </summary>
		public int ReChargeCount
		{
			set{ _rechargecount=value;}
			get{return _rechargecount;}
		}
		/// <summary>
		/// 状态 0=未付，1=已付未确认，2=已付已确认
		/// </summary>
		public int ReChargeState
		{
			set{ _rechargestate=value;}
			get{return _rechargestate;}
		}
		/// <summary>
		/// 创建日期
		/// </summary>
		public DateTime ReChargeDate
		{
			set{ _rechargedate=value;}
			get{return _rechargedate;}
		}
		/// <summary>
		/// 支付日期
		/// </summary>
		public DateTime PayDate
		{
			set{ _paydate=value;}
			get{return _paydate;}
		}
		/// <summary>
		/// 支付方式（见字典表）
		/// </summary>
		public int PayType
		{
			set{ _paytype=value;}
			get{return _paytype;}
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

