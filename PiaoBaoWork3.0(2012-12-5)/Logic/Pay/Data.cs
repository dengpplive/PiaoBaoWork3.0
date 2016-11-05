using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.Order;
using PbProject.Dal.ControlBase;
using PbProject.Logic.ControlBase;

namespace PbProject.Logic.Pay
{
    /// <summary>
    /// 数据处理
    /// </summary>
    public class Data
    {

        #region 参数

        /// <summary>
        /// 采购佣金进舍规则:0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分
        /// </summary>
        public int CommissionType = 0;

        #region 对外接口使用

        public Data(List<PbProject.Model.Bd_Base_Parameters> supParameters)
        {
            foreach (Bd_Base_Parameters item in supParameters)
            {
                if (item.SetName == "setCommission")
                {
                    this.CommissionType = int.Parse(item.SetValue);
                    break;
                }
            }
        }

        #endregion

        public Data()
        {
            try
            {
                //string currentuserid = System.Web.HttpContext.Current.Request["currentuserid"] ?? string.Empty;

                //if (string.IsNullOrEmpty(currentuserid))
                // {
                //SessionContent sessionContent = System.Web.HttpContext.Current.Session[System.Web.HttpContext.Current.Session["Uid"].ToString()] as SessionContent;//保存用户信息
                SessionContent sessionContent = System.Web.HttpContext.Current.Application[System.Web.HttpContext.Current.Session["Uid"].ToString()] as SessionContent;//保存用户信息
                if (sessionContent != null && sessionContent.SupBASEPARAMETERS != null && sessionContent.SupBASEPARAMETERS.Count > 0)
                {
                    List<PbProject.Model.Bd_Base_Parameters> SupParameters = sessionContent.SupBASEPARAMETERS;//落地运营商和供应商公司参数信息

                    foreach (Bd_Base_Parameters item in SupParameters)
                    {
                        if (item.SetName == "setCommission")
                        {
                            CommissionType = int.Parse(item.SetValue);
                            break;
                        }
                    }
                }
                //}
                //else
                //{
                //    System.Web.HttpContext.Current.Response.Redirect("~/Login.aspx");
                //}
            }
            catch (Exception ex)
            {

            }
        }
        DataAction dataAction = null;
        public Data(string cpyno)
        {
            try
            {
                this.dataAction = new DataAction();
                BaseDataManage baseDataManager = new BaseDataManage();
                string No = "000000";
                if (cpyno.Length >= 12)
                    No = cpyno.Substring(0, 12);
                List<Bd_Base_Parameters> SupParameters = baseDataManager.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + No + "'" }) as List<Bd_Base_Parameters>;
                if (SupParameters != null)
                {
                    foreach (Bd_Base_Parameters item in SupParameters)
                    {
                        if (item.SetName == "setCommission")
                        {
                            CommissionType = int.Parse(item.SetValue);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        ///// <summary>
        ///// 采购佣金进舍规则: 0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分
        ///// </summary>
        ///// <param name="type"> 采购佣金进舍规则:0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分</param>
        //public Data(int type)
        //{
        //    CommissionType = type;
        //}

        #endregion

        
        #region 本地计算

        /// <summary>
        /// (佣金)分销扣点: 单人计算(只舍不入保留1位小数)
        /// </summary>
        /// <param name="PMFee">舱位价</param>
        /// <param name="returnPoint">扣点</param>
        /// <param name="returnMoney">现返</param>
        /// <param name="passengerType">乘机人类型</param>
        /// <returns></returns>
        public decimal CreateCommissionFX(decimal PMFee, decimal returnPoint, decimal returnMoney, int passengerType)
        {
            decimal commissionPayFee = 0;

            try
            {
                ////1=成人，2=儿童，3=婴儿
                if (passengerType == 3)
                {
                    commissionPayFee = 0;  //婴儿 没有佣金
                }
                else
                {
                    commissionPayFee = dataAction.MinusCeilSmallNumOne(PMFee * returnPoint / 100 + returnMoney);//收款金额 = (舱位价 * 扣点）(只舍不入保留1位小数)
                }
            }
            catch (Exception)
            {

            }

            return commissionPayFee;
        }

        /// <summary>
        /// 佣金算法(采购 和 供应 同步算法):单人计算 （规则(3种):0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分）
        /// </summary>
        /// <param name="PMFee"></param>
        /// <param name="returnPoint"></param>
        /// <returns></returns>
        public decimal CreateCommissionCG(decimal PMFee, decimal returnPoint)
        {
            decimal commissionPayFee = 0;

            try
            {
                #region 采购佣金

                // 采购佣金进舍规则:0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分
                if (CommissionType == 0)
                {
                    commissionPayFee = dataAction.MinusCeilNum(PMFee * returnPoint / 100);
                }
                else if (CommissionType == 1)
                {
                    commissionPayFee = dataAction.MinusCeilSmallNumOne(PMFee * returnPoint / 100); //单人佣金
                }
                else if (CommissionType == 2)
                {
                    commissionPayFee = dataAction.MinusCeilSmallNum(PMFee * returnPoint / 100); //单人佣金
                }

                #endregion
            }
            catch (Exception)
            {

            }

            return commissionPayFee;
        }

        #region 单人金额计算：付款方

        /// <summary>
        /// 现返金额:单人计算 (通过扣点关系计算) （规则(3种):0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分）
        /// </summary>
        /// <param name="deDetail">扣点明细：(账号^扣点^现返^.......|账号^扣点^现返^....... ) 格式：账号^1^1|账号^1^2|</param>
        /// <param name="orderPolicyXf">订单政策现返</param>
        /// <returns></returns>
        public decimal CreateXianFanPayFee(string deDetail, decimal orderPolicyXf)
        {
            decimal xianFanPayFee = orderPolicyXf * -1; // 订单现返

            try
            {
                if (!string.IsNullOrEmpty(deDetail))
                {
                    string[] deDetails = deDetail.Split('|');

                    decimal tempFanPayFee = 0;

                    for (int i = 0; i < deDetails.Length; i++)
                    {
                        if (deDetails[i] != "")
                        {
                            string[] details = deDetails[i].Split('^');

                            tempFanPayFee = decimal.Parse(details[2]);


                            // 采购佣金进舍规则:0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分
                            if (CommissionType == 0)
                            {
                                tempFanPayFee = dataAction.MinusCeilNum(tempFanPayFee);
                            }
                            else if (CommissionType == 1)
                            {
                                tempFanPayFee = dataAction.MinusCeilSmallNumOne(tempFanPayFee); //单人佣金
                            }
                            else if (CommissionType == 2)
                            {
                                tempFanPayFee = dataAction.MinusCeilSmallNum(tempFanPayFee); //单人佣金
                            }

                            xianFanPayFee += tempFanPayFee;

                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return xianFanPayFee;
        }

        /// <summary>
        /// 单人支付票价计算： 
        /// </summary>
        /// <param name="PMFee">舱位价</param>
        /// <param name="ABFee">基建</param>
        /// <param name="FuelFee">燃油</param>
        /// <param name="returnPoint">最终返点</param>
        /// <param name="returnPoint">现返</param>
        /// <param name="passengerType">乘机人类型</param>
        /// <returns></returns>
        public decimal CreatePassengerPayFee(decimal PMFee, decimal ABFee, decimal FuelFee, decimal returnPoint, decimal xianFanPayFee, int passengerType)
        {
            decimal passengerPayFee = 0;
            try
            {
                decimal yongJinPayFee = 0;

                ////1=成人，2=儿童，3=婴儿
                if (passengerType == 3)
                {
                    xianFanPayFee = 0;  //婴儿 没有佣金
                }
                else
                {
                    yongJinPayFee = CreateCommissionCG(PMFee, returnPoint);
                }

                //单人机票价格 = 单人舱位价 + 单人基建 + 单人燃油 - 单人佣金(优惠) + 单人现返
                passengerPayFee = PMFee + ABFee + FuelFee - yongJinPayFee + xianFanPayFee;
            }
            catch (Exception)
            {

            }
            return passengerPayFee;
        }

        #endregion

        #region 单人金额计算：收款方

        /// <summary>
        /// 收款方(供应):单人收款计算 (规则： 四舍五入 保留2位 ）
        /// </summary>
        /// <param name="PMFee">舱位价</param>
        /// <param name="ABFee">基建</param>
        /// <param name="FuelFee">燃油</param>
        /// <param name="returnPoint">政策原始点</param>
        /// <param name="returnPoint">现返</param>
        /// <param name="passengerType">乘机人类型</param>
        /// <returns></returns>
        public decimal CreateOrderPayFeeGY(decimal PMFee, decimal ABFee, decimal FuelFee, decimal returnPoint, decimal xianFanPayFee, int passengerType)
        {
            decimal passengerPayFee = 0;
            try
            {
                decimal commissionPayFee = 0;

                ////1=成人，2=儿童，3=婴儿
                if (passengerType == 3)
                {
                    commissionPayFee = 0;  //婴儿 没有佣金

                    //单人机票价格 = 单人舱位价 + 单人基建 + 单人燃油
                    passengerPayFee = PMFee + ABFee + FuelFee;
                }
                else
                {
                    commissionPayFee = CreateCommissionCG(PMFee, returnPoint);
                    //commissionPayFee = FourToFiveNum(PMFee * returnPoint / 100, 2); //单人佣金 ：四舍五入 保留2位

                    //单人机票价格 = 单人舱位价 + 单人基建 + 单人燃油 - 单人佣金(优惠) - 单人现返
                    passengerPayFee = PMFee + ABFee + FuelFee - commissionPayFee - xianFanPayFee;
                }
            }
            catch (Exception)
            {

            }
            return passengerPayFee;
        }

        #endregion

        #region 订单金额计算

        /*
        /// <summary>
        /// 订单支付金额:付款方 多人计算
        /// </summary>
        /// <param name="mOrder">订单 mOrder ：订单最终返点、订单扣点明细</param>
        /// <param name="mPassengers">乘机人 mOrder：舱位价、基建、燃油</param>
        public decimal CreateOrderPayMoney(Tb_Ticket_Order mOrder, List<Model.Tb_Ticket_Passenger> mPassengers)
        {
            decimal orderPayFee = 0; //订单支付金额
            try
            {
                if (mOrder != null && mPassengers != null && mPassengers.Count > 0)
                {
                    decimal returnPoint = mOrder.ReturnPoint; //实际返点（扣点后）
                    decimal XianFanPayFee = CreateXianFanPayFee(mOrder.DiscountDetail, mOrder.OldReturnMoney); //单人现返金额

                    foreach (Tb_Ticket_Passenger item in mPassengers)
                    {
                        orderPayFee += CreatePassengerPayFee(item.PMFee, item.ABFee, item.FuelFee, returnPoint, XianFanPayFee,item.PassengerType);
                    }
                }
            }
            catch (Exception)
            {

            }
            return orderPayFee;
        }

        /// <summary>
        /// 出票方收款金额:收款方 多人计算
        /// </summary>
        /// <param name="mOrder">订单 mOrder ：订单最终返点、订单扣点明细</param>
        /// <param name="mPassengers">乘机人 mOrder：舱位价、基建、燃油</param>
        public decimal CreateOrderOrderMoney(Tb_Ticket_Order mOrder, List<Model.Tb_Ticket_Passenger> mPassengers)
        {
            decimal orderPayFee = 0; //订单支付金额
            try
            {
                if (mOrder != null && mPassengers != null && mPassengers.Count > 0)
                {
                    decimal returnPoint = mOrder.PolicyPoint; //实际返点（扣点后）

                    decimal XianFanPayFee = mOrder.ReturnMoney; //政策现返

                    foreach (Tb_Ticket_Passenger item in mPassengers)
                    {
                        orderPayFee += CreateOrderPayFeeGY(item.PMFee, item.ABFee, item.FuelFee, returnPoint, XianFanPayFee, item.PassengerType);
                    }
                }
            }
            catch (Exception)
            {

            }
            return orderPayFee;
        }
        */

        /// <summary>
        /// 后返计算
        /// </summary>
        /// <param name="mOrder">订单 mOrder</param>
        /// <param name="mPassengers">乘机人 mOrder</param>
        /// <returns></returns>
        public decimal CreateOrderHFMoney(Tb_Ticket_Order mOrder, List<Model.Tb_Ticket_Passenger> mPassengers)
        {
            decimal orderHFMoney = 0; //金额
            try
            {
                if (mOrder != null && mPassengers != null && mPassengers.Count > 0)
                {
                    decimal returnPoint = mOrder.LaterPoint; //后返点

                    foreach (Tb_Ticket_Passenger item in mPassengers)
                    {
                        if (item.PassengerType != 3)
                        {
                            orderHFMoney += dataAction.MinusCeilSmallNum(item.PMFee * returnPoint / 100); //累计后返金额
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return orderHFMoney;
        }

        #endregion

        #endregion

        #region 接口计算

        /// <summary>
        /// 接口代付金额:单人收款计算 (规则： 四舍五入 保留2位 ）
        /// </summary>
        /// <param name="PMFee">舱位价</param>
        /// <param name="ABFee">基建</param>
        /// <param name="FuelFee">燃油</param>
        /// <param name="returnPoint">政策原始点</param>
        /// <param name="returnPoint">现返</param>
        /// <param name="passengerType">乘机人类型</param>
        /// <returns></returns>
        public decimal CreatePayFeeIntfacePrice(decimal PMFee, decimal ABFee, decimal FuelFee, decimal returnPoint, decimal xianFanPayFee, int passengerType)
        {
            decimal passengerPayFee = 0;
            try
            {
                decimal commissionPayFee = 0;

                ////1=成人，2=儿童，3=婴儿
                if (passengerType == 3)
                {
                    commissionPayFee = 0;  //婴儿 没有佣金

                    //单人机票价格 = 单人舱位价 + 单人基建 + 单人燃油
                    passengerPayFee = PMFee + ABFee + FuelFee;
                }
                else
                {
                    commissionPayFee =dataAction.FourToFiveNum(PMFee * returnPoint / 100, 2); //单人佣金 ：四舍五入 保留2位

                    //单人机票价格 = 单人舱位价 + 单人基建 + 单人燃油 - 单人佣金(优惠) - 单人现返
                    passengerPayFee = PMFee + ABFee + FuelFee - commissionPayFee - xianFanPayFee;
                }
            }
            catch (Exception)
            {

            }
            return passengerPayFee;
        }

        /// <summary>
        /// 订单接口支付金额:
        /// </summary>
        /// <param name="mOrder">订单 mOrder ：订单最终返点、订单扣点明细</param>
        /// <param name="mPassengers">乘机人 mOrder：舱位价、基建、燃油</param>
        public decimal CreateOrderIntfacePrice(Tb_Ticket_Order mOrder, List<Model.Tb_Ticket_Passenger> mPassengers)
        {
            decimal orderPayFee = 0; //订单支付金额
            try
            {
                decimal returnPoint = mOrder.OldPolicyPoint; //接口原始政策
                decimal XianFanPayFee = 0; //现返 接口不计算

                if (mOrder != null && mPassengers != null && mPassengers.Count > 0)
                {
                    foreach (Tb_Ticket_Passenger item in mPassengers)
                    {
                        orderPayFee += CreatePayFeeIntfacePrice(item.PMFee, item.ABFee, item.FuelFee, returnPoint, XianFanPayFee, item.PassengerType);
                    }
                }
            }
            catch (Exception)
            {

            }
            return orderPayFee;
        }

        #endregion
    }
    public class DataAction
    { 
    #region 数据处理

        /// <summary>
        /// 保留两位小数（只入不舍）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal AddCeilNum(decimal del)
        {
            decimal _del = Math.Round(del + 0.005M - 0.0001M, 2);
            return _del;
        }

        /// <summary>
        /// 保留一位小数（只舍不入）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal MinusCeilSmallNumOne(decimal del)
        {
            decimal _del = Math.Round(del - 0.05M + 0.001M, 1);
            return _del;
        }

        /// <summary>
        /// 保留两位小数（只舍不入）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal MinusCeilSmallNum(decimal del)
        {
            decimal _del = Math.Round(del - 0.005M + 0.0001M, 2);
            return _del;
        }

        /// <summary>  
        /// 实现数据的四舍五入法, 保留小数
        /// </summary>  
        /// <param name="v">要进行处理的数据</param>  
        /// <param name="x">保留的小数位数</param>   
        /// <returns>四舍五入后的结果</returns>   
        public decimal FourToFiveNum(decimal v, int x)
        {
            //decimal _del = Math.Round(del, 2); //四舍五入
            //return _del;

            decimal _del = Math.Round(v + 0.0000001M, x);// //四舍五入
            return _del;
        }

        /// <summary>
        /// 保留整数（只入不舍）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal AddCeilNumInteger(decimal del)
        {
            //decimal _del = Math.Round(del + 0.5M - 0.01M, 0);
            //return _del;

            decimal result = del;
            try
            {
                if (del != 0 && del.ToString().Contains("."))
                {
                    string[] str = del.ToString().Split('.');
                    result = decimal.Parse(str[0]);

                    if (decimal.Parse("0." + str[1]) > 0)
                    {
                        result += 1;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 保留整数（只舍不入）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal MinusCeilNum(decimal del)
        {
            //decimal _del = Math.Round(del - 0.5M + 0.01M, 0);
            //return _del;

            decimal result = del;
            try
            {
                if (del != 0 && del.ToString().Contains("."))
                {
                    string[] str = del.ToString().Split('.');
                    result = decimal.Parse(str[0]);
                }
            }
            catch (Exception ex)
            {

            }
            return result;

        }

        /// <summary>
        /// 四舍五入到十位
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal MinusCeilTen(decimal del)
        {
            del = del / 10;
            string[] dels = del.ToString().Split('.');
            int temp = dels.Length > 1 && int.Parse(dels[1].Substring(0, 1)) >= 5 ? 1 : 0;
            decimal _del = decimal.Parse(dels[0]) + temp;
            _del = _del * 10;
            return _del;
        }

        #endregion

    }
}
