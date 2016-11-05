using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiaoBao.API.Common.Enum
{
    /// <summary>
    /// 行程类型
    /// </summary>
    public enum AirTravelType
    {
        /// <summary>
        /// 单程
        /// </summary>
        OneWay = 1,
        /// <summary>
        /// 往返
        /// </summary>
        RoundWay = 2,
        /// <summary>
        /// 联程
        /// </summary>
        ConnWay = 3
    }
}