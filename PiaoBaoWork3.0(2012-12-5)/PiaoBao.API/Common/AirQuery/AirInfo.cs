using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiaoBao.API.Common.AirQuery
{
    public class AirInfo
    {
        /// <summary>
        /// 承运人编码
        /// </summary>
        public string CarrCode { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string Carrier { get; set; }
        /// <summary>
        /// 航班编号
        /// </summary>
        public string FlightNo { get; set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCity { get; set; }
        /// <summary>
        /// 到达时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCity { get; set; }
        /// <summary>
        /// 机型
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 是否停飞
        /// </summary>
        public string IsStop { get; set; }
        /// <summary>
        /// 机建
        /// </summary>
        public string ABFee { get; set; }
        /// <summary>
        /// 燃油
        /// </summary>
        public string FuelAdultFee { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// 舱位数量
        /// </summary>
        public string TickNum { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public string DiscountRate { get; set; }
        /// <summary>
        /// 舱位价
        /// </summary>
        public string XSFee { get; set; }
        /// <summary>
        /// 票面价
        /// </summary>
        public string PMFee { get; set; }
        /// <summary>
        /// 客规
        /// </summary>
        public string DishonoredBillPrescript { get; set; }
        public string LogChangePrescript { get; set; }
        public string UpCabinPrescript { get; set; }
        /// <summary>
        /// 特价类型(0正常，1特价)
        /// </summary>
        public string SpecialType { get; set; }
        public string aterm { get; set; }
        public string GUID { get; set; }
        public string PolicyGUID { get; set; }
        /// <summary>
        /// 是否高返
        /// </summary>
        public string HighPolicyFlag { get; set; }
        public string LaterPoint { get; set; }
        /// <summary>
        /// 政策佣金
        /// </summary>
        public string Policy { get; set; }
        public string ReturnMoney { get; set; }
        public string Commission { get; set; }
        /// <summary>
        /// 实付金额
        /// </summary>
        public string SJFee { get; set; }
        /// <summary>
        /// 特价类型(1正常，2动态特价，3固定特价)
        /// </summary>
        public string GenerationType { get; set; }
        /// <summary>
        /// 特价
        /// </summary>
        public string SeatPrice { get; set; }
       
    }
}