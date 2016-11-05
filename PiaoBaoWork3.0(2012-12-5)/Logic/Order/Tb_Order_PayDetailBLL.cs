using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;

namespace PbProject.Logic.Order
{
    public class Tb_Order_PayDetailBLL
    {
        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        /// <summary>
        /// 订单信息信息
        /// </summary>
        /// <param name="sqlWhere">查询条件</param>
        /// <returns></returns>
        public List<Tb_Order_PayDetail> GetListBySqlWhere(string sqlWhere)
        {
            List<Tb_Order_PayDetail> reList = baseDataManage.CallMethod("Tb_Order_PayDetail", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Order_PayDetail>;
            return reList;
        }
    }
}
