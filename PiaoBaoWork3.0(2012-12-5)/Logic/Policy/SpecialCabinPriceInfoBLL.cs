using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using System.Web;
namespace PbProject.Logic.Policy
{
    /// <summary>
    /// 特价舱位缓存信息
    /// </summary>
    public class SpecialCabinPriceInfoBLL
    {
        private PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
        /// <summary>
        /// 获取特价缓存
        /// </summary>
        /// <param name="SpAirCode"></param>
        /// <param name="FlyDate"></param>
        /// <param name="FromCityCode"></param>
        /// <param name="ToCityCode"></param>
        /// <param name="SpCabin"></param>
        /// <returns></returns>
        public Tb_SpecialCabin_PriceInfo GetSpPrice(string SpAirCode, string SpFlightCode, DateTime FlyDate, string FromCityCode, string ToCityCode, string SpCabin)
        {
            Tb_SpecialCabin_PriceInfo tb_specialcabin_priceinfo = null;
            string sqlWhere = string.Format("FromCityCode='{0}' and ToCityCode='{1}' and SpAirCode='{2}' and SpFlightCode='{3}' and (SpCabin='{4}' and SpCabin<>'Z') and FlightTime='{5}' ", FromCityCode, ToCityCode, SpAirCode, SpFlightCode, SpCabin, FlyDate.ToString("yyyy-MM-dd HH:mm"));
            List<Tb_SpecialCabin_PriceInfo> SpList = this.baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "GetList", null, new object[] { sqlWhere }) as List<Tb_SpecialCabin_PriceInfo>;
            if (SpList != null && SpList.Count > 0)
            {
                tb_specialcabin_priceinfo = SpList[0];
            }
            return tb_specialcabin_priceinfo;
        }

        /// <summary>
        /// 获取特价缓存
        /// </summary>
        /// <param name="SpAirCode"></param>
        /// <param name="FlyDate"></param>
        /// <param name="FromCityCode"></param>
        /// <param name="ToCityCode"></param>
        /// <param name="SpCabin"></param>
        /// <returns></returns>
        public List<Tb_SpecialCabin_PriceInfo> GetSpPrice(string SpAirCode, DateTime FlyDate, string FromCityCode, string ToCityCode)
        {
            if (SpAirCode != "")
            {
                SpAirCode = "SpAirCode='" + SpAirCode + "' and ";
            }
            string sqlWhere = string.Format("  SpCabin<>'Z' and FromCityCode='{0}' and ToCityCode='{1}' and {2} CONVERT(varchar(100), FlightTime, 23)='{3}' ", FromCityCode, ToCityCode, SpAirCode, FlyDate.ToString("yyyy-MM-dd"));
            List<Tb_SpecialCabin_PriceInfo> SpList = this.baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "GetList", null, new object[] { sqlWhere }) as List<Tb_SpecialCabin_PriceInfo>;

            return SpList;
        }


        /// <summary>
        /// 保存修改特价缓存
        /// </summary>
        /// <param name="SpAirCode">航空公司二字码</param>
        /// <param name="SpFlightCode">航班号</param>
        /// <param name="FlyDate">起飞日期时间 yyyy-MM-dd HH:mm:ss</param>
        /// <param name="FromCityCode">出发城市三字码</param>
        /// <param name="ToCityCode">到达城市三字码</param>
        /// <param name="SpCabin">舱位</param>
        /// <param name="SpPrice">舱位价</param>
        /// <param name="SpABFare">基建</param>
        /// <param name="SpRQFare">燃油</param>
        /// <returns></returns>
        public bool SaveSpPrice(string SpAirCode, string SpFlightCode, DateTime FlyDate, string FromCityCode, string ToCityCode, string SpCabin, decimal SpPrice, decimal SpABFare, decimal SpRQFare)
        {
            bool IsSuc = false;
            try
            {
                if (SpCabin.ToUpper() == "Z" && SpAirCode.ToUpper() == "CZ")
                {
                    return IsSuc;
                }
                Tb_SpecialCabin_PriceInfo tb_specialcabin_priceinfo = null;
                string sqlWhere = string.Format("FromCityCode='{0}' and ToCityCode='{1}' and SpAirCode='{2}' and SpFlightCode='{3}' and SpCabin='{4}' and FlightTime='{5}' ", FromCityCode, ToCityCode, SpAirCode, SpFlightCode, SpCabin, FlyDate.ToString("yyyy-MM-dd HH:mm"));
                List<Tb_SpecialCabin_PriceInfo> SpList = this.baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "GetList", null, new object[] { sqlWhere }) as List<Tb_SpecialCabin_PriceInfo>;
                bool IsAdd = false;
                if (SpList != null && SpList.Count > 0)
                {
                    //修改
                    tb_specialcabin_priceinfo = SpList[0];
                    tb_specialcabin_priceinfo.SpPrice = SpPrice;
                    tb_specialcabin_priceinfo.SpABFare = SpABFare;
                    tb_specialcabin_priceinfo.SpRQFare = SpRQFare;
                    IsAdd = false;
                }
                else
                {
                    //添加
                    tb_specialcabin_priceinfo = new Tb_SpecialCabin_PriceInfo();
                    tb_specialcabin_priceinfo.SpAirCode = SpAirCode;
                    tb_specialcabin_priceinfo.SpFlightCode = SpFlightCode;
                    tb_specialcabin_priceinfo.SpCabin = SpCabin;
                    tb_specialcabin_priceinfo.FromCityCode = FromCityCode;
                    tb_specialcabin_priceinfo.ToCityCode = ToCityCode;
                    tb_specialcabin_priceinfo.FlightTime = FlyDate;

                    tb_specialcabin_priceinfo.SpPrice = SpPrice;
                    tb_specialcabin_priceinfo.SpABFare = SpABFare;
                    tb_specialcabin_priceinfo.SpRQFare = SpRQFare;
                    IsAdd = true;
                }
                tb_specialcabin_priceinfo.UpdateTime = DateTime.Parse(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                if (IsAdd)
                {
                    IsSuc = (bool)this.baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "Insert", null, new object[] { tb_specialcabin_priceinfo });
                }
                else
                {
                    IsSuc = (bool)this.baseDataManage.CallMethod("Tb_SpecialCabin_PriceInfo", "Update", null, new object[] { tb_specialcabin_priceinfo });
                }
            }
            catch (Exception ex)
            {
                PbProject.WebCommon.Log.Log.RecordLog("SaveSpPrice", "方法名:SaveSpPrice 时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ex.Message, true, HttpContext.Current.Request);
            }
            return IsSuc;
        }


    }
}
