using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using PiaoBao.API.Common;
using PbProject.Model;
using PbProject.Logic.Order;
using PbProject.Logic.Policy;
using PbProject.Logic.Pay;
using PbProject.Logic.PID;
using System.Text;
using PnrAnalysis;
using PnrAnalysis.Model;

namespace PiaoBao.API.Services
{
    public class ConfirmOrderServices : BaseServices
    {
        /// <summary>
        /// 确认订单
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="parames"></param>
        public override void Create(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            var userLogin = AuthLogin.GetUserInfo(Username);
            string adultOrderId = parames["adultOrderId"];
            string childOrderId = parames["childOrderId"];
            string adultPolicyGuid = parames["adultPolicyGuid"];//成人政策guid
            string childPolicyGuid = parames["childPolicyGuid"];//婴儿政策guid
            string remark = parames["remark"];




            var adultPolicy = PolicyCacheManager.Get(adultPolicyGuid) as PolicyParamForAPI;
            var childPolicy = PolicyCacheManager.Get(childPolicyGuid) as PolicyParamForAPI;
            OrderInputParam InputParam = createOrderInputParam(adultOrderId, childOrderId);
            Tb_Ticket_OrderBLL orderBLL = new Tb_Ticket_OrderBLL();

            if (adultPolicy == null)
            {
                writer.WriteEx(554, "Cache is disabled", "政策缓存已失效，请重新操作");
            }
            else
            {
                #region 更新订单 主要修改价格,政策和添加订单账单明细
                Bill bill = new Bill();

                Data d = new Data(userLogin.Company.UninCode);//采购佣金进舍规则: 0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分

                string ErrMsg = "";
                bool IsSuc = false;
                //扩展参数
                ParamEx pe = new ParamEx();
                pe.UsePIDChannel = userLogin.FQP.KongZhiXiTong != null && userLogin.FQP.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                //发送指令管理类
                SendInsManage SendIns = new SendInsManage(userLogin.User.LoginName, userLogin.Company.UninCode, pe, userLogin.Configparam);
                //提示
                StringBuilder sbTip = new StringBuilder();
                try
                {

                    AjaxPolicyMatchOutData APM = new AjaxPolicyMatchOutData();
                    APM.OutPutPolicyList.Add(adultPolicy);
                    APM.OutPutPolicyList.Add(childPolicy);
                    //供应商Office
                    string GYOffice = userLogin.Configparam != null ? userLogin.Configparam.Office.ToUpper() : "";
                    if (InputParam != null && APM != null && APM.OutPutPolicyList != null && APM.OutPutPolicyList.Count > 0)
                    {
                        AjAxPolicyParam AdultPolicy = adultPolicy;
                        AjAxPolicyParam ChildPolicy = childPolicy;

                        //至少选择一条政策 成人或者儿童
                        if (AdultPolicy == null && ChildPolicy == null)
                        {
                            writer.WriteEx(567, "choose one policy at least", "请选择一条政策数据");
                        }

                        else
                        {
                            PatInfo INFPrice = null;

                            if (HasBaby(InputParam))
                            {
                                //婴儿价格
                                INFPrice = GetINFPAT();
                            }
                            //订单实体需要更改数据的字段名
                            List<string> UpdateOrderFileds = new List<string>();
                            //订单修改字段
                            UpdateOrderFileds.AddRange(new string[] {"OldRerurnMoney","OldPolicyPoint","OutOrderPayMoney","A7","A13","A1","A11","PrintOffice", "CPCpyNo","YDRemark", "PolicyId", "AirPoint", "PolicyPoint", "ReturnPoint", "PolicyMoney","PolicyCancelTime","PolicyReturnTime",
                    "DiscountDetail","PolicyType","PolicySource","AutoPrintFlag","PolicyRemark","PMFee","ABFee","FuelFee","BabyFee" ,"PayMoney","OrderMoney"});
                            //航段实体需要更改数据的字段名
                            List<string> UpdateSkyWayFileds = new List<string>();
                            //航段修改字段
                            UpdateSkyWayFileds.AddRange(new string[] { "SpacePrice", "ABFee", "FuelFee", "Discount" });
                            //乘客实体需要更改数据的字段名
                            List<string> UpdatePasFileds = new List<string>();
                            //修改乘机人
                            UpdatePasFileds.AddRange(new string[] { "PMFee", "ABFee", "FuelFee" });
                            //承运人二字码
                            string CarrayCode = string.Empty;
                            string orderIDs = "";
                            //婴儿与编码中的婴儿个数是否不一致 true不一致 false一致
                            bool IsINFCheck = false;
                            //婴儿与编码中的婴儿个数是否不一致提示
                            string INFCountCheckMsg = "<b class=\"red\">编码中婴儿个数与预订婴儿个数不一致，请手动补全编码中婴儿！</b>";
                            //Bill返回有无SQL
                            bool IsBillOK = false;

                            //修改实体相关的值后更新即可
                            for (int i = 0; i < InputParam.OrderParamModel.Count; i++)
                            {
                                OrderMustParamModel item = InputParam.OrderParamModel[i];
                                //承运人二字码
                                CarrayCode = item.Order.CarryCode.Split('/')[0].ToUpper().Trim();

                                if (orderIDs.Contains(item.Order.OrderId))
                                    continue;
                                else
                                    orderIDs += item.Order.OrderId + "|";

                                #region 设置需要更改数据的字段名集合
                                item.UpdateOrderFileds = UpdateOrderFileds;
                                item.UpdateSkyWayFileds = UpdateSkyWayFileds;
                                item.UpdatePassengerFileds = UpdatePasFileds;
                                #endregion

                                #region 实体处理
                                //订单中的总价
                                decimal TotalPMPrice = 0m, TotalABFare = 0, TotalRQFare = 0m;


                                //item.Order.YDRemark = remark;

                                //预订备注信息
                                item.Order.YDRemark = remark;

                                //订单处理 成人订单政策
                                if (!item.Order.IsChdFlag && AdultPolicy != null)
                                {
                                    #region 成人或者婴儿实体价格赋值

                                    //检测白屏预订婴儿个数与编码中的婴儿个数
                                    if (item.Order.OrderSourceType == 1 || item.Order.OrderSourceType == 5)
                                    {
                                        IsINFCheck = yudingINFCheck(InputParam.PnrInfo, item.PasList);
                                    }

                                    //婴儿价格
                                    decimal INFPMFee = 0m, INFABFare = 0m, INFRQFare = 0m;
                                    if (INFPrice != null)
                                    {
                                        decimal.TryParse(INFPrice.Fare, out INFPMFee);
                                        decimal.TryParse(INFPrice.TAX, out INFABFare);
                                        decimal.TryParse(INFPrice.RQFare, out INFRQFare);
                                    }
                                    //成人价格
                                    decimal PMFee = AdultPolicy.SeatPrice, ABFare = AdultPolicy.ABFare, RQFare = AdultPolicy.RQFare;

                                    #region 特价缓存处理
                                    //特价时特价缓存处理  为特价且PAT内容不为空
                                    if (AdultPolicy.PolicyKind == 2 && item.SkyList[0].Pat.Trim() != "")
                                    {
                                        //白屏和PNR导入
                                        if (item.Order.OrderSourceType == 1 || item.Order.OrderSourceType == 2 || item.Order.OrderSourceType == 6 || item.Order.OrderSourceType == 10)
                                        {
                                            //特价缓存
                                            SpecialCabinPriceInfoBLL SpBll = new SpecialCabinPriceInfoBLL();
                                            PnrAnalysis.FormatPNR pnrformat = new PnrAnalysis.FormatPNR();
                                            string errMsg = "";
                                            PnrAnalysis.PatModel Pat = pnrformat.GetPATInfo(item.SkyList[0].Pat.Trim(), out errMsg);
                                            if (Pat.UninuePatList.Count > 0)
                                            {
                                                decimal m_Fare = 0m;
                                                decimal m_TAX = 0m;
                                                decimal m_RQFare = 0m;
                                                decimal.TryParse(Pat.UninuePatList[0].Fare, out m_Fare);
                                                decimal.TryParse(Pat.UninuePatList[0].TAX, out m_TAX);
                                                decimal.TryParse(Pat.UninuePatList[0].RQFare, out m_RQFare);
                                                //价格不相等
                                                if (m_Fare != PMFee)
                                                {
                                                    //存入缓存
                                                    SpBll.SaveSpPrice(item.SkyList[0].CarryCode.ToUpper(), item.SkyList[0].FlightCode, item.SkyList[0].FromDate, item.SkyList[0].FromCityCode, item.SkyList[0].ToCityCode, item.SkyList[0].Space, m_Fare, m_TAX, m_RQFare);
                                                }
                                            }
                                        }
                                    }
                                    #endregion


                                    //乘机人实体处理
                                    for (int j = 0; j < item.PasList.Count; j++)
                                    {
                                        if (item.PasList[j].PassengerType == 1)
                                        {
                                            //成人
                                            item.PasList[j].PMFee = PMFee;
                                            item.PasList[j].ABFee = ABFare;
                                            item.PasList[j].FuelFee = RQFare;
                                        }
                                        else
                                        {
                                            //婴儿
                                            if (item.PasList[j].PassengerType == 3 && INFPrice != null)
                                            {
                                                item.PasList[j].PMFee = INFPMFee;
                                                item.PasList[j].ABFee = INFABFare;
                                                item.PasList[j].FuelFee = INFRQFare;
                                            }
                                        }
                                        if (item.PasList[j].PassengerType == 1 || item.PasList[j].PassengerType == 3)
                                        {
                                            //订单价格
                                            TotalPMPrice += item.PasList[j].PMFee;
                                            TotalABFare += item.PasList[j].ABFee;
                                            TotalRQFare += item.PasList[j].FuelFee;
                                        }
                                    }
                                    //航段实体处理
                                    //string Discount = "0";
                                    for (int k = 0; k < item.SkyList.Count; k++)
                                    {
                                        item.SkyList[k].ABFee = ABFare;
                                        item.SkyList[k].FuelFee = RQFare;
                                        //只是单程才重新赋值
                                        if (item.SkyList.Count == 1)
                                        {
                                            item.SkyList[k].SpacePrice = PMFee;
                                        }
                                        //item.SkyList[k].Discount = AdultPolicy.DiscountRate.ToString();
                                        //if (Discount.Length > 10)
                                        //{
                                        //    Discount = Discount.Substring(0, 10);
                                        //}
                                        //item.SkyList[k].Discount = Discount;
                                    }
                                    //成人订单(含有婴儿) 赋值
                                    item.Order.PMFee = TotalPMPrice;
                                    item.Order.ABFee = TotalABFare;
                                    item.Order.FuelFee = TotalRQFare;
                                    if (INFPrice != null)
                                    {
                                        //婴儿票面价
                                        item.Order.BabyFee = INFPMFee;
                                    }
                                    item.Order.PolicyId = AdultPolicy.PolicyId;
                                    item.Order.PolicyPoint = AdultPolicy.PolicyPoint;
                                    item.Order.ReturnMoney = AdultPolicy.PolicyReturnMoney;
                                    item.Order.AirPoint = AdultPolicy.AirPoint;
                                    item.Order.ReturnPoint = AdultPolicy.ReturnPoint;
                                    item.Order.LaterPoint = AdultPolicy.LaterPoint;
                                    item.Order.PolicyMoney = AdultPolicy.PolicyYongJin;
                                    item.Order.DiscountDetail = AdultPolicy.DiscountDetail;
                                    item.Order.PolicyType = int.Parse(AdultPolicy.PolicyType);
                                    item.Order.PolicySource = int.Parse(AdultPolicy.PolicySource);
                                    item.Order.AutoPrintFlag = int.Parse(AdultPolicy.AutoPrintFlag);
                                    item.Order.PolicyCancelTime = AdultPolicy.FPGQTime;
                                    item.Order.PolicyReturnTime = AdultPolicy.PolicyReturnTime;
                                    //出票公司编号
                                    string CPCpyNo = string.IsNullOrEmpty(AdultPolicy.CPCpyNo) ? userLogin.mSupCompany.UninCode : AdultPolicy.CPCpyNo;
                                    item.Order.CPCpyNo = CPCpyNo.Length > 12 ? CPCpyNo.Substring(0, 12) : CPCpyNo;

                                    item.Order.PolicyRemark = AdultPolicy.PolicyRemark;//政策备注
                                    //原始政策返点
                                    item.Order.OldPolicyPoint = AdultPolicy.OldPolicyPoint;
                                    //原始政策现返
                                    item.Order.OldReturnMoney = AdultPolicy.OldPolicyReturnMoney;

                                    item.Order.A1 = 1;//已确认
                                    item.Order.A2 = AdultPolicy.PolicyKind;//政策种类
                                    item.Order.A7 = AdultPolicy.AirPoint; //航空公司返点
                                    item.Order.A11 = AdultPolicy.PatchPonit;//补点


                                    ////计算订单金额  
                                    //item.Order.PayMoney = d.CreateOrderPayMoney(item.Order, item.PasList);
                                    ////出票方收款金额
                                    //item.Order.OrderMoney = d.CreateOrderOrderMoney(item.Order, item.PasList);

                                    bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);


                                    //出票Office
                                    if (AdultPolicy.PolicyOffice != "")
                                    {
                                        item.Order.PrintOffice = AdultPolicy.PolicyOffice;
                                    }
                                    if ((item.Order.OrderSourceType == 1 || item.Order.OrderSourceType == 2))
                                    {
                                        //自动授权Office
                                        if (AdultPolicy.PolicyOffice.Trim().Length == 6 && !GYOffice.Contains(AdultPolicy.PolicyOffice.Trim().ToUpper()))
                                        {
                                            SendIns.AuthToOffice(item.Order.PNR, AdultPolicy.PolicyOffice, item.Order.Office, out ErrMsg);
                                        }
                                        //备注HU的A舱要添加一个指令才能入库，OSI HU CKIN SSAC/S1
                                        if (AdultPolicy.PolicySource == "1" && AdultPolicy.PolicyType == "1" && AdultPolicy.AutoPrintFlag == "2" && item.Order.PNR.Trim().Length == 6 && item.Order.CarryCode.ToUpper().Trim() == "HU" && item.Order.Space.ToUpper().Trim() == "A")
                                        {
                                            string Office = item.Order.Office, Cmd = string.Format("RT{0}|OSI HU CKIN SSAC/S1^\\", item.Order.PNR.Trim());
                                            SendIns.Send(Cmd, ref Office, 10);
                                        }
                                    }
                                    if (item.Order.PolicySource <= 2)
                                    {
                                        //本地政策提示
                                        sbTip.Append("</br><ul><li>1.请于一小时内支付此订单,未支付将自动取消</li><li>2.编码内容中必须存在证件内容一项</li><li>3.PNR需要包含证件号</li><li>" + (IsINFCheck ? "4." + INFCountCheckMsg : "") + "</li></ul>");
                                    }
                                    else
                                    {
                                        //接口和共享政策提示
                                        if (AdultPolicy.PolicyOffice.Trim().Length == 6)
                                        {
                                            sbTip.Append("</br><ul><li>1.编码内容中必须存在证件内容一项</li><li>2.PNR需要包含证件号</li><li>3.请授权,授权指令：RMK TJ AUTH " + AdultPolicy.PolicyOffice + "</li>" + (IsINFCheck ? "4." + INFCountCheckMsg : "") + "</ul>");
                                        }
                                        else
                                        {
                                            sbTip.Append("<ul ><li>1.编码内容中必须存在证件内容一项!</li><li>2.PNR需要包含证件号!</li>" + (IsINFCheck ? "3." + INFCountCheckMsg : "") + "</ul>");
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    if (ChildPolicy != null)
                                    {
                                        #region 儿童实体赋值
                                        //儿童订单政策

                                        //儿童价格
                                        decimal PMFee = ChildPolicy.SeatPrice, ABFare = ChildPolicy.ABFare, RQFare = ChildPolicy.RQFare;
                                        //乘机人实体处理
                                        for (int j = 0; j < item.PasList.Count; j++)
                                        {
                                            if (item.PasList[j].PassengerType == 2)
                                            {
                                                //儿童
                                                item.PasList[j].PMFee = PMFee;
                                                item.PasList[j].ABFee = ABFare;
                                                item.PasList[j].FuelFee = RQFare;
                                                //订单价格
                                                TotalPMPrice += item.PasList[j].PMFee;
                                                TotalABFare += item.PasList[j].ABFee;
                                                TotalRQFare += item.PasList[j].FuelFee;
                                            }
                                        }
                                        //航段实体处理
                                        //string Discount = "0";
                                        for (int k = 0; k < item.SkyList.Count; k++)
                                        {
                                            item.SkyList[k].ABFee = ABFare;
                                            item.SkyList[k].FuelFee = RQFare;
                                            //只是单程才重新赋值
                                            if (item.SkyList.Count == 1)
                                            {
                                                item.SkyList[k].SpacePrice = PMFee;
                                            }
                                            //Discount = ChildPolicy.DiscountRate.ToString();
                                            //if (Discount.Length > 10)
                                            //{
                                            //    Discount = Discount.Substring(0, 10);
                                            //}
                                            //item.SkyList[k].Discount = Discount;
                                        }
                                        //儿童订单赋值                              
                                        item.Order.PMFee = TotalPMPrice;
                                        item.Order.ABFee = TotalABFare;
                                        item.Order.FuelFee = TotalRQFare;
                                        //出票公司编号
                                        string CPCpyNo = string.IsNullOrEmpty(ChildPolicy.CPCpyNo) ? userLogin.mSupCompany.UninCode : ChildPolicy.CPCpyNo;
                                        item.Order.CPCpyNo = CPCpyNo.Length > 12 ? CPCpyNo.Substring(0, 12) : CPCpyNo;
                                        item.Order.PolicyId = ChildPolicy.PolicyId;
                                        item.Order.AirPoint = ChildPolicy.AirPoint;
                                        item.Order.PolicyPoint = ChildPolicy.PolicyPoint;
                                        item.Order.ReturnPoint = ChildPolicy.ReturnPoint;
                                        item.Order.LaterPoint = ChildPolicy.LaterPoint;
                                        item.Order.ReturnMoney = ChildPolicy.PolicyReturnMoney;
                                        item.Order.PolicyMoney = ChildPolicy.PolicyYongJin;
                                        item.Order.DiscountDetail = ChildPolicy.DiscountDetail;
                                        item.Order.PolicyType = int.Parse(ChildPolicy.PolicyType);
                                        item.Order.PolicySource = int.Parse(ChildPolicy.PolicySource);
                                        item.Order.AutoPrintFlag = int.Parse(ChildPolicy.AutoPrintFlag);
                                        item.Order.PolicyCancelTime = ChildPolicy.FPGQTime;
                                        item.Order.PolicyReturnTime = ChildPolicy.PolicyReturnTime;
                                        item.Order.PolicyRemark = ChildPolicy.PolicyRemark;//政策备注
                                        //原始政策返点
                                        item.Order.OldPolicyPoint = ChildPolicy.OldPolicyPoint;
                                        //原始政策现返
                                        item.Order.OldReturnMoney = ChildPolicy.OldPolicyReturnMoney;

                                        item.Order.A1 = 1;//已确认
                                        item.Order.A7 = ChildPolicy.AirPoint; //航空公司返点
                                        //政策种类
                                        item.Order.A2 = ChildPolicy.PolicyKind;

                                        ////计算订单金额;
                                        //item.Order.PayMoney = d.CreateOrderPayMoney(item.Order, item.PasList);
                                        ////出票方收款金额
                                        //item.Order.OrderMoney = d.CreateOrderOrderMoney(item.Order, item.PasList);

                                        bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);

                                        //出票Office
                                        if (ChildPolicy.PolicyOffice != "")
                                        {
                                            item.Order.PrintOffice = ChildPolicy.PolicyOffice;
                                        }
                                        //---------------------------------------

                                        #endregion
                                    }
                                }

                                //代付返点，金额
                                if (item.Order.PolicySource > 2)
                                {
                                    // 接口 取原始政策
                                    item.Order.A7 = item.Order.OldPolicyPoint;
                                    item.Order.OutOrderPayMoney = d.CreateOrderIntfacePrice(item.Order, item.PasList);
                                }
                                else
                                {
                                    //本地 取航空公司政策
                                    decimal tempOldPolicyPoint = item.Order.OldPolicyPoint;

                                    item.Order.OldPolicyPoint = item.Order.A7;
                                    item.Order.OutOrderPayMoney = d.CreateOrderIntfacePrice(item.Order, item.PasList);
                                    item.Order.OldPolicyPoint = tempOldPolicyPoint;
                                }

                                item.Order.A13 = d.CreateOrderIntfacePrice(item.Order, item.PasList);// 后返金额

                                #endregion

                                #region 添加订单账单明细sql
                                List<string> sqlList = bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);
                                if (sqlList != null && sqlList.Count > 0)
                                {
                                    IsBillOK = true;
                                    InputParam.ExecSQLList.AddRange(sqlList.ToArray());
                                }
                                #endregion
                            }//For结束
                            //订单金额是否有误
                            bool IsOrderPayZero = false;
                            foreach (OrderMustParamModel item in InputParam.OrderParamModel)
                            {
                                //判断金额是否正确
                                if (item.Order.PayMoney <= 0 || ((item.Order.PayMoney + item.Order.PayMoney * 0.003M) < item.Order.OrderMoney))
                                {
                                    IsOrderPayZero = true;
                                    PbProject.WebCommon.Log.Log.RecordLog("OrderServices", "PayMoneyError|" + ErrMsg + "订单：PayMoney=" + item.Order.PayMoney + " OrderMoney=" + item.Order.OrderMoney + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                                    //  DataBase.LogCommon.Log.Error("OrderServices", new Exception(ErrMsg + "订单：PayMoney=" + item.Order.PayMoney + " OrderMoney=" + item.Order.OrderMoney + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray())));
                                    break;
                                }
                            }
                            #region 验证和修改订单
                            if (!IsBillOK)
                            {
                                ErrMsg = "订单生成失败！！";
                                PbProject.WebCommon.Log.Log.RecordLog("OrderServices", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ErrMsg + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                            }
                            else
                            {
                                if (!IsOrderPayZero)
                                {
                                    //修改订单有关实体信息
                                    IsSuc = orderBLL.UpdateOrder(ref InputParam, out ErrMsg);
                                    if (IsSuc)
                                    {
                                        ErrMsg = "订单生成成功！" + sbTip.ToString();
                                    }
                                    else
                                    {
                                        PbProject.WebCommon.Log.Log.RecordLog("OrderServices", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ErrMsg + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                                        ErrMsg = "订单生成失败！";
                                    }
                                }
                                else
                                {
                                    PbProject.WebCommon.Log.Log.RecordLog("OrderServices", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ErrMsg + "SQL:" + string.Join("\r\n  ", InputParam.ExecSQLList.ToArray()), true, HttpContext.Current.Request);
                                    ErrMsg = "订单金额错误,生成订单失败！";
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        ErrMsg = "未获取到政策数据，生成订单失败！";
                    }
                }
                catch (Exception ex)
                {
                    PbProject.WebCommon.Log.Log.RecordLog("OrderServices", "时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ErrMsg, true, HttpContext.Current.Request);
                    ErrMsg = "订单生成异常," + ex.Message;
                    DataBase.LogCommon.Log.Error("OrderServices" + userLogin.User.id.ToString(), ex);
                }
                finally
                {
                    if (IsSuc)
                    {
                        writer.Write("ok");
                    }
                    else
                    {
                        writer.WriteEx(567, "confirm Order Error", ErrMsg);
                    }
                }
                #endregion
            }















        }
        #region Create 私有方法
        private OrderInputParam createOrderInputParam(string adultOrderId, string childOrderId)
        {
            Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();
            OrderInputParam oip = new OrderInputParam();
            if (!string.IsNullOrEmpty(adultOrderId))
            {
                string ErrMsg = "";
                //成人订单数据显示            
                oip = OrderBLL.GetOrder(adultOrderId, oip, out ErrMsg);
            }
            if (!string.IsNullOrEmpty(childOrderId))
            {
                //儿童订单数据显示
                string ErrMsg = "";
                //儿童订单数据显示            
                oip = OrderBLL.GetOrder(childOrderId, oip, out ErrMsg);
            }
            return oip;
        }
        private bool HasBaby(OrderInputParam adultPasList)
        {
            return adultPasList.OrderParamModel.Where(p => p.PasList.Where(q => q.PassengerType == 3).Count() != 0).Count() != 0;

        }
        public PatInfo GetINFPAT()
        {
            string strPrice = "0|0|0@0|0|0@0|0|0";
            string[] strArr = strPrice.Split('@');
            PatInfo pat = null;
            if (strArr.Length > 0)
            {
                string[] strData = strArr[0].Split('|');
                if (strData.Length == 3)
                {
                    pat = new PatInfo();
                    decimal SeatPrice = 0m, ABFare = 0m, RQFare = 0m;
                    decimal.TryParse(strData[0], out SeatPrice);
                    decimal.TryParse(strData[1], out ABFare);
                    decimal.TryParse(strData[2], out RQFare);
                    pat.Fare = strData[0];
                    pat.TAX = strData[1];
                    pat.RQFare = strData[2];
                    pat.Price = (SeatPrice + ABFare + RQFare).ToString();
                    pat.PriceType = "3";
                }
            }
            return pat;
        }
        private void GetPassenger(OrderInputParam InputParam, List<Tb_Ticket_Passenger> AdultPasList, List<Tb_Ticket_Passenger> ChildPasList, RePnrObj pnrObj)
        {
            AdultPasList = InputParam.OrderParamModel.Where(p => !p.Order.IsChdFlag).Select(p => p.PasList).FirstOrDefault();
            ChildPasList = InputParam.OrderParamModel.Where(p => p.Order.IsChdFlag).Select(p => p.PasList).FirstOrDefault();
        }
        /// <summary>
        /// 白屏预订 乘客婴儿个数与编码中解析出来的婴儿个数比较是否一致 给出个提示
        /// </summary>
        /// <returns></returns>
        public bool yudingINFCheck(RePnrObj PnrInfo, List<Tb_Ticket_Passenger> PasList)
        {
            bool IsCheck = false;
            int INFCount = 0;
            int tempINFCount = 0;
            try
            {
                foreach (Tb_Ticket_Passenger pas in PasList)
                {
                    if (pas.PassengerType == 3)
                    {
                        INFCount++;
                    }
                }
                if (PnrInfo.PnrList.Length > 0 && PnrInfo.PnrList[0] != null)
                {
                    foreach (PassengerInfo item in PnrInfo.PnrList[0]._PassengerList)
                    {
                        if (item.PassengerType == "3")
                        {
                            tempINFCount++;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (INFCount != tempINFCount)
                {
                    IsCheck = true;
                }
            }
            return IsCheck;
        }
        private List<Tb_Ticket_SkyWay> getSkyWay(PnrModel pnrModel, PolicyParamForAPI policy)
        {
            var result = pnrModel._LegList.Select((p) =>
            {
                var r = new Tb_Ticket_SkyWay()
                {
                    CarryCode = p.AirCode,
                    FlightCode = p.FlightNum,
                    FromDate = DateTime.Parse(p.FlyStartTime),
                    ToDate = DateTime.Parse(p.FlyEndTime),
                    FromCityCode = p.FromCode,
                    ToCityCode = p.ToCode,
                    Space = p.Seat,
                    Discount = policy.DiscountRate.ToString()
                };
                return r;
            }).ToList();
            return result;
        }

        private List<Tb_Ticket_Passenger> getPassenger(PnrModel pnrModel)
        {
            var result = pnrModel._PassengerList.Select((p) =>
            {
                var r = new Tb_Ticket_Passenger()
                {
                    PassengerName = p.PassengerName,
                    PassengerType = int.Parse(p.PassengerType),
                    Cid = p.SsrCardID,
                };
                return r;
            }).ToList();
            return result;
        }
        #endregion
    }
}