using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;

namespace PbProject.Logic.Order
{
    public class Tb_Ticket_PassengerBLL
    {
        /// <summary>
        /// 根据sql查询乘机人信息
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <returns>对应参数信息</returns>
        public List<Tb_Ticket_Passenger> GetPasListBySQLWhere(string sqlWhere)
        {
            return new Dal.ControlBase.BaseData<Tb_Ticket_Passenger>().GetList(sqlWhere);
        }

        /// <summary>
        /// 根据 订单 orderID 查询
        /// </summary>
        /// <param name="orderID">订单号</param>
        /// <returns></returns>
        public List<Tb_Ticket_Passenger> GetPasListByOrderID(string orderID)
        {
            string sqlWhere = "OrderId='" + orderID + "'";

            return new Dal.ControlBase.BaseData<Tb_Ticket_Passenger>().GetList(sqlWhere);
        } 
    }
}
