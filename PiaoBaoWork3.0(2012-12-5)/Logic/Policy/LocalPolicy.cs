using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using DataBase.Data;
using System.Data;
namespace PbProject.Logic.Policy
{
    public class LocalPolicy
    {
        BaseDataManage Manage = new BaseDataManage();
        public LocalPolicy()
        { 
        
        }
        /// <summary>
        /// 创建新的DataSet集合对象
        /// </summary>
        /// <returns>返回创建好的空DataSet集合</returns>
        public DataSet ReturnNewDataSet()
        {
            DataSet dsNew = new DataSet();
            DataTable dtNew = new DataTable();
            //新添加列名
            DataColumn dc_CarryCode = new DataColumn("CarryCode");//0.承运人二字码
            DataColumn dc_ApplianceFlight = new DataColumn("ApplianceFlight");//1.适用航班号
            DataColumn dc_UnApplianceFlight = new DataColumn("UnApplianceFlight");//2.不适用航班号
            DataColumn dc_ScheduleConstraints = new DataColumn("ScheduleConstraints");//3.班期限制
            DataColumn dc_Space = new DataColumn("Space");//4.舱位
            DataColumn dc_OldPolicy = new DataColumn("OldPolicy");//5.原始政策
            DataColumn dc_GYPolicy = new DataColumn("GYPolicy");//6.供应的政策
            DataColumn dc_FXPolicy = new DataColumn("FXPolicy");//7.分销的政策
            DataColumn dc_PolicySource = new DataColumn("PolicySource");//8.政策来源
            DataColumn dc_PolicyId = new DataColumn("PolicyId");//9.政策编号
            DataColumn dc_PolicyType = new DataColumn("PolicyType");//10.政策类型
            DataColumn dc_Remark = new DataColumn("Remark");//11.政策备注
            DataColumn dc_WorkTime = new DataColumn("WorkTime");//12.上班时间
            DataColumn dc_BusinessTime = new DataColumn("BusinessTime");//13.业务时间
            DataColumn dc_AddTime = new DataColumn("AddTime");//14.添加时间
            DataColumn dc_SpecialType = new DataColumn("SpecialType");//15.特价类型
            DataColumn dc_ShippingPrice = new DataColumn("ShippingPrice");//16.特价价格
            DataColumn dc_Discount = new DataColumn("Discount");//17.特价折扣
            DataColumn dc_ReturnMoney = new DataColumn("ReturnMoney");//18.现返
            DataColumn dc_IsApplyToShareFlight = new DataColumn("IsApplyToShareFlight");//19.是否共享
            DataColumn dc_IsPause = new DataColumn("IsPause");//20.是否挂起
            DataColumn dc_FCity = new DataColumn("FCity");//21.出发城市
            DataColumn dc_TCity = new DataColumn("TCity");//22.到达城市
            DataColumn dc_IsLowerOpen = new DataColumn("IsLowerOpen");//23.是否低开 1低开 0不低开（特价，接口默认不低开，本地普通默认低开）
            DataColumn dc_DiscountRate = new DataColumn("DiscountRate");//24.折扣
            DataColumn dc_Id = new DataColumn("Id");//25.ID
            DataColumn dc_A14 = new DataColumn("A14");//26.共享航班适用承运人
            DataColumn dc_PolOffice = new DataColumn("PolOffice");//27.政策对应office号
            DataColumn dc_PID = new DataColumn("PID");//28.政策对应PID(共享专用)
            DataColumn dc_KeyNo = new DataColumn("KeyNo");//29.政策对应KeyNo(共享专用)
            DataColumn dc_A21 = new DataColumn("A21");//30.高返政策
            DataColumn dc_A22 = new DataColumn("A22");//31.自动出票方式 手动（空 null 0） 半自动（1） 全自动（2）
            DataColumn dc_A24 = new DataColumn("A24");//32.后返点数   点数
            DataColumn dc_A25 = new DataColumn("A25");//33.后返开关 1开
            //添加到DataTable集合中
            dtNew.Columns.Add(dc_CarryCode);
            dtNew.Columns.Add(dc_ApplianceFlight);
            dtNew.Columns.Add(dc_UnApplianceFlight);
            dtNew.Columns.Add(dc_ScheduleConstraints);
            dtNew.Columns.Add(dc_Space);
            dtNew.Columns.Add(dc_OldPolicy);
            dtNew.Columns.Add(dc_GYPolicy);
            dtNew.Columns.Add(dc_FXPolicy);
            dtNew.Columns.Add(dc_PolicySource);
            dtNew.Columns.Add(dc_PolicyId);
            dtNew.Columns.Add(dc_PolicyType);
            dtNew.Columns.Add(dc_Remark);
            dtNew.Columns.Add(dc_WorkTime);
            dtNew.Columns.Add(dc_BusinessTime);
            dtNew.Columns.Add(dc_AddTime);
            dtNew.Columns.Add(dc_SpecialType);
            dtNew.Columns.Add(dc_ShippingPrice);
            dtNew.Columns.Add(dc_Discount);
            dtNew.Columns.Add(dc_ReturnMoney);
            dtNew.Columns.Add(dc_IsApplyToShareFlight);
            dtNew.Columns.Add(dc_IsPause);
            dtNew.Columns.Add(dc_FCity);
            dtNew.Columns.Add(dc_TCity);
            dtNew.Columns.Add(dc_IsLowerOpen);
            dtNew.Columns.Add(dc_DiscountRate);
            dtNew.Columns.Add(dc_Id);
            dtNew.Columns.Add(dc_A14);          
            dtNew.Columns.Add(dc_PolOffice);
            dtNew.Columns.Add(dc_PID);
            dtNew.Columns.Add(dc_KeyNo);
            dtNew.Columns.Add(dc_A21);
            dtNew.Columns.Add(dc_A22);
            dtNew.Columns.Add(dc_A24);
            dtNew.Columns.Add(dc_A25);
            //添加到DataSet集合中
            dsNew.Tables.Add(dtNew);

            //返回创建好的DataSet集合
            return dsNew;
        }
        /// <summary>
        /// 获取本地政策普通
        /// </summary>
        /// <param name="CpyNo"></param>
        /// <param name="StartCityNameCode"></param>
        /// <param name="TargetCityNameCode"></param>
        /// <param name="TravelType"></param>
        /// <returns></returns>
        public List<Tb_Ticket_Policy> getLocalPolicy(string CpyNo, string StartCityNameCode, string MiddleCityNameCode,
            string TargetCityNameCode, string FromDate, string ReturnDate, string TravelType)
        {
            bool isBackOrUnite = false;//是否是往返或联成
            if (TravelType == "2" || TravelType == "3")
            {
                isBackOrUnite = true;
            }
            TravelType = changeTravelType(TravelType);
            string GCpyNo = CpyNo.Substring(0, 12);//获取上级供应商或落地运营商的ID
            string midsql = " ";
            string BackOrUnitesqlTime = " ";
            if (MiddleCityNameCode != "")
            {
                midsql = " and (MiddleCityNameCode like '%" + MiddleCityNameCode + "%' or MiddleCityNameCode like '%ALL%' )";
            }
            if (isBackOrUnite)
            {
                BackOrUnitesqlTime = " and   ( '" + ReturnDate + "' >=FlightStartDate and '" + ReturnDate + "' <=FlightEndDate )"
                 + " and   ( '" + ReturnDate + "' >=PrintStartDate and '" + ReturnDate + "' <=PrintEndDate )";
            }
            string sqlwhere = " 1=1 "
                + " and CpyNo='" + GCpyNo + "'"
                + " and (StartCityNameCode like '%" + StartCityNameCode + "%' or StartCityNameCode like '%ALL%' )"
                + midsql
                + " and (TargetCityNameCode like '%" + TargetCityNameCode + "%' or TargetCityNameCode like '%ALL%') "
                + " and   ( '" + FromDate + "' >=FlightStartDate and '" + FromDate + "' <=FlightEndDate )"
                + " and   ( '" + FromDate + "' >=PrintStartDate and '" + FromDate + "' <=PrintEndDate )"
                + BackOrUnitesqlTime
                + " and TravelType in  (" + TravelType + ")"
                + " and A1=0 "
                + " and AuditType=1 " 
                + " and IsPause=0 ";
            List<Tb_Ticket_Policy> objList = Manage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_Policy>;
            return objList;
        }
        /// <summary>
        /// 获取本地政策成人默认
        /// </summary>
        /// <param name="CpyNo"></param>
        /// <param name="StartCityNameCode"></param>
        /// <param name="TargetCityNameCode"></param>
        /// <param name="TravelType"></param>
        /// <returns></returns>
        public List<Tb_Ticket_Policy> getLocalPolicyDefault(string CpyNo)
        {
            string GCpyNo = CpyNo.Substring(0, 12);//获取上级供应商或落地运营商的ID
            string sqlwhere = " 1=1 "
                            + "and CpyNo='" + GCpyNo + "'"
                            + "and A1=1"
                            +" and AuditType=1";
            List<Tb_Ticket_Policy> objList = Manage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_Policy>;
            return objList;
        }
        /// <summary>
        /// 获取本地政策儿童默认
        /// </summary>
        /// <param name="CpyNo"></param>
        /// <param name="StartCityNameCode"></param>
        /// <param name="TargetCityNameCode"></param>
        /// <param name="TravelType"></param>
        /// <returns></returns>
        public List<Tb_Ticket_Policy> getLocalPolicyDefaultChild(string CpyNo)
        {
            string GCpyNo = CpyNo.Substring(0, 12);//获取上级供应商或落地运营商的ID
            string sqlwhere = " 1=1 "
                            + " and CpyNo='" + GCpyNo + "'"
                            + " and A1=2"
                            + " and AuditType=1";
            List<Tb_Ticket_Policy> objList = Manage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_Policy>;
            return objList;
        }
        /// <summary>
        /// 获取共享政策
        /// </summary>
        /// <param name="CpyNo"></param>
        /// <param name="StartCityNameCode"></param>
        /// <param name="TargetCityNameCode"></param>
        /// <param name="TravelType"></param>
        /// <returns></returns>
        public List<Tb_Ticket_Policy> getSharePolicy(string CpyNo, string StartCityNameCode, string MiddleCityNameCode,
            string TargetCityNameCode, string FromDate, string ReturnDate, string TravelType, bool IsINF)
        {
            List<Tb_Ticket_Policy> objList = new List<Tb_Ticket_Policy>();
            if (!IsINF)//有婴儿不获取共享政策
            {
                bool isBackOrUnite = false;//是否是往返或联成
                if (TravelType == "2" || TravelType == "3")
                {
                    isBackOrUnite = true;
                }
                TravelType = changeTravelType(TravelType);
                string GCpyNo = CpyNo.Substring(0, 12);//获取上级供应商或落地运营商的ID
                string midsql = " ";
                string BackOrUnitesqlTime = " ";
                if (MiddleCityNameCode != "")
                {
                    midsql = " and (MiddleCityNameCode like '%" + MiddleCityNameCode + "%' or MiddleCityNameCode like '%ALL%' )";
                }
                if (isBackOrUnite)
                {
                    BackOrUnitesqlTime = " and   ( '" + ReturnDate + "' >=FlightStartDate and '" + ReturnDate + "' <=FlightEndDate )"
                     + " and   ( '" + ReturnDate + "' >=PrintStartDate and '" + ReturnDate + "' <=PrintEndDate )";
                }
                string sqlwhere = " 1=1 "
                   + " and CpyNo<>'" + GCpyNo + "'"
                   + " and SharePoint>0 "
                   + " and (StartCityNameCode like '%" + StartCityNameCode + "%' or StartCityNameCode like '%ALL%' )"
                   + midsql
                   + " and (TargetCityNameCode like '%" + TargetCityNameCode + "%' or TargetCityNameCode like '%ALL%') "
                   + " and ( '" + FromDate + "' >=FlightStartDate and '" + FromDate + "' <=FlightEndDate )"
                   + " and ( '" + FromDate + "' >=PrintStartDate and '" + FromDate + "' <=PrintEndDate )"
                   + BackOrUnitesqlTime
                   + " and TravelType in  (" + TravelType + ")"
                   + " and A1=0 "
                   + " and AuditType=1 "
                   + " and IsPause=0 "
                   + " and a16<>1 ";//A16临时启用字段(0和空代表要共享,1代表不共享)
                objList = Manage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_Policy>;
            }
            return objList;
        }
        /// <summary>
        /// 转换行程类型和数据库对应
        /// </summary>
        /// <param name="trave"></param>
        /// <returns></returns>
        private string changeTravelType(string trave)
        {
            //行程类型 1.单程，2.往返/单程，3.往返，4.中转联程
            if (trave == "1")
            {
                trave = "1,2";
            }
            if (trave == "2")
            {
                trave = "2,3";
            }
            if (trave == "3")
            {
                trave = "4";
            }
            return trave;
        }

    }
}
