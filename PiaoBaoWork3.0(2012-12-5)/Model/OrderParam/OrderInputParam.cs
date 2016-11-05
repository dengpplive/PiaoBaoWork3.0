using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PnrAnalysis.Model;
namespace PbProject.Model
{
    /// <summary>
    /// 生成订单传入参数类
    /// </summary>    
    [Serializable]
    public class OrderInputParam
    {
        private List<OrderMustParamModel> _OrderParamModel = new List<OrderMustParamModel>();

        /// <summary>
        /// 生成订单所需实体
        /// </summary>
        public List<OrderMustParamModel> OrderParamModel
        {
            get { return _OrderParamModel; }
            set { _OrderParamModel = value; }
        }

        private List<string> _ExecSQLList = new List<string>();
        /// <summary>
        /// 需要执行的SQL语句列表
        /// </summary>
        public List<string> ExecSQLList
        {
            get { return _ExecSQLList; }
            set { _ExecSQLList = value; }
        }
        private RePnrObj _PnrInfo = null;
        /// <summary>
        /// PNR有关的数据信息
        /// </summary>
        public RePnrObj PnrInfo
        {
            get { return _PnrInfo; }
            set { _PnrInfo = value; }
        }

        private int _IsCreatePayDetail = 0;
        /// <summary>
        /// 是否生成账单默认0不生成 1生成
        /// </summary>
        public int IsCreatePayDetail
        {
            get
            {
                return _IsCreatePayDetail;
            }
            set
            {
                _IsCreatePayDetail = value;
            }
        }

        private bool _IsRobTicketOrder = false;
        /// <summary>
        /// 是否为抢票订单 true 是   false不是
        /// </summary>
        public bool IsRobTicketOrder
        {
            get
            {
                return _IsRobTicketOrder;
            }
            set
            {
                _IsRobTicketOrder = value;
            }
        }
        private string _OldRobTicketOrderId = string.Empty;
        /// <summary>
        /// 抢票关联的原订单号
        /// </summary>
        public string OldRobTicketOrderId
        {
            get
            {
                return _OldRobTicketOrderId;
            }
            set
            {
                _OldRobTicketOrderId = value;
            }
        }

        private string _ErrMsg = string.Empty;
        /// <summary>
        /// 生成订单出错信息
        /// </summary>
        public string ErrMsg
        {
            get
            {
                return _ErrMsg;
            }
            set
            {
                _ErrMsg = value;
            }
        }
    }
    [Serializable]
    public class OrderMustParamModel
    {
        private List<Tb_Ticket_Passenger> _PasList = null;
        /// <summary>
        /// 乘机人列表
        /// </summary>
        public List<Tb_Ticket_Passenger> PasList
        {
            get { return _PasList; }
            set { _PasList = value; }
        }

        private List<string> _UpdatePassengerFileds = new List<string>();
        /// <summary>
        /// 更新操作时需要更新乘机人表的字段名集合
        /// </summary>
        public List<string> UpdatePassengerFileds
        {
            get { return _UpdatePassengerFileds; }
            set { _UpdatePassengerFileds = value; }
        }
        private List<Tb_Ticket_SkyWay> _SkyList = null;
        /// <summary>
        /// 航段列表
        /// </summary>
        public List<Tb_Ticket_SkyWay> SkyList
        {
            get { return _SkyList; }
            set { _SkyList = value; }
        }

        private List<string> _UpdateSkyWayFileds = new List<string>();
        /// <summary>
        /// 更新操作时需要更新航段表的字段名集合
        /// </summary>
        public List<string> UpdateSkyWayFileds
        {
            get { return _UpdateSkyWayFileds; }
            set { _UpdateSkyWayFileds = value; }
        }
        private Tb_Ticket_Order _Order = null;
        /// <summary>
        /// 订单信息
        /// </summary>
        public Tb_Ticket_Order Order
        {
            get { return _Order; }
            set { _Order = value; }
        }

        private List<string> _UpdateOrderFileds = new List<string>();
        /// <summary>
        /// 更新操作时需要更新订单表的字段名集合
        /// </summary>
        public List<string> UpdateOrderFileds
        {
            get { return _UpdateOrderFileds; }
            set { _UpdateOrderFileds = value; }
        }

        private Log_Tb_AirOrder _LogOrder = null;
        /// <summary>
        /// 订单日志实体
        /// </summary>
        public Log_Tb_AirOrder LogOrder
        {
            get { return _LogOrder; }
            set { _LogOrder = value; }
        }
    }
}
