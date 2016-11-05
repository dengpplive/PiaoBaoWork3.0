using System;
namespace PbProject.Model
{
	/// <summary>
	/// Tb_PosInfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Tb_PosInfo
	{
		public Tb_PosInfo()
		{}
		#region Model
		private Guid _id;
		private string _cpyno="";
		private string _cpyname="";
        private string _cpyloginname = "";

		private int _cpytype=0;
		private DateTime _opertime= DateTime.Now;
		private string _opercpyno="";
		private string _opercpyname="";
		private string _operloginname="";
		private string _operusername="";
		private int _posmode=0;
		private string _posno="";
        private double _posrate = 0.0;
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
		/// 公司登录账号 :支付宝 pos 使用
		/// </summary>
        public string CpyLoginName
		{
            set { _cpyloginname = value; }
            get { return _cpyloginname; }
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
		/// 操作员所属公司编号
		/// </summary>
		public string OperCpyNo
		{
			set{ _opercpyno=value;}
			get{return _opercpyno;}
		}
		/// <summary>
		/// 操作员所属公司名称
		/// </summary>
		public string OperCpyName
		{
			set{ _opercpyname=value;}
			get{return _opercpyname;}
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
		/// POS机类型：支付宝、快钱、易宝等
		/// </summary>
		public int PosMode
		{
			set{ _posmode=value;}
			get{return _posmode;}
		}
		/// <summary>
		/// POS机编号
		/// </summary>
		public string PosNo
		{
			set{ _posno=value;}
			get{return _posno;}
		}
        /// <summary>
        /// POS费率
        /// </summary>
        public double PosRate
        {
            set { _posrate = value; }
            get { return _posrate; }
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
		#endregion Model

	}
}

