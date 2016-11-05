using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model
{
    /// <summary>
    /// 航班查询传过来的参数信息
    /// </summary>
    /// 
    [Serializable]
    public class FlightQueryParam
    {

        private string m_KongZhiXiTong = string.Empty;
        /// <summary>
        /// 控制系统权限
        /// </summary>
        public string KongZhiXiTong
        {
            get
            {
                return m_KongZhiXiTong;
            }
            set
            {
                m_KongZhiXiTong = value;
            }
        }

        private string m_GongYingKongZhiFenXiao = string.Empty;
        /// <summary>
        /// 供应控制分销开关 
        /// </summary>
        public string GongYingKongZhiFenXiao
        {
            get
            {
                return m_GongYingKongZhiFenXiao;
            }
            set
            {
                m_GongYingKongZhiFenXiao = value;
            }
        }
        private string m_PasData = string.Empty;
        /// <summary>
        /// 乘客拼接字符串数据格式:乘客序号#####乘客姓名#####乘客类型#####证件号类型#####证件号码#####乘客手机#####是否常旅客@乘客序号#####乘客姓名#####乘客类型#####证件号类型#####证件号码#####乘客手机#####是否常旅客   
        /// </summary>
        public string PasData
        {
            get
            {
                return m_PasData;
            }
            set
            {
                m_PasData = value;
            }
        }

        private bool m_IsAsAdultOrder = false;
        /// <summary>
        /// 是否关联成人订单号 默认（不关联）false  true关联
        /// </summary>
        public bool IsAsAdultOrder
        {
            get
            {
                return m_IsAsAdultOrder;
            }
            set
            {
                m_IsAsAdultOrder = value;
            }
        }
        private bool m_AllowChangePNRFlag = false;
        /// <summary>
        /// 是否允许换编码出票 true允许 false不允许
        /// </summary>
        public bool AllowChangePNRFlag
        {
            get
            {
                return m_AllowChangePNRFlag;
            }
            set
            {
                m_AllowChangePNRFlag = value;
            }
        }

        private bool m_IsCHDToAudltTK = false;
        /// <summary>
        /// 是否儿童出成人票 true是 false否
        /// </summary>
        public bool IsCHDToAudltTK
        {
            get
            {
                return m_IsCHDToAudltTK;
            }
            set
            {
                m_IsCHDToAudltTK = value;
            }
        }

        private bool m_IsRobTicketOrder = false;
        /// <summary>
        /// 是否为抢票订单 true 是 false不是
        /// </summary>
        public bool IsRobTicketOrder
        {
            get
            {
                return m_IsRobTicketOrder;
            }
            set
            {
                m_IsRobTicketOrder = value;
            }
        }

        private string m_CHDAssociationAdultOrderId = string.Empty;
        /// <summary>
        /// 关联成人订单号
        /// </summary>
        public string CHDAssociationAdultOrderId
        {
            get
            {
                return m_CHDAssociationAdultOrderId;
            }
            set
            {
                m_CHDAssociationAdultOrderId = value;
            }
        }


        private string m_Carryer = string.Empty;
        /// <summary>
        /// 承运人 格式：3U^川航~3U^川航
        /// </summary>
        public string Carryer
        {
            get
            {
                return m_Carryer;
            }
            set
            {
                m_Carryer = value;
            }
        }

        private string m_FlyNo = string.Empty;
        /// <summary>
        /// 航班号 格式：8881~8548
        /// </summary>
        public string FlyNo
        {
            get
            {
                return m_FlyNo;
            }
            set
            {
                m_FlyNo = value;
            }
        }
        private string m_Aircraft = string.Empty;
        /// <summary>
        /// 机型 格式： 330~321
        /// </summary>
        public string Aircraft
        {
            get
            {
                return m_Aircraft;
            }
            set
            {
                m_Aircraft = value;
            }
        }
        private string m_Time = string.Empty;
        /// <summary>
        /// 起止时间 格式：  2013-1-07=07:30=10:05~2013-2-26=07:45=10:40
        /// </summary>
        public string Time
        {
            get
            {
                return m_Time;
            }
            set
            {
                m_Time = value;
            }
        }

        private string m_City = string.Empty;
        /// <summary>
        /// 起止城市 格式： CTU-PEK^成都-北京~PEK-CTU^北京-成都
        /// </summary>
        public string City
        {
            get
            {
                return m_City;
            }
            set
            {
                m_City = value;
            }
        }

        private string m_ABFee = "0";
        /// <summary>
        /// 基建 格式： 50.00~50.00
        /// </summary>
        public string ABFee
        {
            get
            {
                return m_ABFee;
            }
            set
            {
                m_ABFee = value;
            }
        }

        private string m_FuelAdultFee = "0";
        /// <summary>
        /// 燃油 格式：130~130
        /// </summary>
        public string FuelAdultFee
        {
            get
            {
                return m_FuelAdultFee;
            }
            set
            {
                m_FuelAdultFee = value;
            }
        }

        private string m_DiscountRate = string.Empty;
        /// <summary>
        /// 折扣 格式： 30~35
        /// </summary>
        public string DiscountRate
        {
            get
            {
                return m_DiscountRate;
            }
            set
            {
                m_DiscountRate = value;
            }
        }

        private string m_TickNum = "0";
        /// <summary>
        /// 座位数 格式：>9~>9
        /// </summary>
        public string TickNum
        {
            get
            {
                return m_TickNum;
            }
            set
            {
                m_TickNum = value;
            }
        }

        private string m_XSFee = string.Empty;

        /// <summary>
        /// 舱位价 格式：  430.00~500.00
        /// </summary>
        public string XSFee
        {
            get
            {
                return m_XSFee;
            }
            set
            {
                m_XSFee = value;
            }
        }

        private string m_Mileage = string.Empty;
        /// <summary>
        /// 里程 格式：1440.000000~1440.000000
        /// </summary>
        public string Mileage
        {
            get
            {
                return m_Mileage;
            }
            set
            {
                m_Mileage = value;
            }
        }


        private string m_Cabin = string.Empty;
        /// <summary>
        /// 舱位 格式：  N~K
        /// </summary>
        public string Cabin
        {
            get
            {
                return m_Cabin;
            }
            set
            {
                m_Cabin = value;
            }
        }

        private string m_FareFee = string.Empty;
        /// <summary>
        /// Y舱价格 格式：1440.000000~1440.000000
        /// </summary>
        public string FareFee
        {
            get
            {
                return m_FareFee;
            }
            set
            {
                m_FareFee = value;
            }
        }

        private string m_Reservation = string.Empty;
        /// <summary>
        /// 客规 格式： 啊啊啊~你
        /// </summary>
        public string Reservation
        {
            get
            {
                return m_Reservation;
            }
            set
            {
                m_Reservation = value;
            }
        }


        private string m_SpecialType = string.Empty;
        /// <summary>
        /// 特价类型 1动态特价 2固态特价 0为普通价格
        /// </summary>
        public string SpecialType
        {
            get
            {
                return m_SpecialType;
            }
            set
            {
                m_SpecialType = value;
            }
        }


        private string m_TravelType = string.Empty;
        /// <summary>
        /// 行程类型 
        /// </summary>
        public string TravelType
        {
            get
            {
                return m_TravelType;
            }
            set
            {
                m_TravelType = value;
            }
        }

        private string m_Terminal = string.Empty;
        /// <summary>
        /// 航站楼
        /// </summary>
        public string Terminal
        {
            get
            {
                return m_Terminal;
            }
            set
            {
                m_Terminal = value;
            }

        }


    }
}
