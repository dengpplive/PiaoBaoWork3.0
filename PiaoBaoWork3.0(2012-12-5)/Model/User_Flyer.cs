using System;
namespace PbProject.Model
{
	/// <summary>
	/// User_Flyer:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class User_Flyer
	{
		public User_Flyer()
		{}
		#region Model
		private Guid _id;
		private string _memberaccount="";
		private string _remainwithid="";
		private string _name="";
		private string _certificatenum="";
		private int _certificatetype=1;
		private string _tel="";
		private string _cpyno="";
		private int _sex=0;
		private int _flyertype=1;
		private string _remark="";
		private DateTime _brontime= DateTime.Now;
		private string _cpyandno="";
		private int _a1=0;
		private decimal _a2=0M;
		private DateTime _a3= DateTime.Now;
		private string _a4="";
		private string _a5="";
		/// <summary>
		/// 常旅客表Id
		/// </summary>
		public Guid id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 归属人类型
		/// </summary>
		public string MemberAccount
		{
			set{ _memberaccount=value;}
			get{return _memberaccount;}
		}
		/// <summary>
		/// 归属人Id
		/// </summary>
		public string RemainWithId
		{
			set{ _remainwithid=value;}
			get{return _remainwithid;}
		}
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 证件号
		/// </summary>
		public string CertificateNum
		{
			set{ _certificatenum=value;}
			get{return _certificatenum;}
		}
		/// <summary>
		/// 证件类型
		/// </summary>
		public int CertificateType
		{
			set{ _certificatetype=value;}
			get{return _certificatetype;}
		}
		/// <summary>
		/// 联系电话
		/// </summary>
		public string Tel
		{
			set{ _tel=value;}
			get{return _tel;}
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
		/// 男0女 1
		/// </summary>
		public int Sex
		{
			set{ _sex=value;}
			get{return _sex;}
		}
		/// <summary>
		/// 旅客类型（1成人2儿童3婴儿）
		/// </summary>
		public int Flyertype
		{
			set{ _flyertype=value;}
			get{return _flyertype;}
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
		/// 出生日期
		/// </summary>
		public DateTime BronTime
		{
			set{ _brontime=value;}
			get{return _brontime;}
		}
		/// <summary>
		/// 航空公司和卡号 CA,111|CZ,222
		/// </summary>
		public string CpyandNo
		{
			set{ _cpyandno=value;}
			get{return _cpyandno;}
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
		public decimal A2
		{
			set{ _a2=value;}
			get{return _a2;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public DateTime A3
		{
			set{ _a3=value;}
			get{return _a3;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public string A4
		{
			set{ _a4=value;}
			get{return _a4;}
		}
		/// <summary>
		/// 备用
		/// </summary>
		public string A5
		{
			set{ _a5=value;}
			get{return _a5;}
		}
		#endregion Model

	}
}

