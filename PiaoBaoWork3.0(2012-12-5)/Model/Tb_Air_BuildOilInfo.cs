using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_Air_BuildOilInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_Air_BuildOilInfo
    {
        public Tb_Air_BuildOilInfo()
        { }
        #region Model
        private Guid _id;
        private string _fromcitycode;
        private string _tocitycode;
        private int _persontype;
        private decimal _buildprice;
        private decimal _oilprice;
        private string _a1;
        private string _a2;
        private string _a3;
        private string _a4;
        private string _a5;
        /// <summary>
        /// 
        /// </summary>
        public Guid id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCode
        {
            set { _fromcitycode = value; }
            get { return _fromcitycode; }
        }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCode
        {
            set { _tocitycode = value; }
            get { return _tocitycode; }
        }
        /// <summary>
        /// 乘机人类型（1：成人；2：儿童）
        /// </summary>
        public int PersonType
        {
            set { _persontype = value; }
            get { return _persontype; }
        }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal BuildPrice
        {
            set { _buildprice = value; }
            get { return _buildprice; }
        }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal OilPrice
        {
            set { _oilprice = value; }
            get { return _oilprice; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public string A1
        {
            set { _a1 = value; }
            get { return _a1; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public string A2
        {
            set { _a2 = value; }
            get { return _a2; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public string A3
        {
            set { _a3 = value; }
            get { return _a3; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public string A4
        {
            set { _a4 = value; }
            get { return _a4; }
        }
        /// <summary>
        /// 备用
        /// </summary>
        public string A5
        {
            set { _a5 = value; }
            get { return _a5; }
        }
        #endregion Model

    }
}

