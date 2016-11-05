using System;
namespace PbProject.Model
{
    /// <summary>
    /// Tb_SpecialCabin_PriceInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class Tb_SpecialCabin_PriceInfo
    {
        public Tb_SpecialCabin_PriceInfo()
        { }
        #region Model
        private Guid _id;
        private string _spaircode;
        private string _spflightcode;
        private string _spcabin;
        private string _fromcitycode;
        private string _tocitycode;
        private DateTime _flighttime;
        private DateTime _updatetime = DateTime.Now;
        private decimal _spprice;
        private decimal _spabfare;
        private decimal _sprqfare;

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
        /// 特价航空公司二字码 CA
        /// </summary>
        public string SpAirCode
        {
            set { _spaircode = value; }
            get { return _spaircode; }
        }
        /// <summary>
        /// 航班号 1436
        /// </summary>
        public string SpFlightCode
        {
            get
            {
                return _spflightcode;
            }
            set
            {
                _spflightcode = value;
            }
        }
        /// <summary>
        /// 特价航空公司舱位 Y
        /// </summary>
        public string SpCabin
        {
            set { _spcabin = value; }
            get { return _spcabin; }
        }
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string FromCityCode
        {
            set { _fromcitycode = value; }
            get { return _fromcitycode; }
        }
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string ToCityCode
        {
            set { _tocitycode = value; }
            get { return _tocitycode; }
        }
        /// <summary>
        /// 航班起飞日期时间
        /// </summary>
        public DateTime FlightTime
        {
            set { _flighttime = value; }
            get { return _flighttime; }
        }
        /// <summary>
        /// 添加更新时间 缓存时间
        /// </summary>
        public DateTime UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }
        /// <summary>
        /// 特价价格
        /// </summary>
        public decimal SpPrice
        {
            set { _spprice = value; }
            get { return _spprice; }
        }
        /// <summary>
        /// 基建
        /// </summary>
        public decimal SpABFare
        {

            set { _spabfare = value; }
            get { return _spabfare; }
        }
        /// <summary>
        /// 燃油
        /// </summary>
        public decimal SpRQFare
        {

            set { _sprqfare = value; }
            get { return _sprqfare; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string A1
        {
            set { _a1 = value; }
            get { return _a1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A2
        {
            set { _a2 = value; }
            get { return _a2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A3
        {
            set { _a3 = value; }
            get { return _a3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A4
        {
            set { _a4 = value; }
            get { return _a4; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string A5
        {
            set { _a5 = value; }
            get { return _a5; }
        }
        #endregion Model

    }
}

