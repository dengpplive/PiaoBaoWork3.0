using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace PbProject.Model
{
    /// <summary>
    /// 获取政策匹配中传过来的政策数据
    /// </summary>
    [Serializable]
    public class AjaxPolicyMatchOutData
    {
        private List<AjAxPolicyParam> _OutPutPolicyList = new List<AjAxPolicyParam>();
        /// <summary>
        /// 政策数据列表
        /// </summary>
        public List<AjAxPolicyParam> OutPutPolicyList
        {
            get { return _OutPutPolicyList; }
            set { _OutPutPolicyList = value; }
        }

        private string _PolicyErrMsg = string.Empty;
        /// <summary>
        /// 没有获取到政策 或者政策出错时的提示信息 默认为空
        /// </summary>
        public string PolicyErrMsg
        {
            get { return _PolicyErrMsg; }
            set { _PolicyErrMsg = value; }
        }
        /// <summary>
        /// 政策字符串显示数据 还有日志
        /// </summary>
        /// <returns></returns>
        public string ToString(string OrderId)
        {
            StringBuilder sbLog = new StringBuilder();
            try
            {
                sbLog.AppendFormat("【时间:{0} 订单号:{1}】=======================================================\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), OrderId);
                foreach (AjAxPolicyParam model in _OutPutPolicyList)
                {
                    sbLog.Append("\r\n政策数据:\r\n");
                    List<string> PropertyList = new List<string>();
                    Type t = model.GetType();
                    PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                    object obj = null;
                    foreach (PropertyInfo p in properties)
                    {
                        obj = p.GetValue(model, null);
                        PropertyList.Add(p.Name + "=" + (obj == null ? "null" : obj));
                    }
                    if (PropertyList.Count > 0)
                    {
                        sbLog.Append(string.Join("\r\n", PropertyList.ToArray()));
                    }
                }
                sbLog.Append("结束=======================================================\r\n");
            }
            catch (Exception)
            {
            }
            return sbLog.ToString();
        }
    }
    /// <summary>
    ///确认页面需要的一条政策数据实体 可扩展添加
    /// </summary>
    [Serializable]
    public class AjAxPolicyParam
    {
        private string _CarryCode = string.Empty;
        /// <summary>
        /// 航空公司二字码 CA
        /// </summary>
        public string CarryCode
        {
            get { return _CarryCode; }
            set { _CarryCode = value; }
        }

        private string _Space = string.Empty;
        /// <summary>
        /// 舱位
        /// </summary>
        public string Space
        {
            get { return _Space; }
            set { _Space = value; }
        }

        private decimal _DiscountRate = 0m;
        /// <summary>
        /// 折扣 特价政策为-1
        /// </summary>
        public decimal DiscountRate
        {
            get { return _DiscountRate; }
            set { _DiscountRate = value; }
        }
        private string _PolicyId = string.Empty;
        /// <summary>
        /// 政策ID
        /// </summary>
        public string PolicyId
        {
            get { return _PolicyId; }
            set { _PolicyId = value; }
        }
        private decimal _AirPoint = 0m;
        /// <summary>
        /// 航空公司返点
        /// </summary>
        public decimal AirPoint
        {
            get { return _AirPoint; }
            set { _AirPoint = value; }
        }
        private decimal _AirReturnMoney = 0m;
        /// <summary>
        /// 航空公司现返
        /// </summary>
        public decimal AirReturnMoney
        {
            get { return _AirReturnMoney; }
            set { _AirReturnMoney = value; }
        }


        private decimal _PolicyPoint = 0;
        /// <summary>
        /// 出票政策返点 用于计算
        /// </summary>
        public decimal PolicyPoint
        {
            get { return _PolicyPoint; }
            set { _PolicyPoint = value; }
        }
        private decimal _OldPolicyPoint = 0;
        /// <summary>
        /// 原始政策返点 用来订单显示
        /// </summary>
        public decimal OldPolicyPoint
        {
            get { return _OldPolicyPoint; }
            set { _OldPolicyPoint = value; }
        }
        private decimal _PolicyReturnMoney = 0m;

        /// <summary>
        /// 出票政策现返 用于计算
        /// </summary>
        public decimal PolicyReturnMoney
        {
            get { return _PolicyReturnMoney; }
            set { _PolicyReturnMoney = value; }
        }

        private decimal _OldPolicyReturnMoney = 0m;

        /// <summary>
        /// 原始政策现返 用于订单显示
        /// </summary>
        public decimal OldPolicyReturnMoney
        {
            get { return _OldPolicyReturnMoney; }
            set { _OldPolicyReturnMoney = value; }
        }

        private decimal _ReturnPoint = 0m;
        /// <summary>
        /// 实际返点（扣点后） 用户最终看到的政策点数(分销或者采购) 
        /// </summary>
        public decimal ReturnPoint
        {
            get { return _ReturnPoint; }
            set { _ReturnPoint = value; }
        }
        private decimal _PolicyYongJin = 0m;
        /// <summary>
        /// 政策佣金 
        /// </summary>
        public decimal PolicyYongJin
        {
            get { return _PolicyYongJin; }
            set { _PolicyYongJin = value; }
        }

        private decimal _PolicyShiFuMoney = 0m;
        /// <summary>
        ///政策实付金额 
        /// </summary>
        public decimal PolicyShiFuMoney
        {
            get { return _PolicyShiFuMoney; }
            set { _PolicyShiFuMoney = value; }
        }
        private decimal _LaterPoint = 0m;
        /// <summary>
        /// 政策后返点
        /// </summary>
        public decimal LaterPoint
        {
            get { return _LaterPoint; }
            set { _LaterPoint = value; }
        }


        private decimal _SeatPrice = 0m;
        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice
        {
            get { return _SeatPrice; }
            set { _SeatPrice = value; }
        }
        private decimal _ABFare = 0m;
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal ABFare
        {
            get { return _ABFare; }
            set { _ABFare = value; }
        }
        private decimal _RQFare = 0m;
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal RQFare
        {
            get { return _RQFare; }
            set { _RQFare = value; }
        }

        private string _DiscountDetail = "0^0^0";
        /// <summary>
        /// 扣点明细(用户1^扣点点数^现返|用户2^扣点点数^现返|……)（a^0.1^1|）
        /// </summary>
        public string DiscountDetail
        {
            get { return _DiscountDetail; }
            set { _DiscountDetail = value; }
        }

        private string _PolicyRemark = string.Empty;
        /// <summary>
        /// 政策备注
        /// </summary>
        public string PolicyRemark
        {
            get { return _PolicyRemark; }
            set { _PolicyRemark = value; }
        }

        private string _JinriGYCode = string.Empty;
        /// <summary>
        /// 今日供应商ID
        /// </summary>
        public string JinriGYCode
        {
            get { return _JinriGYCode; }
            set { _JinriGYCode = value; }
        }

        private int _PolicyKind = 0;
        /// <summary>
        /// 政策种类  0.通用， 1.普通，2.特价
        /// </summary>
        public int PolicyKind
        {
            get
            {
                return _PolicyKind;

            }
            set
            {
                _PolicyKind = value;
            }
        }
        private string _PolicyType = "1";
        /// <summary>
        /// 政策类型 1=B2B，2=BSP，3=B2B/BSP
        /// </summary>
        public string PolicyType
        {
            get { return _PolicyType; }
            set { _PolicyType = value; }
        }
        //自动出票方式 手动(0或者null空)， 半自动1， 自动2
        private string _AutoPrintFlag = "0";
        public string AutoPrintFlag
        {
            get { return _AutoPrintFlag; }
            set { _AutoPrintFlag = value; }
        }
        private string _PolicySource = "1";
        /// <summary>
        /// 默认1  1=本地B2B, 2=本地BSP,3=517,4=百拓,5=8000翼,6=今日,7=票盟,8=51book,9=共享
        /// </summary>
        public string PolicySource
        {
            get { return _PolicySource; }
            set { _PolicySource = value; }
        }
        private string _PolicyOffice = string.Empty;
        /// <summary>
        /// 出票Office
        /// </summary>
        public string PolicyOffice
        {
            get { return _PolicyOffice; }
            set { _PolicyOffice = value; }
        }

        private string _CPCpyNo = string.Empty;
        /// <summary>
        /// 出票公司编号
        /// </summary>
        public string CPCpyNo
        {
            get
            {
                return _CPCpyNo;
            }
            set
            {
                _CPCpyNo = value;
            }
        }

        private string _DefaultType = "0";
        /// <summary>
        /// 默认0不是默认政策 1成人默认政策 2儿童默认政策
        /// </summary>
        public string DefaultType
        {
            get { return _DefaultType; }
            set { _DefaultType = value; }
        }

        private string _HighPolicyFlag = "0";
        /// <summary>
        /// 是否高返政策 1是 0否 默认0
        /// </summary>
        public string HighPolicyFlag
        {
            get { return _HighPolicyFlag; }
            set { _HighPolicyFlag = value; }
        }

        private string _WorkTime = "00:00-00:00";
        /// <summary>
        /// 正常上班时间00:00-00:00
        /// </summary>
        public string WorkTime
        {
            get { return _WorkTime; }
            set { _WorkTime = value; }
        }
        private string _PolicyCancelTime = "00:00-00:00";
        /// <summary>
        /// 废票时间
        /// </summary>
        public string PolicyCancelTime
        {
            get { return _PolicyCancelTime; }
            set { _PolicyCancelTime = value; }
        }
        private string _PolicyReturnTime = "00:00-00:00";
        /// <summary>
        /// 退票时间
        /// </summary>
        public string PolicyReturnTime
        {
            get { return _PolicyReturnTime; }
            set { _PolicyReturnTime = value; }
        }
        private string _FPGQTime = string.Empty;
        /// <summary>
        /// 废票改签时间 00:00-00:00
        /// </summary>
        public string FPGQTime
        {
            get { return _FPGQTime; }
            set { _FPGQTime = value; }
        }
        private string _chuPiaoShiJian = string.Empty;
        /// <summary>
        /// 出票时间
        /// 0-B2B|1-BSP|2-517|3-百拓|4-8000|5-今日|6-票盟|7-51book|8-共享|9-易行
        /// </summary>
        public string chuPiaoShiJian
        {
            get { return _chuPiaoShiJian; }
            set { _chuPiaoShiJian = value; }
        }

        private string _PayType = "";
        /// <summary>
        /// 支付方式 0-网银|1-支付宝|2-快钱|3-汇付|4-财付通|5-账户支付|6-收银  
        /// </summary>
        public string PayType
        {
            get { return _PayType; }
            set { _PayType = value; }
        }

        private decimal _PatchPonit = 0M;
        /// <summary>
        /// 补点值 
        /// </summary>
        public decimal PatchPonit
        {
            get { return _PatchPonit; }
            set { _PatchPonit = value; }
        }

        private decimal _AirPayMoney = 0m;
        /// <summary>
        /// //B2B航空公司政策支付金额
        /// </summary>
        public decimal AirPayMoney
        {
            get
            {
                return _AirPayMoney;
            }
            set
            {
                _AirPayMoney = value;
            }
        }
    }
}
