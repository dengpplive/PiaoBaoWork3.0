using System;
namespace PbProject.Model
{
	/// <summary>
	/// Bd_Air_Aircraft:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Bd_Air_Aircraft
	{
		public Bd_Air_Aircraft()
		{}
		#region Model
		private Guid _id;
		private decimal _abfeen=0M;
		private decimal _abfeew=0M;
		private string _aircraft="";
		private string _remark="";
		private int _a1=0;
		private decimal _a2=0M;
		private DateTime _a3= Convert.ToDateTime("1900-01-01");
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
		/// 国内机建
		/// </summary>
		public decimal ABFeeN
		{
			set{ _abfeen=value;}
			get{return _abfeen;}
		}
		/// <summary>
		/// 国外机建
		/// </summary>
		public decimal ABFeeW
		{
			set{ _abfeew=value;}
			get{return _abfeew;}
		}
		/// <summary>
		/// 机型
		/// </summary>
		public string Aircraft
		{
			set{ _aircraft=value;}
			get{return _aircraft;}
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

