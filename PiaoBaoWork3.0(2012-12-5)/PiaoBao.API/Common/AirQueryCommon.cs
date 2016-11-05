using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.API.Common.Enum;
using System.Data;
using PbProject.Logic.Buy;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using PiaoBao.API.Common.AirQuery;

namespace PiaoBao.API.Common
{
    /// <summary>
    /// 航班查询公共类
    /// </summary>
    public class AirQueryCommon
    {

        #region 外部公有方法

        /// <summary>
        /// 是否是特价仓位
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="space">舱位</param>
        /// <returns>是否是特价舱位</returns>
        public bool IsSpecialSpace(UserLoginInfo userInfo, string space, string carryCode)
        {
            bool isSpecialSpace;
            BaseDataManage Manage = new BaseDataManage();
            AirQurey airQuery = new AirQurey(userInfo.BaseParametersList, userInfo.User, userInfo.Company);
            string GCpyNo = userInfo.Company.UninCode.Substring(0, 12);//获取上级供应商或落地运营商的ID
            string sqlwhere = " 1=1 "
                + " and CpyNo='" + GCpyNo + "'";
            List<Tb_SpecialCabin> objList = Manage.CallMethod("Tb_SpecialCabin", "GetList", null, new object[] { sqlwhere }) as List<Tb_SpecialCabin>;
            isSpecialSpace = airQuery.juageSpecialType(carryCode, space, objList);
            return isSpecialSpace;
        }

        /// <summary>
        /// 查询航班
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="startCity">出发城市三字码</param>
        /// <param name="midCity">中转城市三字码</param>
        /// <param name="endCity">到达城市三字码</param>
        /// <param name="startDate">出发时间</param>
        /// <param name="backDate">返回时间</param>
        /// <param name="airTravelType">行程类型</param>
        /// <param name="cairry">承运人(航空公司)</param>
        /// <param name="isQueryShareFlight">是否查询共享航班</param>
        /// <param name="isQuerySpecialFlight">是否查询特价航班</param>
        /// <returns>返回AirInfoCollection类型List对象</returns>
        public AirInfoCollectionList GetAirQueryJSon(UserLoginInfo userInfo, string startCity, string midCity, string endCity, string startDate,
            string backDate, AirTravelType airTravelType, string cairry, bool isQueryShareFlight, bool isQuerySpecialFlight)
        {
            AirInfoCollectionList airInfoCollectionList = null;
            List<AirInfoCollection> list = null;
            string cacheGUID;
            if (CheckParams(startCity, midCity, endCity, startDate, backDate, airTravelType))//检查参数安全性
            {
                airInfoCollectionList = new AirInfoCollectionList();
                DataSet ds_second_Flight;
                DataSet ds_Flight = AirQueryDataSet(userInfo, startCity, midCity, endCity, startDate, backDate, airTravelType, cairry, isQueryShareFlight, isQuerySpecialFlight, out cacheGUID, out ds_second_Flight);
                if (airTravelType == AirTravelType.OneWay)//单程才匹配政策
                {
                    DataSet ds_Policy = GetPolicyDataSet(userInfo, startCity, midCity, endCity, startDate, backDate, airTravelType, cacheGUID, true, false, false);
                    DataSet ds_Combine = CombineFlightAndPolicyDataSet(ds_Flight, ds_Policy);
                    list = DataSetToList(ds_Combine, airTravelType);//转换航班
                    airInfoCollectionList.FirstAirInfoList = list;
                }
                else
                {
                    List<AirInfoCollection> list_second = null;
                    list = DataSetToList(ds_Flight, airTravelType);//转换第一次航班
                    list_second = DataSetToList(ds_second_Flight, airTravelType);//转换第二次航班
                    airInfoCollectionList.FirstAirInfoList = list;
                    airInfoCollectionList.SecondAirInfoList = list_second;
                }
                airInfoCollectionList.CacheNameGuid = cacheGUID;
            }
            return airInfoCollectionList;
        }

        #endregion

        #region 内部私有方法

        /// <summary>
        /// 查询航班
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="startCity">出发城市三字码</param>
        /// <param name="midCity">中转城市三字码</param>
        /// <param name="endCity">到达城市三字码</param>
        /// <param name="startDate">出发时间</param>
        /// <param name="backDate">返回时间</param>
        /// <param name="airTravelType">行程类型</param>
        /// <param name="cairry">承运人(航空公司)</param>
        /// <param name="isQueryShareFlight">是否查询共享航班</param>
        /// <param name="isQuerySpecialFlight">是否查询特价航班</param>
        /// <param name="cacheGUID">out 缓存guid 键</param>
        /// <returns>返回DataSet格式航班信息</returns>
        private DataSet AirQueryDataSet(UserLoginInfo userInfo, string startCity, string midCity, string endCity, string startDate,
            string backDate, AirTravelType airTravelType, string cairry, bool isQueryShareFlight, bool isQuerySpecialFlight,
            out string cacheGUID, out DataSet ds_ConnOrRound)
        {
            ds_ConnOrRound = null;
            DataSet ds_Flight = null;
            try
            {
                #region 变量申明及参数赋值

                int num = 0;
                AirQurey airQuery = new AirQurey(userInfo.BaseParametersList, userInfo.User, userInfo.Company);
                string begintime = DateTime.Now.ToString("mm:ss:fff");
                PbProject.Model.definitionParam.SelectCityParams selectCityParams = new PbProject.Model.definitionParam.SelectCityParams();
                selectCityParams.fcity = startCity;
                selectCityParams.mcity = midCity;
                selectCityParams.tcity = endCity;
                selectCityParams.time = startDate;
                selectCityParams.Totime = backDate;
                selectCityParams.cairry = cairry;
                selectCityParams.TravelType = (int)airTravelType;
                selectCityParams.num = num;
                selectCityParams.mEmployees = userInfo.User;
                selectCityParams.mCompany = userInfo.Company;
                selectCityParams.IsShowGX = isQueryShareFlight;//true 不显示,false 显示

                #endregion

                if (airTravelType == AirTravelType.ConnWay)//联程
                {
                    selectCityParams.tcity = midCity;
                }
                ds_Flight = airQuery.StartForInterface(selectCityParams, out cacheGUID);//单程只查一次

                if (airTravelType == AirTravelType.RoundWay || airTravelType == AirTravelType.ConnWay)//往返联成的情况需要读取两次IBE数据
                {
                    if (airTravelType == AirTravelType.RoundWay)//往返
                    {
                        selectCityParams.fcity = endCity;
                        selectCityParams.tcity = startCity;
                    }
                    if (airTravelType == AirTravelType.ConnWay)//联程
                    {
                        selectCityParams.fcity = midCity;
                        selectCityParams.tcity = endCity;
                    }

                    selectCityParams.time = backDate;
                    airQuery = new AirQurey(userInfo.BaseParametersList, userInfo.User, userInfo.Company);
                    ds_ConnOrRound = airQuery.StartForInterface(selectCityParams, out cacheGUID);//查询航班
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds_Flight;
        }

        /// <summary>
        /// 获取政策
        /// </summary>
        /// <param name="startCity">出发城市</param>
        /// <param name="midCity">中转城市</param>
        /// <param name="endCity">到达城市</param>
        /// <param name="startDate">出发时间</param>
        /// <param name="backDate">到达时间</param>
        /// <param name="airTravelType">行程类型</param>
        /// <param name="cacheGUID">航班信息缓存ID</param>
        /// <param name="isQueryOrPolicy">是白屏还是获取政策 (查询航班页面定死为true)</param>
        /// <param name="orderID">订单ID</param>
        /// <param name="userInfo">用户信息对象</param>
        /// <param name="hasChild">是否有儿童(查询航班页面定死为false)</param>
        /// <returns>返回DataSet政策信息</returns>
        private DataSet GetPolicyDataSet(UserLoginInfo userInfo, string startCity, string midCity, string endCity, string startDate, string backDate,
            AirTravelType airTravelType, string cacheGUID, bool isQueryOrPolicy, bool hasChild, bool hasBaby)
        {
            PbProject.Logic.Policy.PolicyMatch plpp = new PbProject.Logic.Policy.PolicyMatch();
            DataSet ds = plpp.getMatchingPolicy(userInfo.Company.UninCode, startCity, midCity, endCity, startDate, backDate,
                ((int)airTravelType).ToString(), cacheGUID, isQueryOrPolicy, userInfo.Company.GroupId, string.Empty, userInfo.User, hasChild, hasBaby, false);
            return ds;
        }

        /// <summary>
        /// 合并航班和政策DataSet
        /// </summary>
        /// <param name="ds_Flight">航班DataSet</param>
        /// <param name="ds_Policy">政策DataSet</param>
        /// <returns>返回合并好的DataSet</returns>
        private DataSet CombineFlightAndPolicyDataSet(DataSet ds_Flight, DataSet ds_Policy)
        {
            DataSet ds = CreateFlightAndPolicyDataSet(ds_Flight);//创建空DataSet结构
            DataRow dr_temp;
            DataTable dt_temp;
            //现在理论上2个ds的顺序和row里的顺序都是相同一一对应的，但为了防止特殊情况，所以做此循环(性能有一定损失)。
            foreach (DataTable dt_Flight in ds_Flight.Tables)//DataTable匹配
            {
                foreach (DataTable dt_Policy in ds_Policy.Tables)
                {
                    if (dt_Flight.TableName.Trim() == dt_Policy.TableName.Trim())
                    {
                        foreach (DataRow dr_Flight in dt_Flight.Rows)//DataRow匹配
                        {
                            foreach (DataRow dr_Policy in dt_Policy.Rows)
                            {
                                if (dr_Flight["GUID"].ToString().Trim() == dr_Policy["GUID"].ToString().Trim())
                                {
                                    dt_temp = ds.Tables[dt_Flight.TableName];//存放DataTable引用
                                    dr_temp = dt_temp.NewRow();//在空DataSet中指定表创建一行
                                    CombineDataRow(dr_temp, dr_Flight, dr_Policy);//合并DataRow
                                    dt_temp.Rows.Add(dr_temp);//将DataRow添加到该DataTable
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// 合并DataRow
        /// </summary>
        /// <param name="dr">合并后存放的DataRow</param>
        /// <param name="dr_Flight">航班DataRow</param>
        /// <param name="dr_Policy">政策DataRow</param>
        private void CombineDataRow(DataRow dr, DataRow dr_Flight, DataRow dr_Policy)
        {
            #region 字段数组

            string[] arr_FieldOfFlight ={        
                "CarrCode",
                "Carrier",
                "FlightNo",
                "StartTime",
                "FromCity",
                "EndTime",
                "ToCity",
                "Model",
                "IsStop",
                "ABFee",
                "FuelAdultFee",
                "Space",
                "TickNum",
                "DiscountRate",
                "XSFee",
                "PMFee",
                "DishonoredBillPrescript",
                "LogChangePrescript",
                "UpCabinPrescript",
                "SpecialType",
                "aterm",
                "GUID"
                                        };
            string[] arr_FieldOfPolicy ={ 
                "HighPolicyFlag",
                "LaterPoint",
                "Policy",
                "ReturnMoney",
                "Commission",
                "SJFee",
                "GenerationType",
                "SeatPrice"

                                        };

            #endregion

            foreach (string str in arr_FieldOfFlight)
            {
                dr[str] = dr_Flight[str];
            }
            foreach (string str in arr_FieldOfPolicy)
            {
                dr[str] = dr_Policy[str];
            }
            if (dr["SpecialType"].ToString().Trim() == "1")//如果为动态特价，特价金额设置为“特价”
            {
                dr["SeatPrice"] = "特价";
            }
            dr["PolicyGUID"] = dr_Policy["PolicyId"];
        }

        /// <summary>
        /// 创建匹配好政策的航班DataSet结构
        /// </summary>
        /// <param name="ds_Flight">航班DataSet</param>
        /// <returns>返回创建好结构的DataSet</returns>
        private DataSet CreateFlightAndPolicyDataSet(DataSet ds_Flight)
        {
            DataSet ds = new DataSet();
            DataTable dt_temp;
            foreach (DataTable dt in ds_Flight.Tables)
            {
                dt_temp = CreateFlightAndPolicyDataTable(dt.TableName);//表名和航班表中保持一致
                ds.Tables.Add(dt_temp);
            }
            return ds;
        }

        #region 返回的DataSet中所有DataTable所需要的字段
        //查询航班需要字段
        //CarrCode 承运人编码
        //Carrier 承运人
        //FlightNo 航班编号
        //StartTime 起飞时间
        //FromCity 出发城市
        //EndTime 到达时间
        //ToCity 到达城市
        //Model 机型
        //IsStop 
        //ABFee 机建
        //FuelAdultFee 燃油
        //Space 舱位
        //TickNum 舱位数量
        //DiscountRate 折扣
        //XSFee 舱位价
        //PMFee 票面价
        //DishonoredBillPrescript 客规
        //LogChangePrescript
        //UpCabinPrescript
        //SpecialType 特价类型(0正常，1特价)
        //aterm
        //GUID

        //查询政策需要字段
        //GUID
        //HighPolicyFlag 是否高返
        //LaterPoint
        //Policy 政策佣金
        //ReturnMoney
        //Commission
        //SJFee 实付金额
        //GenerationType 特价类型(1正常，2动态特价，3固定特价) 动态特价金额写“特价”
        //SeatPrice 特价
        #endregion

        /// <summary>
        /// 创建匹配好政策的航班DataTable结构
        /// </summary>
        /// <param name="tableName">航班表表名</param>
        /// <returns>返回创建好结构的DataTable</returns>
        private DataTable CreateFlightAndPolicyDataTable(string tableName)
        {
            DataTable dt = new DataTable(tableName);
            dt.Columns.Add("CarrCode");
            dt.Columns.Add("Carrier");
            dt.Columns.Add("FlightNo");
            dt.Columns.Add("StartTime");
            dt.Columns.Add("FromCity");
            dt.Columns.Add("EndTime");
            dt.Columns.Add("ToCity");
            dt.Columns.Add("Model");
            dt.Columns.Add("IsStop");
            dt.Columns.Add("ABFee");
            dt.Columns.Add("FuelAdultFee");
            dt.Columns.Add("Space");
            dt.Columns.Add("TickNum");
            dt.Columns.Add("DiscountRate");
            dt.Columns.Add("XSFee");
            dt.Columns.Add("PMFee");
            dt.Columns.Add("DishonoredBillPrescript");
            dt.Columns.Add("LogChangePrescript");
            dt.Columns.Add("UpCabinPrescript");
            dt.Columns.Add("SpecialType");
            dt.Columns.Add("aterm");
            dt.Columns.Add("GUID");
            dt.Columns.Add("PolicyGUID");
            dt.Columns.Add("HighPolicyFlag");
            dt.Columns.Add("LaterPoint");
            dt.Columns.Add("Policy");
            dt.Columns.Add("ReturnMoney");
            dt.Columns.Add("Commission");
            dt.Columns.Add("SJFee");
            dt.Columns.Add("GenerationType");
            dt.Columns.Add("SeatPrice");
            return dt;
        }

        private List<AirInfoCollection> DataSetToList(DataSet ds, AirTravelType att)
        {
            List<AirInfoCollection> list = new List<AirInfoCollection>();
            AirInfoCollection airInfoCollection;
            AirInfo airInfo;
            foreach (DataTable dt in ds.Tables)
            {
                airInfoCollection = new AirInfoCollection() { FlightNum = dt.TableName };
                foreach (DataRow row in dt.Rows)
                {
                    airInfo = new AirInfo()
                    {
                        CarrCode = CommonMethod.GetFomartString(row["CarrCode"]),
                        Carrier = CommonMethod.GetFomartString(row["Carrier"]),
                        FlightNo = CommonMethod.GetFomartString(row["FlightNo"]),
                        StartTime = CommonMethod.GetFomartString(row["StartTime"]),
                        FromCity = CommonMethod.GetFomartString(row["FromCity"]),
                        EndTime = CommonMethod.GetFomartString(row["EndTime"]),
                        ToCity = CommonMethod.GetFomartString(row["ToCity"]),
                        Model = CommonMethod.GetFomartString(row["Model"]),
                        IsStop = CommonMethod.GetFomartString(row["IsStop"]),
                        ABFee = CommonMethod.GetFomartString(row["ABFee"]),
                        FuelAdultFee = CommonMethod.GetFomartString(row["FuelAdultFee"]),
                        Space = CommonMethod.GetFomartString(row["Space"]),
                        TickNum = CommonMethod.GetFomartString(row["TickNum"]),
                        DiscountRate = CommonMethod.GetFomartString(row["DiscountRate"]),
                        XSFee = CommonMethod.GetFomartString(row["XSFee"]),
                        PMFee = CommonMethod.GetFomartString(row["PMFee"]),
                        DishonoredBillPrescript = CommonMethod.GetFomartString(row["DishonoredBillPrescript"]),
                        LogChangePrescript = CommonMethod.GetFomartString(row["LogChangePrescript"]),
                        UpCabinPrescript = CommonMethod.GetFomartString(row["UpCabinPrescript"]),
                        SpecialType = CommonMethod.GetFomartString(row["SpecialType"]),
                        aterm = CommonMethod.GetFomartString(row["aterm"]),
                        GUID = CommonMethod.GetFomartString(row["GUID"]),
                    };
                    if (att == AirTravelType.OneWay)
                    {
                        airInfo.PolicyGUID = CommonMethod.GetFomartString(row["PolicyGUID"]);
                        airInfo.HighPolicyFlag = CommonMethod.GetFomartString(row["HighPolicyFlag"]);
                        airInfo.LaterPoint = CommonMethod.GetFomartString(row["LaterPoint"]);
                        airInfo.Policy = CommonMethod.GetFomartString(row["Policy"]);
                        airInfo.ReturnMoney = CommonMethod.GetFomartString(row["ReturnMoney"]);
                        airInfo.Commission = CommonMethod.GetFomartString(row["Commission"]);
                        airInfo.SJFee = CommonMethod.GetFomartString(row["SJFee"]);
                        airInfo.GenerationType = CommonMethod.GetFomartString(row["GenerationType"]);
                        airInfo.SeatPrice = CommonMethod.GetFomartString(row["SeatPrice"]);
                    }
                    else
                    {
                        airInfo.PolicyGUID = string.Empty;
                        airInfo.HighPolicyFlag = string.Empty;
                        airInfo.LaterPoint = string.Empty;
                        airInfo.Policy = string.Empty;
                        airInfo.ReturnMoney = string.Empty;
                        airInfo.Commission = string.Empty;
                        airInfo.SJFee = string.Empty;
                        airInfo.GenerationType = string.Empty;
                        airInfo.SeatPrice = string.Empty;
                    }
                    airInfoCollection.AirInfoList.Add(airInfo);
                }
                if (airInfoCollection.AirInfoList.Count > 0)
                {
                    list.Add(airInfoCollection);
                }
            }
            return list;
        }

        /// <summary>
        /// DataSet转JSon  格式→｛表名:[{字段:值,字段:值,…}],表名:[{字段:值,字段:值,…},…],…}
        /// </summary>
        /// <param name="ds">要转换的DataSet</param>
        /// <returns>返回JSon格式String</returns>
        private string DataSetToJSon(DataSet ds)
        {
            StringBuilder sb_Json = new StringBuilder("{");
            foreach (DataTable dt in ds.Tables)
            {
                sb_Json.Append("\"");
                sb_Json.Append(dt.TableName.Trim());
                sb_Json.Append("\"");
                sb_Json.Append(":[");
                foreach (DataRow dr in dt.Rows)
                {
                    sb_Json.Append("{");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sb_Json.Append("\"");
                        sb_Json.Append(dt.Columns[i].ColumnName);
                        sb_Json.Append("\"");
                        sb_Json.Append(":");
                        sb_Json.Append("\"");
                        sb_Json.Append(ReplaceSign(dr[i].ToString().Trim()));
                        sb_Json.Append("\"");
                        sb_Json.Append(",");
                    }
                    sb_Json.Remove(sb_Json.Length - 1, 1);
                    sb_Json.Append("},");
                }
                sb_Json.Remove(sb_Json.Length - 1, 1);
                sb_Json.Append("],");
            }
            sb_Json.Remove(sb_Json.Length - 1, 1);
            sb_Json.Append("}");
            return sb_Json.ToString();
        }

        /// <summary>
        /// 去除特殊符号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ReplaceSign(string str)
        {
            str = str.Replace("\"", string.Empty);
            str = str.Replace("'", string.Empty);
            str = str.Replace("\\", string.Empty);
            str = str.Replace("/", string.Empty);
            str = str.Replace(",", string.Empty);
            str = str.Replace(":", string.Empty);
            str = str.Replace("{", string.Empty);
            str = str.Replace("}", string.Empty);
            str = str.Replace("[", string.Empty);
            str = str.Replace("]", string.Empty);
            return str;
        }

        /// <summary>
        /// 检查必要参数安全性
        /// </summary>
        /// <param name="startCity">出发城市三字码</param>
        /// <param name="midCity">中转城市三字码</param>
        /// <param name="endCity">到达城市三字码</param>
        /// <param name="startDate">出发时间</param>
        /// <param name="backDate">返回时间</param>
        /// <param name="airTravelType">行程类型</param>
        /// <returns></returns>
        private bool CheckParams(string startCity, string midCity, string endCity, string startDate,
            string backDate, AirTravelType airTravelType)
        {
            if (CityIsEqual(startCity, midCity) || CityIsEqual(startCity, endCity) || CityIsEqual(endCity, midCity))
            {
                throw new Exception("出发城市、中转城市、到达城市不能相同");
            }
            if (CheckDate(startDate, backDate, airTravelType))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查日期类型
        /// </summary>
        /// <param name="startDate">出发时间</param>
        /// <param name="backDate">返回时间</param>
        /// <param name="airTravelType">行程类型</param>
        /// <returns></returns>
        private bool CheckDate(string startDate, string backDate, AirTravelType airTravelType)
        {
            DateTime date_Start = DateTime.Parse(startDate + " 23:59:59");
            if (date_Start < DateTime.Now)
            {
                throw new Exception("出发日期必须大于等于当前日期");
            }
            if (airTravelType == AirTravelType.RoundWay)//往返时，检查开始日期必须小于返回日期
            {
                DateTime date_Back = DateTime.Parse(backDate + " 23:59:59");
                if (date_Start > date_Back)
                {
                    throw new Exception("出发日期必须小于等于返回日期");
                }
            }
            return true;
        }

        /// <summary>
        /// 检查2个城市三字码是否相同
        /// </summary>
        /// <param name="cityA"></param>
        /// <param name="cityB"></param>
        /// <returns></returns>
        private bool CityIsEqual(string cityA, string cityB)
        {
            return cityA.Trim().Equals(cityB.Trim());

        }


        #endregion

    }

    public class PolicyParamForAPI : AjAxPolicyParam
    {
        public string Guid { get; set; }
    }
}