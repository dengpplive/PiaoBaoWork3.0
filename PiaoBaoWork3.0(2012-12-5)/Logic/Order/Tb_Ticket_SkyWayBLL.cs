using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;

namespace PbProject.Logic.Order
{
    public class Tb_Ticket_SkyWayBLL
    {
        /// <summary>
        /// 根据订单号查询航段信息
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns>对应参数信息</returns>
        public List<Tb_Ticket_SkyWay> GetSkyWayListBySQLWhere(string sqlWhere)
        {
            return new Dal.ControlBase.BaseData<Tb_Ticket_SkyWay>().GetList(sqlWhere);
        } 
    }
}
