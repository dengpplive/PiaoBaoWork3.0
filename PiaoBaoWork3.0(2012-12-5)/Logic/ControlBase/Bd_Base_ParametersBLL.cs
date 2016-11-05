using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Dal.ControlBase;
using DataBase.Data;
using DataBase.Unique;
using PbProject.WebCommon.Utility;

namespace PbProject.Logic.ControlBase
{
    /// <summary>
    /// 参数信息
    /// </summary>
    public class Bd_Base_ParametersBLL
    {

        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        /// <summary>
        /// 查询,通过 域名 查询
        /// </summary>
        /// <param name="url"></param>
        /// <returns>对应参数信息</returns>
        public Bd_Base_Parameters GetParametersByUrl(string url)
        {
            Bd_Base_Parameters model = null;
            try
            {
                User_Company uCompany = new PbProject.Logic.Login().GetByURL(url);
                if (uCompany != null)
                {
                    List<Bd_Base_Parameters> parametersList = GetParametersListByCpyNo(uCompany.UninCode);
                    if (parametersList != null)
                    {
                        foreach (Bd_Base_Parameters bParameters in parametersList)
                        {
                            if (bParameters.SetName.ToUpper() == "CSSURL")
                            {
                                model = bParameters;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return model;
        }

        /// <summary>
        /// 查询,通过公司编号查询 对应参数信息
        /// </summary>
        /// <param name="cpyNo">公司编号</param>
        /// <returns></returns>
        public List<Bd_Base_Parameters> GetParametersListByCpyNo(string cpyNo)
        {
            //List<Bd_Base_Parameters> parametersNewList = new List<Bd_Base_Parameters>();
            //try
            //{
            //    //List<Bd_Base_Parameters> parametersList = new Dal.ControlBase.BaseData<Bd_Base_Parameters>().GetList();
            //    //foreach (Bd_Base_Parameters bParameters in parametersList)
            //    //{
            //    //    if (bParameters.CpyNo == cpyNo)
            //    //    {
            //    //        parametersNewList.Add(bParameters);
            //    //    }
            //    //}
            //}
            //catch (Exception)
            //{
            //}
            //return parametersNewList;

            return baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + cpyNo + "'" }) as List<Bd_Base_Parameters>;
        }


        /// <summary>
        /// 根据条件查询参数
        /// </summary>
        /// <param name="sqlwhere">条件</param>
        /// <returns></returns>
        public List<Bd_Base_Parameters> GetParametersListByWhere(string sqlwhere)
        {
            return baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { sqlwhere }) as List<Bd_Base_Parameters>;
        }
        /// <summary>
        /// 构造用户发送指令的配置信息
        /// </summary>
        /// <param name="baseParametersList"></param>
        /// <returns></returns>
        public static ConfigParam GetConfigParam(List<Bd_Base_Parameters> baseParametersList)
        {
            return GetConfigParam(BaseParams.getParams(baseParametersList).HeiPingCanShu, BaseParams.getParams(baseParametersList).DaPeiZhiCanShu);
        }
        /// <summary>
        /// 获取供应配置信息
        /// </summary>
        /// <param name="strHeiPingCanShu"></param>
        /// <param name="strDaPeiZhiCanShu"></param>
        /// <returns></returns>
        public static ConfigParam GetConfigParam(string strHeiPingCanShu, string strDaPeiZhiCanShu)
        {
            ConfigParam configparam = new ConfigParam();
            try
            {
                string[] HeiPingCanShu = strHeiPingCanShu.Split('|');
                string[] DaPeiZhiCanShu = strDaPeiZhiCanShu.Split('|');
                //大配置信息
                configparam.BigCfgIP = DaPeiZhiCanShu[0];
                configparam.BigCfgPort = DaPeiZhiCanShu[1];
                configparam.BigCfgOffice = DaPeiZhiCanShu[2];
                configparam.BigCfgAccount = DaPeiZhiCanShu[3];
                //小配置信息
                configparam.WebBlackIP = HeiPingCanShu[0];
                configparam.WebBlackPort = HeiPingCanShu[1];
                configparam.WhiteScreenIP = HeiPingCanShu[2];
                configparam.WhiteScreenPort = HeiPingCanShu[3];
                configparam.Office = HeiPingCanShu[4];
                configparam.WebBlackAccount = HeiPingCanShu[5];
                configparam.WebBlackPwd = HeiPingCanShu[6];
                configparam.ECPort = HeiPingCanShu[7];
                configparam.TicketCompany = HeiPingCanShu[8];
                configparam.IataCode = HeiPingCanShu[9];
                if (HeiPingCanShu.Length > 10)
                {
                    string pidkeyno = HeiPingCanShu[10];
                    string[] strArr = pidkeyno.Split('^');
                    if (strArr.Length == 2)
                    {
                        configparam.Pid = strArr[0];
                        configparam.Pid = strArr[1];
                    }
                }
            }
            catch (Exception)
            {
            }
            return configparam;
        }

        /// <summary>
        /// 通过参数返回数据
        /// </summary>
        /// <param name="apmod">获取政策匹配中传过来的政策数据</param>
        /// <param name="userParametersList">当前登录用户公司参数信息</param>
        /// <param name="supBaseParametersList">落地运营商和供应商公司参数信息</param>
        /// <returns></returns>
        public AjaxPolicyMatchOutData GetAjaxPolicyMatchOutDataNew(AjaxPolicyMatchOutData apmod,
             List<PbProject.Model.Bd_Base_Parameters> userParametersList,
            List<PbProject.Model.Bd_Base_Parameters> supBaseParametersList)
        {
            try
            {
                if (apmod != null && apmod.OutPutPolicyList != null && apmod.OutPutPolicyList.Count > 0)
                {
                    string wangYinZhangHao = PbProject.Model.definitionParam.paramsName.wangYinZhangHao; //参数名
                    string wangYinLeiXing = PbProject.Model.definitionParam.paramsName.wangYinLeiXing; //参数名
                   
                    string payTypes = ""; //对应政策的支付方式
                    bool IsPolicySource = false; //判断是否有共享

                    string temp = "";//变量
                    string CPCpyNoS = "";//变量
                   
                    foreach (AjAxPolicyParam items in apmod.OutPutPolicyList)
                    {
                        temp = "'" + items.CPCpyNo + "'";

                        if (!CPCpyNoS.Contains(temp))
                            CPCpyNoS += temp + ",";

                        if (items.PolicySource == "9")
                            IsPolicySource = true;
                    }

                    List<Bd_Base_Parameters> bParametersList = null; //有共享政策时，读取共享出票方的参数
                    if (IsPolicySource == true)
                    {
                        CPCpyNoS = CPCpyNoS.TrimEnd(',');
                        string sqlWhere = " CpyNo in (" + CPCpyNoS + ") and (SetName='" + wangYinZhangHao + "' or SetName='" + wangYinLeiXing + "')";
                        bParametersList = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL().GetParametersListByWhere(sqlWhere);
                    }

                    string strGongYingKongZhiFenXiao = PbProject.WebCommon.Utility.BaseParams.getParams(userParametersList).GongYingKongZhiFenXiao;

                    Bd_Base_Parameters zhangHao = null;//参数变量
                    Bd_Base_Parameters leiXing = null;//参数变量

                    foreach (AjAxPolicyParam item in apmod.OutPutPolicyList)
                    {
                        zhangHao = null;
                        leiXing = null;
                        payTypes = "|";
                        temp = "";

                        if (item.PolicySource == "9")
                        {
                            #region 获取账号
                            foreach (Bd_Base_Parameters bParameter in bParametersList)
                            {
                                if (bParameter.CpyNo == item.CPCpyNo)
                                {
                                    if (bParameter.SetName == wangYinZhangHao)
                                        zhangHao = bParameter;
                                    else if (bParameter.SetName == wangYinLeiXing)
                                        leiXing = bParameter;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region 获取账号

                            foreach (Bd_Base_Parameters baseParameter in supBaseParametersList)
                            {
                                if (baseParameter.SetName == wangYinZhangHao)
                                    zhangHao = baseParameter;
                                else if (baseParameter.SetName == wangYinLeiXing)
                                    leiXing = baseParameter;
                            }

                            #endregion

                            #region 本地支付

                            if (strGongYingKongZhiFenXiao != null)
                            {
                                if (strGongYingKongZhiFenXiao.Contains("|76|"))
                                    payTypes += "5|";//账户余额支付
                                if ((item.PolicySource == "1" || item.PolicySource == "2") && strGongYingKongZhiFenXiao.Contains("|79|"))
                                    payTypes += "6|";//收银
                            }

                            #endregion
                        }

                        #region 在线支付
                        if (zhangHao != null && !string.IsNullOrEmpty(zhangHao.SetValue) && zhangHao.SetValue.Contains("|"))
                        {
                            string[] setValues = zhangHao.SetValue.Split('|');

                            string[] setValue0 = setValues[0].Split('^');
                            if (!string.IsNullOrEmpty(setValue0[0]))//支付宝
                            {
                                payTypes += "1|";//支付宝
                                temp += "5,";
                            }
                            string[] setValue1 = setValues[1].Split('^');
                            if (!string.IsNullOrEmpty(setValue1[0]))
                            {
                                //快钱
                                payTypes += "2|";
                                temp += "6,";
                            }
                            string[] setValue2 = setValues[2].Split('^');
                            if (!string.IsNullOrEmpty(setValue2[0]))
                            {
                                //汇付
                                payTypes += "3|";
                                temp += "7,";
                            }
                            string[] setValue3 = setValues[3].Split('^');
                            if (!string.IsNullOrEmpty(setValue3[0]))
                            {
                                //财付通
                                payTypes += "4|";
                                temp += "8,";
                            }
                        }

                        if (leiXing != null && !string.IsNullOrEmpty(leiXing.SetValue) && temp.Contains(leiXing.SetValue))
                            payTypes += "0|";//网银

                        #endregion

                        item.PayType = payTypes; //赋值
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return apmod;
        }
    }
}