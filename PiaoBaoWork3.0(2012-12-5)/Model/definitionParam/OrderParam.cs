using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model.definitionParam
{
    [Serializable]
    public class OrderParam
    {
        ///// <param name="oldpolicy">原始政策</param>
        ///// <param name="policy">扣点后的政策</param>  
        ///// <param name="bxfee">保险</param>
        ///// <param name="hfrate">接口费率</param>
        ///// <param name="gyrate">供应收款费率</param>
        ///// <param name="sendfee">配送费</param>
        ///// <param name="sxfee">销售价</param>
        ///// <param name="pmfee">票面价</param>
        ///// <param name="jjfee">基建</param>
        ///// <param name="ryfee">燃油</param>
        ///// <param name="xffee">现返</param>
        ///// <param name="mOrder">订单表model</param>
        ///// <param name="mPassenger">乘客表model</param>

        private decimal _Oldpolicy = 0M; //原始政策
        private decimal _NewPolicy = 0M; //扣点后的政策
        private decimal _GyRate = 0M; //收款费率
        private decimal _JkRate = 0M; //代付接口费率
        private decimal _PmFee = 0M; //票面价
        private decimal _CwFee = 0M; //舱位价
        private decimal _JjFee = 0M; //基建
        private decimal _RyFee = 0M; //燃油
        private string _KdDetails = string.Empty; //扣点明细



    }
}
