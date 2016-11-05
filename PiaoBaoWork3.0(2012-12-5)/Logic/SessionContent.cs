using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PbProject.Model;

namespace PbProject.Logic
{
    [Serializable]
    public class SessionContent
    {
        /// <summary>
        /// 当前登录用户用户个人信息 
        /// </summary>
        public PbProject.Model.User_Employees USER { get; set; }
        /// <summary>
        /// 当前登录用户公司信息
        /// </summary>
        public PbProject.Model.User_Company COMPANY { get; set; }

        /// <summary>
        /// 落地运营商和供应商公司信息
        /// </summary>
        public PbProject.Model.User_Company SUPCOMPANY { get; set; }


        /// <summary>
        /// 有关指令配置信息
        /// </summary>
        public PbProject.Model.ConfigParam CONFIGPARAM { get; set; }

        /// <summary>
        /// 当前登录用户公司参数信息
        /// </summary>
        public List<PbProject.Model.Bd_Base_Parameters> BASEPARAMETERS { get; set; }

        /// <summary>
        /// 落地运营商和供应商公司参数信息
        /// </summary>
        public List<PbProject.Model.Bd_Base_Parameters> SupBASEPARAMETERS { get; set; }

        /// <summary>
        /// 当前登录用户权限
        /// </summary>
        public User_Permissions M_USERPERMISSIONS { get; set; }

        /// <summary>
        /// 登录用户
        /// </summary>
        //public string USERLOGIN = "userLoginPB";

        /// <summary>
        /// 验证码
        /// </summary>
        public string CHECKCODE = "checkCodePB";
        /// <summary>
        /// 落地运营商和供应商公司编号
        /// </summary>
        public string parentCpyno { get; set; }
    }
}