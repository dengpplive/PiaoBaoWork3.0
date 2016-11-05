using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model.definitionParam
{
    [Serializable]
    public class PolicySourceParam
    {
        //订单来源：1 本地B2B, 2 本地BSP,3 517,4 百拓,5 8000翼,6 今日,7 票盟,8 51book ,9 共享,10易行
        /// <summary>
        /// 1 B2B
        /// </summary>
        public static readonly int B2B = 1;
        /// <summary>
        /// 2 BSP
        /// </summary>
        public static readonly int BSP = 2;
        /// <summary>
        /// 3 517
        /// </summary>
        public static readonly int b517na = 3;
        /// <summary>
        /// 4 百拓
        /// </summary>
        public static readonly int baiTuo = 4;
        /// <summary>
        /// 5 8000翼
        /// </summary>
        public static readonly int b8000yi = 5;
        /// <summary>
        /// 6 今日
        /// </summary>
        public static readonly int bToday = 6;
        /// <summary>
        /// 7 票盟
        /// </summary>
        public static readonly int bPiaoMeng = 7;
        /// <summary>
        /// 8 51book
        /// </summary>
        public static readonly int b51book = 8;
        /// <summary>
        /// 9 共享
        /// </summary>
        public static readonly int bGX = 9;
        /// <summary>
        /// 10 易行
        /// </summary>
        public static readonly int bYeeXing = 10;
    }
}
