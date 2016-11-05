using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_Ticket_TakeOffDetail:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_Ticket_TakeOffDetail
	{
		public Tb_Ticket_TakeOffDetail()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _cpyname="";
		private int _cpytype=0;
		private DateTime _opertime= DateTime.Now;
		private string _operloginname="";
		private string _operusername="";
		private string _groupid="";
		private int _basetype=0;
		private string _policysource="";
		private string _carrycode="";
		private string _fromcitycode="";
		private string _tocitycode="";
		private string _timescope="";
		private string _pointscope="";
		private int _selecttype=0;
		private decimal _point=0M;
		private decimal _money=0M;
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
		/// 统一扣点组ID
		/// </summary>
		public string GroupId
		{
			set{ _groupid=value;}
			get{return _groupid;}
		}
		/// <summary>
		/// 类型（0=全部,1=本地，2=竞价平台，3=共享）
		/// </summary>
		public int BaseType
		{
			set{ _basetype=value;}
			get{return _basetype;}
		}
		/// <summary>
		/// 竞价平台序号
		/// </summary>
		public string PolicySource
		{
			set{ _policysource=value;}
			get{return _policysource;}
		}
		/// <summary>
		/// 承运人代码（所有承运人：ALL）
		/// </summary>
		public string CarryCode
		{
			set{ _carrycode=value;}
			get{return _carrycode;}
		}
		/// <summary>
		/// 出发城市代码（多个城市以“/”分隔，所有城市：ALL）
		/// </summary>
		public string FromCityCode
		{
			set{ _fromcitycode=value;}
			get{return _fromcitycode;}
		}
		/// <summary>
		/// 出发城市代码（多个城市以“/”分隔，所有城市：ALL）
		/// </summary>
		public string ToCityCode
		{
			set{ _tocitycode=value;}
			get{return _tocitycode;}
		}
		/// <summary>
		/// 时间范围
		/// </summary>
		public string TimeScope
		{
			set{ _timescope=value;}
			get{return _timescope;}
		}
		/// <summary>
		/// 返点范围
		/// </summary>
		public string PointScope
		{
			set{ _pointscope=value;}
			get{return _pointscope;}
		}
		/// <summary>
		/// 调整类型（1=扣点，2=留点,3=本地政策补点）
		/// </summary>
		public int SelectType
		{
			set{ _selecttype=value;}
			get{return _selecttype;}
		}
		/// <summary>
		/// 点数（百分比）
		/// </summary>
		public decimal Point
		{
			set{ _point=value;}
			get{return _point;}
		}
		/// <summary>
		/// 返现金额
		/// </summary>
		public decimal Money
		{
			set{ _money=value;}
			get{return _money;}
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

