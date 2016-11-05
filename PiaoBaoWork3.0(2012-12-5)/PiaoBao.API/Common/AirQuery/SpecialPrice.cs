using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiaoBao.API.Common.AirQuery
{
    public class SpecialPrice
    {
        /// <summary>
        /// 舱位价
        /// </summary>
        public string SpacePrice { get; set; }
        /// <summary>
        /// 基建费
        /// </summary>
        public string Tax { get; set; }
        /// <summary>
        /// 燃油费
        /// </summary>
        public string RQFare { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public string TotalPrice { get; set; }
        /// <summary>
        /// 实付金额
        /// </summary>
        public string RealPayPrice { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public string Commission { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public string Discount { get; set; }
    }
}