using System;
namespace PbProject.Model.definitionParam
{
    [Serializable]
    public partial class SelectCityParams
    {
        private string _fcity;
        private string _mcity;
        private string _tcity;
        private string _time;
        private string _Totime;
        private string _cairry;
        private int _TravelType;
        private int _num;
        private PbProject.Model.User_Employees _mEmployees;
        private PbProject.Model.User_Company _mCompany;
        private bool _IsShowGX;

        /// <summary>
        /// 出发城市
        /// </summary>
        public string fcity
        {
            get { return _fcity; }
            set { _fcity = value; }
        }
        /// <summary>
        /// 中转城市
        /// </summary>
        public string mcity
        {
            get { return _mcity; }
            set { _mcity = value; }
        }

        /// <summary>
        /// 到达城市
        /// </summary>
        public string tcity
        {
            get { return _tcity; }
            set { _tcity = value; }
        }

        /// <summary>
        /// 出发时间
        /// </summary>
        public string time
        {
            get { return _time; }
            set { _time = value; }
        }

        /// <summary>
        ///往返与联程第二次出发时间
        /// </summary>
        public string Totime
        {
            get { return _Totime; }
            set { _Totime = value; }
        }
        /// <summary>
        /// 承运人
        /// </summary>
        public string cairry
        {
            get { return _cairry; }
            set { _cairry = value; }
        }
        /// <summary>
        /// 行程类型
        /// </summary>
        public int TravelType
        {
            get { return _TravelType; }
            set { _TravelType = value; }
        }


        /// <summary>
        /// 航班数量
        /// </summary>
        public int num
        {
            get { return _num; }
            set { _num = value; }
        }



        /// <summary>
        /// 员工信息
        /// </summary>
        public PbProject.Model.User_Employees mEmployees
        {
            get { return _mEmployees; }
            set { _mEmployees = value; }
        }

        /// <summary>
        /// 公司信息
        /// </summary>
        public PbProject.Model.User_Company mCompany
        {
            get { return _mCompany; }
            set { _mCompany = value; }
        }


        /// <summary>
        /// 是否共享
        /// </summary>
        public bool IsShowGX
        {
            get { return _IsShowGX; }
            set { _IsShowGX = value; }
        }

    }
}
