using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using PiaoBao.API.Common;
using PiaoBao.API.Common.Enum;
using PiaoBao.API.Common.AirQuery;

namespace PiaoBao.API.Services
{
    /// <summary>
    /// 往返航班查询
    /// </summary>
    public class SkyWayRoundWayServices : BaseServices
    {
        public override void Query(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            UserLoginInfo userInfo = AuthLogin.GetUserInfo(Username);
            AirQueryCommon airQueryCommon = new AirQueryCommon();
            try
            {

                #region 参数赋值

                string startCity = CommonMethod.GetFomartString(parames["startCity"]);
                string endCity = CommonMethod.GetFomartString(parames["endCity"]);
                string startDate = CommonMethod.GetFomartDate(CommonMethod.GetFomartString(parames["startDate"]));
                string backDate = CommonMethod.GetFomartDate(CommonMethod.GetFomartString(parames["backDate"]));
                string cairry = CommonMethod.GetFomartString(parames["cairry"]);
                bool isQueryShareFlight = CommonMethod.GetFomartString(parames["isQueryShareFlight"]).Equals("1") ? true : false;
                bool isQuerySpecialFlight = CommonMethod.GetFomartString(parames["isQuerySpecialFlight"]).Equals("1") ? true : false;

                #endregion

                //获取航班JSon
                AirInfoCollectionList list = airQueryCommon.GetAirQueryJSon(
                                userInfo,
                                startCity,
                                string.Empty,
                                endCity,
                                startDate,
                                backDate,
                                AirTravelType.RoundWay,
                                cairry,
                                isQueryShareFlight,
                                isQuerySpecialFlight
                       );
                writer.Write(list);
            }
            catch (Exception ex)
            {
                writer.WriteEx(547, "params type err", ex.Message);
            }
        }
    }
}