using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 返回参数
    /// </summary>
    /// 
    [Serializable]
    public class RePnrObj
    {
        public RePnrObj()
        {
            PatList[0] = "";
            PatList[1] = "";
            PatList[2] = "";

            PatModelList[0] = null;
            PatModelList[1] = null;
            PatModelList[2] = null;
            PnrList[0] = null;
            PnrList[1] = null;
        }
        /// <summary>
        /// 成人编码
        /// </summary>
        public string AdultPnr = string.Empty;
        /// <summary>
        /// 成人编码对应的大编码
        /// </summary>
        public string AdultPnrToBigPNR = string.Empty;
        /// <summary>
        /// 儿童编码对应的大编码
        /// </summary>
        public string childPnrToBigPNR = string.Empty;
        /// <summary>
        /// 儿童编码
        /// </summary>
        public string childPnr = string.Empty;
        /// <summary>
        /// 成人rt编码内容
        /// </summary>
        public string AdultPnrRTContent = string.Empty;
        /// <summary>
        /// 儿童rt编码内容
        /// </summary>
        public string childPnrRTContent = string.Empty;
        /// <summary>
        /// PNR内容导入处理后的RT内容
        /// </summary>
        public string HandleRTContent = string.Empty;
        /// <summary>
        /// 预订使用Office号
        /// </summary>
        public string Office = string.Empty;
        /// <summary>
        /// 航空公司出票Office
        /// </summary>
        public string PrintOffice = string.Empty;

        /// <summary>
        ///pat内容  1成人Pat 2儿童Pat 3婴儿Pat
        /// </summary>
        public string[] PatList = new string[3];

        /// <summary>
        /// Pat内容解析价格数据列表 1成人PatModel 2儿童PatModel 3婴儿PatModel
        /// </summary>
        public PatModel[] PatModelList = new PatModel[3];
        /// <summary>
        /// Pnr内容解析出来实体信息 1成人 2儿童
        /// </summary>
        public PnrModel[] PnrList = new PnrModel[2];

        /// <summary>
        /// 儿童出成人票 儿童PAT内容
        /// </summary>
        public string CHDToAdultPatCon = string.Empty;

        /// <summary>
        /// 儿童出成人票 儿童PAT价格实体
        /// </summary>
        public PatModel CHDToAdultPat = null;

        /// <summary>
        /// 指令信息
        /// </summary>
        public SortedList<string, string> InsList = new SortedList<string, string>();

        /// <summary>
        /// 成人预订信息
        /// </summary>
        public SortedList<string, string> AdultYudingList = new SortedList<string, string>();
        /// <summary>
        /// 儿童预订信息
        /// </summary>
        public SortedList<string, string> ChildYudingList = new SortedList<string, string>();
        /// <summary>
        /// 编码类型 1为成人编码 2为儿童编码
        /// </summary>
        public string PnrType = "1";
        /// <summary>
        /// 字符串分隔符
        /// </summary>
        public string SplitChar = FormatPNR.GetSplitChar();
        /// <summary>
        /// IP
        /// </summary>
        public string ServerIP = string.Empty;
        /// <summary>
        /// Port
        /// </summary>
        public string ServerPort = string.Empty;


        public override string ToString()
        {
            StringBuilder sbLog = new StringBuilder();
            if (!string.IsNullOrEmpty(AdultPnr))
            {
                sbLog.AppendFormat("成人编码:{0}\r\n", AdultPnr);
                sbLog.AppendFormat("成人大编码:{0}\r\n", AdultPnrToBigPNR);
                sbLog.AppendFormat("成人RT数据:{0}\r\n", AdultPnrRTContent);
                sbLog.AppendFormat("成人PAT数据:{0}\r\n", PatList[0]);
                sbLog.AppendFormat("婴儿PAT数据:{0}\r\n", PatList[2]);
            }
            if (!string.IsNullOrEmpty(childPnr))
            {
                sbLog.AppendFormat("儿童编码:{0}\r\n", childPnr);
                sbLog.AppendFormat("儿童大编码:{0}\r\n", childPnrToBigPNR);
                sbLog.AppendFormat("儿童RT数据:{0}\r\n", childPnrRTContent);
                sbLog.AppendFormat("儿童PAT数据:{0}\r\n", PatList[1]);
            }
            return sbLog.ToString();
        }
    }
    /// <summary>
    /// 传入参数
    /// </summary>
    /// 
    [Serializable]
    public class PnrParamObj
    {
        /// <summary>
        /// 使用PID通道 0.使用EC 1.使用凯迅PID 2使用新版PID 3.其他PID
        /// </summary>
        public int UsePIDChannel = 0;

        /// <summary>
        /// 是否是获取特价1是 0否
        /// </summary>
        public int IsGetSpecialPrice = 0;


        /// <summary>
        /// 登录平台或者使用接口的帐号名 可选项
        /// </summary>
        public string UserName = string.Empty;

        /// <summary>
        /// PID 可选项
        /// </summary>
        public string PID = string.Empty;
        /// <summary>
        /// KeyNo 可选项
        /// </summary>
        public string KeyNo = string.Empty;

        /// <summary>
        /// 服务器IP 必填
        /// </summary>
        public string ServerIP = string.Empty;
        /// <summary>
        /// 端口 必填
        /// </summary>
        public int ServerPort = 350;

        /// <summary>
        /// 使用对用的office 如CTU303 必填
        /// </summary>
        public string Office = string.Empty;

        /// <summary>
        /// 距离飞机起飞前多少小时预订 默认1小时 可选
        /// </summary>
        public int FlyAdvanceTime = 1;


        /// <summary>
        /// 电话号码 02855885555  可选项
        /// </summary>
        public string CTTel = string.Empty;

        /// <summary>
        /// 手机号码 15825666666 可选项
        /// </summary>
        public string CTCTPhone = string.Empty;
        /// <summary>
        /// 落地运营商手机号码
        /// </summary>
        public string LuoDiCTCTPhone = string.Empty;
        /// <summary>
        /// 落地运营商电话号码
        /// </summary>
        public string LuoDiCTTel = string.Empty;

        /// <summary>
        /// 航空公司二字码 如：CA  必填
        /// </summary>
        public string CarryCode = string.Empty;

        /// <summary>
        /// 是否儿童出成人票 1是 0否 可选项默认0
        /// </summary>
        public string ChildIsAdultEtdz = "0";

        /// <summary>
        /// 单独生成儿童Pnr对应备注的成人编码 默认为空 可选
        /// </summary>
        public string AdultPnr = string.Empty;

        /// <summary>
        /// 乘机人实体  必填
        /// </summary>
        public List<IPassenger> PasList = new List<IPassenger>();

        /// <summary>
        /// 航段实体   必填
        /// </summary>
        public List<ISkyLeg> SkyList = new List<ISkyLeg>();

        /// <summary>
        /// 是否为接口有编码时使用 1是 0否(生成编码)
        /// </summary>
        public int IsInterface = 0;
        /// <summary>
        /// 接口传过来的编码
        /// </summary>
        public string IPnr = string.Empty;

    }
    /// <summary>
    /// 接口 乘机人实体
    /// </summary>    
    [Serializable]
    public class IPassenger
    {
        /// <summary>
        /// 乘客类型 1,成人 2，儿童 3，婴儿
        /// </summary>
        public int PassengerType = 1;
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public System.String PassengerName = string.Empty;

        /// <summary>
        /// 乘机人证件号 注明: 婴儿和CZ儿童证件号为出生日月年日期型  否则定不起儿童编码
        /// </summary>
        public string PasSsrCardID = string.Empty;

        /// <summary>
        ///婴儿出生日期如：2012-08-20  婴儿必须
        /// </summary>
        //public string YinerBirthday = string.Empty;

        /// <summary>
        /// 儿童出生日期 用于儿童编码 Chld标识
        /// </summary>
        public string ChdBirthday = string.Empty;

        /// <summary>
        /// 航空公司卡号 11111
        /// </summary>
        public string CpyandNo = string.Empty;
    }
    /// <summary>
    /// 接口 航段实体
    /// </summary>
    [Serializable]
    public class ISkyLeg
    {
        /// <summary>
        /// 出发城市二字码 如:CTU
        /// </summary>
        public string fromCode = string.Empty;
        /// <summary>
        /// 到达城市二字码 如:PEK
        /// </summary>
        public string toCode = string.Empty;
        /// <summary>
        /// 航空公司二字码 如CZ
        /// </summary>
        public string CarryCode = string.Empty;
        /// <summary>
        /// 航班号 如:8514
        /// </summary>
        public string FlightCode = string.Empty;
        /// <summary>
        /// 舱位 如：F
        /// </summary>
        public string Space = string.Empty;
        /// <summary>
        /// 起飞日期 如:2012-09-20
        /// </summary>
        public string FlyStartDate = string.Empty;
        /// <summary>
        /// 起飞时间 如: 0830
        /// </summary>
        public string FlyStartTime = string.Empty;
        /// <summary>
        /// 到达时间 如: 0930
        /// </summary>
        public string FlyEndTime = string.Empty;
        /// <summary>
        /// 折扣
        /// </summary>
        public string Discount = "0";

    }
}
