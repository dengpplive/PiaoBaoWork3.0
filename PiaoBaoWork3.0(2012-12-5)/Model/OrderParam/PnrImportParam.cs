using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model
{
    /// <summary>
    /// Pnr导入参数
    /// </summary>   
    [Serializable]
    public class PnrImportParam
    {
        public string Pnr = string.Empty;
        public string BigPnr = string.Empty;
        /// <summary>
        ///当前登录用户角色 1=平台，2=落地运营商，3=供应商，4=分销商，5=采购商
        /// </summary>
        public string RoleType = "";
        /// <summary>
        /// 编码通道来源 默认0正常Pnr导入 1团队PNR，2儿童PNR，3.大编码 4.PNR内容导入 5.升舱换开通道
        /// </summary>
        public int ImportTongDao = 0;
        /// <summary>
        /// 是否PNR入库记账1是0否
        /// </summary>
        public int IsImportJZ = 0;
        /// <summary>
        /// 是否继续导入默认0不继续 
        /// </summary>
        public int IsNextOK = 0;
        /// <summary>
        /// Pnr内容导入是否合并 默认1合并 0不合并
        /// </summary>
        public int IsMerge = 1;
        /// <summary>
        /// 是否第一次请求 默认0是 否则不是
        /// </summary>
        public int IsSecond = 0;
        /// <summary>
        /// 是否允许换编码出票 默认1允许 0不允许 
        /// </summary>
        public int AllowChangePNR = 1;
        /// <summary>
        /// 新订单号 升舱换开
        /// </summary>
        public string OrderId = string.Empty;
        //rt和pat数据
        public string RTAndPatData = string.Empty;
        //RT数据
        public string RTData = string.Empty;
        //PAT数据
        public string PATData = string.Empty;
        /// <summary>
        /// 发指令使用的Office
        /// </summary>
        public string Office = string.Empty;
        /// <summary>
        /// 需要授权的Office列表
        /// </summary>
        public List<string> authOfficeList = new List<string>();
        /// <summary>
        /// 提示消息
        /// </summary>
        public string TipMsg = string.Empty;
        /// <summary>
        /// 第二次使用参数
        /// </summary>
        public ImportTipParam SecondPM = new ImportTipParam();

        /// <summary>
        /// 城市基础数据
        /// </summary>
        public List<Bd_Air_AirPort> CityList = new List<Bd_Air_AirPort>();
        /// <summary>
        /// 航空公司基础数据
        /// </summary>
        public List<Bd_Air_Carrier> AirList = new List<Bd_Air_Carrier>();

        /// <summary>
        /// Y舱位价格
        /// </summary>
        public List<Bd_Air_Fares> FareList = new List<Bd_Air_Fares>();

        /// <summary>
        /// 供应商设置的航空公司信息 含有出票Office
        /// </summary>
        public List<Tb_Ticket_PrintOffice> GYAirTicketlist = new List<Tb_Ticket_PrintOffice>();
        /// <summary>
        /// 乘机人与证件号 乘机人 证件号
        /// </summary>
        public SortedList<string, string> sortList = new SortedList<string, string>();

        public OrderInputParam OrderParam = new OrderInputParam();

        //---------------------------------------------------------
        /// <summary>
        /// 是否生成订单默认true 
        /// </summary>
        public bool IsCreateOrder = true;
        /// <summary>
        /// 后台1 前台0
        /// </summary>
        public int Source = 0;
        /// <summary>
        /// 选择的客户信息
        /// </summary>
        public PbProject.Model.User_Employees m_UserInfo = null;
        /// <summary>
        /// 选择的客户公司信息
        /// </summary>
        public PbProject.Model.User_Company m_CurCompany;
        /// <summary>
        /// 选择的供应公司信息
        /// </summary>
        public PbProject.Model.User_Company m_SupCompany;
        //-----------------------------------------------------------
        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        public PbProject.Model.User_Employees m_LoginUser = null;
        /// <summary>
        /// 当前登录用户公司信息
        /// </summary>
        public PbProject.Model.User_Company m_LoginCompany;
    }

    [Serializable]
    public class ImportTipParam
    {
        private string _DataFlag = "0";
        /// <summary>
        /// 显示操作数据标识 默认0消息提示 1乘客证件号补全数据 2成功跳转 3.PNR输入儿童关联成人订单号
        /// </summary>
        public string DataFlag
        {
            get
            {
                return _DataFlag;
            }
            set
            {
                _DataFlag = value;
            }
        }
        private string _OpType = "0";
        /// <summary>
        /// 0提示 1操作
        /// </summary>
        public string OpType
        {
            get
            {
                return _OpType;
            }
            set
            {
                _OpType = value;
            }
        }

        private string _ErrCode = "0";
        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrCode
        {
            get
            {
                return _ErrCode;
            }
            set
            {
                _ErrCode = value;
            }
        }
        private string _Msg = string.Empty;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg
        {
            get
            {
                return _Msg;
            }
            set
            {
                _Msg = value;
            }
        }
        //-----以下是操作数据部分---------------------------------------------------
        private string _PnrContent = string.Empty;
        /// <summary>
        /// PNR内容
        /// </summary>
        public string PnrContent
        {
            get
            {
                return _PnrContent;
            }
            set
            {
                _PnrContent = value;
            }
        }

        private string _PATContent = string.Empty;
        /// <summary>
        /// PAT内容
        /// </summary>
        public string PATContent
        {
            get
            {
                return _PATContent;
            }
            set
            {
                _PATContent = value;
            }
        }

        private string _Office = string.Empty;
        /// <summary>
        /// 导编码使用的Offcie
        /// </summary>
        public string Office
        {
            get
            {
                return _Office;
            }
            set
            {
                _Office = value;
            }
        }

        private string _GoUrl = string.Empty;
        /// <summary>
        /// 成功后跳转到指定Url
        /// </summary>
        public string GoUrl
        {
            get
            {
                return _GoUrl;
            }
            set
            {
                _GoUrl = value;
            }
        }
    }
}
