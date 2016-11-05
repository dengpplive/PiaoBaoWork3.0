using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PbProject.Model;
using System.Reflection;
namespace PbProject.WebCommon.Utility
{
    public class BaseParams
    {
         
        public BaseParams()
        {
           
        }
        /// <summary>
        /// 查询公司所有的参数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static PbProject.Model.definitionParam.BaseSwitch getParams(List<PbProject.Model.Bd_Base_Parameters> list)
        {
            PbProject.Model.definitionParam.BaseSwitch pmdb = new Model.definitionParam.BaseSwitch();
            foreach (Bd_Base_Parameters item in list)
            {
                if (PbProject.Model.definitionParam.paramsName.Base_Oil == item.SetName)
                {
                    pmdb.Base_Oil = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.cssURL == item.SetName)
                {
                    pmdb.CssURL = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.daPeiZhiCanShu == item.SetName)
                {
                    pmdb.DaPeiZhiCanShu = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.gongYingKongZhiFenXiao == item.SetName)
                {
                    pmdb.GongYingKongZhiFenXiao = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.heiPingCanShu == item.SetName)
                {
                    pmdb.HeiPingCanShu = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.jieKouZhangHao == item.SetName)
                {
                    pmdb.JieKouZhangHao = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.kongZhiXiTong == item.SetName)
                {
                    pmdb.KongZhiXiTong = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.Order_Index == item.SetName)
                {
                    pmdb.Order_Index = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.Policy_Order == item.SetName)
                {
                    pmdb.Policy_Order = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.wangYinLeiXing == item.SetName)
                {
                    pmdb.WangYinLeiXing = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.wangYinZhangHao == item.SetName)
                {
                    pmdb.WangYinZhangHao = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.suoShuYeWuYuan == item.SetName)
                {
                    pmdb.SuoShuYeWuYuan = item.SetValue;
                    continue;
                }

                if (PbProject.Model.definitionParam.paramsName.autoAccount == item.SetName)
                {
                    pmdb.AutoAccount = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.autoPayAccount == item.SetName)
                {
                    pmdb.AutoPayAccount = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.setCommission == item.SetName)
                {
                    pmdb.setCommission = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.cssQQ == item.SetName)
                {
                    pmdb.cssQQ = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.isDuLiFenXiao == item.SetName)
                {
                    pmdb.IsDuLiFenXiao = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.isShowDuLiInfo == item.SetName)
                {
                    pmdb.IsShowDuLiInfo = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.yunYingQuanXian == item.SetName)
                {
                    pmdb.yunYingQuanXian = item.SetValue;
                    continue;
                }
                if (PbProject.Model.definitionParam.paramsName.chuPiaoShiJian == item.SetName)
                {
                    pmdb.chuPiaoShiJian = item.SetValue;
                    continue;
                }
            }
            return pmdb;
        }


    }
}
