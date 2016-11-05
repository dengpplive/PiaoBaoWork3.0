using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using DataBase.Data;
using System.Data;
using System.Threading;
using PbProject.Logic.Pay;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PbProject.Logic.Policy
{
    public class UGroupPolicy
    {
        BaseDataManage Manage = new BaseDataManage();

        public UGroupPolicy()
        { 
        
        }
        /// <summary>
        /// 获取散冲团政策
        /// </summary>
        /// <param name="airCode">承运人</param>
        /// <param name="fromCity">出发城市</param>
        /// <param name="toCity">到达城市</param>
        /// <param name="flightTime">出发时间</param>
        /// <returns></returns>
        public List<Tb_Ticket_UGroupPolicy> getTb_Ticket_UGroupPolicy(string airCode,string fromCity,string toCity,
            string flightTime)
        {
            string sqlwhere = " 1=1 "
                            + " and AirCode like '%" + airCode + "%'"
                            + " and FromCityCode like '%" + fromCity + "%'"
                            + " and ToCityCode like '%" + toCity + "%'"
                            + " and FlightStartDate<='" + flightTime + "'"
                            + " and FlightEndDate>='" + flightTime + "'"
                            + " and PrintStartDate<='" + flightTime + "'"
                            + " and PrintEndDate>='" + flightTime + "'";
            List<Tb_Ticket_UGroupPolicy> objList = Manage.CallMethod("Tb_Ticket_UGroupPolicy", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_UGroupPolicy>;
            return objList;
        }
    }
}
