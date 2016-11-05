using System;
namespace PbProject.Model.definitionParam
{
    [Serializable]
    public partial class BaseSwitch
    {
        private string _base_Oil;
        private string _cssURL;
        private string _daPeiZhiCanShu;
        private string _gongYingKongZhiFenXiao;
        private string _heiPingCanShu;
        private string _jieKouZhangHao;
        private string _kongZhiXiTong;
        private string _order_Index;
        private string _policy_Order;
        private string _wangYinLeiXing;
        private string _wangYinZhangHao;
        private string _suoShuYeWuYuan;
        private bool _ChildNotTicketIsShowRemark = true;
        private string _autoAccount;
        private string _autoPayAccount;
        private string _setCommission;
        private string _cssQQ;
        private string _isDuLiFenXiao;
        private string _isShowDuLiInfo;
        private string _yunYingQuanXian;
        private string _chuPiaoShiJian;
        /// <summary>
        /// 燃油费基数
        /// 格式如下,130^|70^^|60^^|30^^ 数字不足4位用^补齐
        /// 0(成人800公里以上),1(成人800以下),2(儿童800公里以上),3(儿童800公里以下)
        /// </summary>
        public string Base_Oil
        {
            get { return _base_Oil; }
            set { _base_Oil = value; }
        }
        /// <summary>
        /// 机票样式路径
        /// </summary>
        public string CssURL
        {
            get { return _cssURL; }
            set { _cssURL = value; }
        }

        /// <summary>
        /// 接口不出儿童票是否显示 true 默认显示 false不显示
        /// </summary>
        public bool ChildNotTicketIsShowRemark
        {
            get { return _ChildNotTicketIsShowRemark; }
            set { _ChildNotTicketIsShowRemark = value; }
        }
        /// <summary>
        /// 大配置参数
        /// 大配置参数(0大配置IP|1大配置端口|2大配置Office|3大配置名称与密码)
        /// </summary>
        public string DaPeiZhiCanShu
        {
            get { return _daPeiZhiCanShu; }
            set { _daPeiZhiCanShu = value; }
        }
        /// <summary>
        /// 供应控制分销开关
        /// </summary>
        public string GongYingKongZhiFenXiao
        {
            get { return _gongYingKongZhiFenXiao; }
            set { _gongYingKongZhiFenXiao = value; }
        }
        /// <summary>
        /// 黑屏参数
        /// 黑屏参数(0网页黑屏IP|1网页黑屏端口|2白屏IP|3白屏交互端口|4Office号|5网页黑屏帐号|6网页黑屏密码|7EC网页黑屏监听端口|8开票单位名称|9航协号)
        /// </summary>
        public string HeiPingCanShu
        {
            get { return _heiPingCanShu; }
            set { _heiPingCanShu = value; }
        }
        /// <summary>
        /// 接口账号(0 517账号^517密码^517密钥^517支付帐号^517支付密码|
        /// 1 51book账号^51book密码^51book密钥^51book通知URL地址|
        /// 2 百拓账号^百拓密码^百拓密钥|
        /// 3 票盟账号^票盟密码^票盟密钥|
        /// 4 今日账号^今日密码|
        /// 5 8000y账号^8000y密码^8000y代扣支付宝|
        /// 6 易行总帐号^易行供应帐号)
        /// </summary>
        public string JieKouZhangHao
        {
            get { return _jieKouZhangHao; }
            set { _jieKouZhangHao = value; }
        }
        /// <summary>
        /// 控制系统权限
        /// </summary>
        public string KongZhiXiTong
        {
            get { return _kongZhiXiTong; }
            set { _kongZhiXiTong = value; }
        }
        /// <summary>
        /// 生成订单编号使用自增值，防止编号重复
        /// </summary>
        public string Order_Index
        {
            get { return _order_Index; }
            set { _order_Index = value; }
        }
        /// <summary>
        /// 竞价平台政策排序规则
        /// </summary>
        public string Policy_Order
        {
            get { return _policy_Order; }
            set { _policy_Order = value; }
        }
        /// <summary>
        /// 网银类型 0不使用网银,5支付宝网银,6快钱网银,7汇付网银,8财付通网银
        /// </summary>
        public string WangYinLeiXing
        {
            get { return _wangYinLeiXing; }
            set { _wangYinLeiXing = value; }
        }

        /// <summary>
        /// 网银账号集合
        /// 网银账号集合(0支付宝收款^支付宝充值收款^本地费率^共享费率|1快钱收款^快钱充值收款^本地费率^共享费率|2汇付收款^汇付充值收款^本地费率^共享费率|3财付通收款^财付通充值收款^本地费率^共享费率)
        /// </summary>
        public string WangYinZhangHao
        {
            get { return _wangYinZhangHao; }
            set { _wangYinZhangHao = value; }
        }
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string SuoShuYeWuYuan
        {
            get { return _suoShuYeWuYuan; }
            set { _suoShuYeWuYuan = value; }
        }

        /// <summary>
        /// 自动出票航空公司帐号密码(总长度只有500，现在暂时够用)
        /// 格式：CA:xxx//xxx^^^CZ:xxx//xxx^^^MU:xxx//xxx
        /// </summary>
        public string AutoAccount
        {
            get { return _autoAccount; }
            set { _autoAccount = value; }
        }

        /// <summary>
        /// 自动出票支付帐号参数
        /// 格式：自动出票方式(1，支付宝本票通；2，汇付天下出票窗)^自动出票重调次数^帐号|是否签约(1，已签约；2，未签)^帐号|密码|支付方式(1，信用账户；2，付款账户)
        /// </summary>
        public string AutoPayAccount
        {
            get { return _autoPayAccount; }
            set { _autoPayAccount = value; }
        }
        /// <summary>
        /// 采购佣金取舍
        /// </summary>
        public string setCommission
        {
            get { return _setCommission; }
            set { _setCommission = value; }
        }
        /// <summary>
        /// qq
        /// </summary>
        public string cssQQ
        {
            get { return _cssQQ; }
            set { _cssQQ = value; }
        }
        /// <summary>
        /// 独立分销设置(是否是独立分销(0否1是))
        /// </summary>
        public string IsDuLiFenXiao
        {
            get { return _isDuLiFenXiao; }
            set { _isDuLiFenXiao = value; }
        }
        /// <summary>
        /// 是否显示自己信息(0否1是)
        /// </summary>
        public string IsShowDuLiInfo
        {
            get { return _isShowDuLiInfo; }
            set { _isShowDuLiInfo = value; }
        }
        /// <summary>
        /// 运营权限
        /// </summary>
        public string yunYingQuanXian
        {
            get { return _yunYingQuanXian; }
            set { _yunYingQuanXian = value; }
        }
        /// <summary>
        /// 出票时间
        /// 0-B2B|1-BSP|2-517|3-百拓|4-8000|5-今日|6-票盟|7-51book|8-共享|9-易行
        /// </summary>
        public string chuPiaoShiJian
        {
            get { return _chuPiaoShiJian; }
            set { _chuPiaoShiJian = value; }
        }

    }
}
