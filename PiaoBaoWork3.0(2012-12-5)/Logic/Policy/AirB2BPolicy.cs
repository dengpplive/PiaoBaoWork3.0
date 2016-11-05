using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using System.Data;
namespace PbProject.Logic.Policy
{
    /// <summary>
    /// b2b参数
    /// </summary>
    public class PolicyParamEx
    {
        /// <summary>
        /// 出票公司编号
        /// </summary>
        public string CpCpyNo = string.Empty;
        /// <summary>
        /// 编码
        /// </summary>
        public string Pnr = string.Empty;
        /// <summary>
        /// 大编码
        /// </summary>
        public string BigPnr = string.Empty;
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarryCode = string.Empty;
        /// <summary>
        /// 行程类型
        /// </summary>
        public string TravelType = "1";
        /// <summary>
        /// 供应商名字
        /// </summary>
        public string CpyName = string.Empty;
        /// <summary>
        /// 出票Office
        /// </summary>
        public string Office = string.Empty;
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string StartCityNameCode = string.Empty;
        /// <summary>
        /// 中转城市三字码
        /// </summary>
        public string MiddleCityNameCode = string.Empty;
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string TargetCityNameCode = string.Empty;
        /// <summary>
        /// 舱位
        /// </summary>
        public string Space = string.Empty;

        /// <summary>
        /// 航空公司返点
        /// </summary>
        public decimal AirReBate = 0m;

    }
    public class AirB2BPolicy
    {
        /// <summary>
        /// 获取航空公司B2B政策
        /// </summary>
        /// <param name="CpCpyNo">出票公司编号</param>
        /// <param name="Pnr">编码</param>
        /// <param name="BigPnr">大编码</param>
        /// <param name="CarryCode">航空公司二字码</param>
        /// <returns></returns>
        public List<Tb_Ticket_Policy> getAirB2BPolicy(PolicyParamEx Param, out string ErrMsg)
        {
            ErrMsg = "";
            List<Tb_Ticket_Policy> airPolicyList = new List<Tb_Ticket_Policy>();
            Param.CpCpyNo = Param.CpCpyNo.Length >= 12 ? Param.CpCpyNo.Substring(0, 12) : Param.CpCpyNo;
            PbProject.Model.definitionParam.BaseSwitch BS = new Model.definitionParam.BaseSwitch();
            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(Param.CpCpyNo.ToString());
            if (ParList == null)
            {
                return airPolicyList;
            }
            BS = WebCommon.Utility.BaseParams.getParams(ParList);
            //权限
            if (BS.KongZhiXiTong.Contains("|105|") && BS.yunYingQuanXian.Contains("|105|"))
            {
                w_Airb2bPolicy.B2BPolicyWebService B2BWebService = new w_Airb2bPolicy.B2BPolicyWebService();
                DataSet dsPolicy = B2BWebService.GetAirB2BPolicy(Param.CpCpyNo, Param.Pnr, Param.BigPnr, Param.CarryCode);
                if (dsPolicy != null && dsPolicy.Tables.Count > 0 && Param != null)
                {
                    DataTable ErrTable = dsPolicy.Tables["ErrMsg"];
                    if (ErrTable != null && ErrTable.Rows.Count > 0 && ErrTable.Rows[0][0] != DBNull.Value && ErrTable.Rows[0][0].ToString() == "OK")
                    {
                        foreach (DataTable item in dsPolicy.Tables)
                        {
                            if (item.TableName == "policy" && item.Rows.Count > 0)
                            {
                                Tb_Ticket_Policy tb_ticket_policy = new Tb_Ticket_Policy();

                                #region 为Tb_Ticket_Policy实体赋值
                                DataRow dr = item.Rows[0];
                                string strpgid = dr["pgid"] != DBNull.Value ? dr["pgid"].ToString() : "";
                                string strpgcode = dr["pgcode"] != DBNull.Value ? dr["pgcode"].ToString() : "";
                                //为票面价
                                string strticketprice = dr["ticketprice"] != DBNull.Value ? dr["ticketprice"].ToString() : "";
                                //政策返点
                                string strpolicynum = dr["policynum"] != DBNull.Value ? dr["policynum"].ToString() : "";
                                decimal AirReBate = 0m;
                                if (!decimal.TryParse(strpolicynum, out AirReBate))
                                {
                                    continue;
                                }
                                //税费
                                string strtotaltax = dr["totaltax"] != DBNull.Value ? dr["totaltax"].ToString() : "";
                                //实付金额
                                string strpayprice = dr["payprice"] != DBNull.Value ? dr["payprice"].ToString() : "";
                                decimal payprice = 0m;
                                decimal.TryParse(strpayprice, out payprice);

                                string strfc = dr["fc"] != DBNull.Value ? dr["fc"].ToString() : "";

                                tb_ticket_policy.id = Guid.NewGuid();
                                tb_ticket_policy.CpyNo = Param.CpCpyNo;
                                tb_ticket_policy.CpyName = "";
                                tb_ticket_policy.PolicyKind = 0;//-未确认-- 
                                tb_ticket_policy.GenerationType = 1;//-未确认-- 
                                tb_ticket_policy.ReleaseType = 1;//-未确认-- 
                                tb_ticket_policy.CarryCode = Param.CarryCode;
                                //行程类型
                                int TravelType = 1;
                                int.TryParse(Param.TravelType, out TravelType);
                                tb_ticket_policy.TravelType = TravelType;

                                tb_ticket_policy.PolicyType = 1;
                                tb_ticket_policy.TeamFlag = 0;
                                tb_ticket_policy.Office = Param.Office;
                                tb_ticket_policy.StartCityNameCode = Param.StartCityNameCode;//-未确认--
                                tb_ticket_policy.StartCityNameSame = 2;
                                tb_ticket_policy.MiddleCityNameCode = Param.MiddleCityNameCode;//-未确认--
                                tb_ticket_policy.MiddleCityNameSame = 2;
                                tb_ticket_policy.TargetCityNameCode = Param.TargetCityNameCode;//-未确认--
                                tb_ticket_policy.TargetCityNameSame = 2;
                                tb_ticket_policy.ApplianceFlightType = 1;
                                tb_ticket_policy.ApplianceFlight = "";
                                tb_ticket_policy.UnApplianceFlight = "";
                                tb_ticket_policy.ScheduleConstraints = "";
                                tb_ticket_policy.ShippingSpace = Param.Space;
                                tb_ticket_policy.InterPolicyId = "";//-未确认--
                                tb_ticket_policy.SpacePrice = 0m; //-未确认--
                                tb_ticket_policy.ReferencePrice = 0m;
                                tb_ticket_policy.AdvanceDay = 0;
                                tb_ticket_policy.AirReBate = 0;//-未确认-- 
                                tb_ticket_policy.AirReBateReturnMoney = 0;
                                tb_ticket_policy.DownPoint = AirReBate;//-未确认-- 
                                tb_ticket_policy.DownReturnMoney = 0;
                                tb_ticket_policy.LaterPoint = 0;
                                tb_ticket_policy.LaterReturnMoney = 0;
                                tb_ticket_policy.SharePoint = 0;
                                tb_ticket_policy.SharePointReturnMoney = 0;
                                tb_ticket_policy.FlightStartDate = System.DateTime.Now;
                                tb_ticket_policy.FlightEndDate = System.DateTime.Now;
                                tb_ticket_policy.PrintStartDate = System.DateTime.Now;
                                tb_ticket_policy.PrintEndDate = System.DateTime.Now;
                                tb_ticket_policy.AuditDate = System.DateTime.Now;
                                tb_ticket_policy.AuditType = 1;
                                tb_ticket_policy.AuditLoginName = "";//-未确认-- 
                                tb_ticket_policy.AuditName = "";//-未确认-- 
                                tb_ticket_policy.CreateDate = System.DateTime.Now;
                                tb_ticket_policy.CreateLoginName = "";//-未确认-- 
                                tb_ticket_policy.CreateName = "";//-未确认-- 
                                tb_ticket_policy.UpdateDate = System.DateTime.Now;//-未确认-- 
                                tb_ticket_policy.UpdateLoginName = "";//-未确认-- 
                                tb_ticket_policy.UpdateName = "";//-未确认-- 
                                tb_ticket_policy.Remark = "";//-未确认-- 
                                tb_ticket_policy.IsApplyToShareFlight = 0;
                                tb_ticket_policy.ShareAirCode = "";
                                tb_ticket_policy.IsLowerOpen = 0;
                                tb_ticket_policy.HighPolicyFlag = 0;
                                tb_ticket_policy.AutoPrintFlag = 2;//默认自动出票
                                tb_ticket_policy.GroupId = "";//-未确认-- 
                                tb_ticket_policy.IsPause = 0;
                                tb_ticket_policy.A13 = "1";//航空公司政策
                                tb_ticket_policy._AirPayMoney = payprice;//支付金额
                                #endregion
                                //添加到集合
                                airPolicyList.Add(tb_ticket_policy);
                            }
                        }
                    }
                    else
                    {
                        if (ErrTable != null && ErrTable.Rows.Count > 0 && ErrTable.Rows[0][0] != DBNull.Value && ErrTable.Rows[0][0].ToString() != "")
                        {
                            ErrMsg = ErrTable.Rows[0][0].ToString();
                        }
                        else
                        {
                            DataTable pnrinfo = dsPolicy.Tables["pnrinfo"];
                            if (pnrinfo != null && pnrinfo.Rows.Count > 0)
                            {
                                //<pnr>MFC9B3|JMKDVD</pnr><air>SC</air>
                                ErrMsg = "航空公司:" + pnrinfo.Rows[0]["air"].ToString() + "编码：" + pnrinfo.Rows[0]["pnr"].ToString() + " 信息：" + pnrinfo.Rows[0]["message"].ToString();
                            }
                        }
                    }
                }
            }
            return airPolicyList;
        }
    }
}
