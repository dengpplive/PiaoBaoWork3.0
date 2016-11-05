using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using DataBase.Data;

namespace PbProject.Logic.Policy
{
    /// <summary>
    /// 政策
    /// </summary>
    public class Tb_Ticket_PolicyBLL
    {

        BaseDataManage baseDataManage = new BaseDataManage();

        /// <summary>
        /// 根据id获取政策
        /// </summary>
        /// <param name="logRrror"></param>
        public Tb_Ticket_Policy GetById(string id)
        {
            return new Dal.ControlBase.BaseData<Tb_Ticket_Policy>().GetById(id);
        }
        public List<Tb_Ticket_Policy> GetListByStrWhere(string StrWhere)
        {
            //return new Dal.ControlBase.BaseData<Tb_Ticket_Policy>().GetBySQLList(StrWhere);

            return baseDataManage.CallMethod("Tb_Ticket_Policy", "GetList", null, new Object[] { StrWhere }) as List<Tb_Ticket_Policy>;
        }
       
    }
}
