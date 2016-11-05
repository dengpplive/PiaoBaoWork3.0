//初始化所有国内机场城市 （最新版本）
var newcCtys = new Array();
newcCtys[0] = new Array('AKU', '阿克苏', 'AKESU', 'AKS');
newcCtys[1] = new Array('NGQ', '阿里', 'ALI', 'ALI');
newcCtys[2] = new Array('AAT', '阿勒泰', 'ALTAY', 'ALT');
newcCtys[3] = new Array('AKA', '安康', 'ANKANG', 'AK');
newcCtys[4] = new Array('AQG', '安庆', 'ANQING', 'AQ');
newcCtys[5] = new Array('AOG', '鞍山', 'ANSHAN', 'AS');
newcCtys[6] = new Array('AVA', '安顺', 'ANSHUN', 'AS');
newcCtys[7] = new Array('AYN', '安阳', 'ANYANG', 'AY');
newcCtys[8] = new Array('AEB', '百色', 'BAISE', 'BS');
newcCtys[9] = new Array('BSD', '保山', 'BAOSHAN', 'BS');
newcCtys[10] = new Array('BAV', '包头', 'BAOTOU', 'BT');
newcCtys[11] = new Array('BHY', '北海', 'BEIHAI', 'BH');
newcCtys[12] = new Array('PEK', '北京', 'BEIJING', 'BJ');
newcCtys[13] = new Array('BFU', '蚌埠', 'BENGBU', 'BB');
newcCtys[14] = new Array('BPL', '博乐', 'BOLE', 'BOL');
newcCtys[15] = new Array('NBS', '长白山', 'CHANGBAISHAN', 'CBS');
newcCtys[16] = new Array('CGQ', '长春', 'CHANGCHUN', 'CC');
newcCtys[17] = new Array('CGD', '常德', 'CHANGDE', 'CD');
newcCtys[18] = new Array('BPX', '昌都', 'CHANGDU', 'CD');
newcCtys[19] = new Array('CNI', '长海', 'CHANGHAI', 'CH');
newcCtys[20] = new Array('CSX', '长沙', 'CHANGSHA', 'CS');
newcCtys[21] = new Array('CIH', '长治', 'CHANGZHI', 'CZ');
newcCtys[22] = new Array('CZX', '常州', 'CHANGZHOU', 'CZ');
newcCtys[23] = new Array('CHG', '朝阳', 'CHAOYANG', 'CY');
newcCtys[24] = new Array('CCC', '潮州', 'CHAOZHOU', 'CZ');
newcCtys[25] = new Array('CTU', '成都', 'CHENGDU', 'CD');
newcCtys[26] = new Array('CYI', '嘉义', 'CHIAYI', 'JY');
newcCtys[27] = new Array('CIF', '赤峰', 'CHIFENG', 'CF');
newcCtys[28] = new Array('HIN', '清州', 'CHINJU', 'QZ');
newcCtys[29] = new Array('CKG', '重庆', 'CHONGQING', 'CQ');
newcCtys[30] = new Array('DLU', '大理', 'DALI', 'DL');
newcCtys[31] = new Array('DLC', '大连', 'DALIAN', 'DL');
newcCtys[32] = new Array('DDG', '丹东', 'DANDONG', 'DD');
newcCtys[33] = new Array('DQA', '大庆', 'DAQING', 'DQ');
newcCtys[34] = new Array('DAT', '大同', 'DATONG', 'DT');
newcCtys[35] = new Array('DAX', '达县', 'DAXIAN', 'DX');
newcCtys[36] = new Array('DZU', '大足', 'DAZU', 'DZ');
newcCtys[37] = new Array('DOY', '东营', 'DONGYING', 'DY');
newcCtys[38] = new Array('DNH', '敦煌', 'DUNHUANG', 'DH');
newcCtys[39] = new Array('DSN', '鄂尔多斯', 'EERDUOSI', 'EEDS');
newcCtys[40] = new Array('ENH', '恩施', 'ENSHI', 'ES');
newcCtys[41] = new Array('ERL', '二连浩特', 'ERLIANHAOTE', 'ERLIAN');
newcCtys[42] = new Array('FUO', '佛山', 'FOSHAN', 'FS');
newcCtys[43] = new Array('FUG', '阜阳', 'FUYANG', 'FY');
newcCtys[44] = new Array('FYN', '富蕴', 'FUYUN', 'FY');
newcCtys[45] = new Array('FOC', '福州', 'FUZHOU', 'FZ');
newcCtys[46] = new Array('KOW', '赣州', 'GANZHOU', 'GZ');
newcCtys[47] = new Array('GOQ', '格尔木', 'GEERMU', 'GEM');
newcCtys[48] = new Array('GHN', '广汉', 'GUANGHAN', 'GH');
newcCtys[49] = new Array('GYS', '广元', 'GUANGYUAN', 'GY');
newcCtys[50] = new Array('CAN', '广州', 'GUANGZHOU', 'GZ');
newcCtys[51] = new Array('KWL', '桂林', 'GUILIN', 'GL');
newcCtys[52] = new Array('KWE', '贵阳', 'GUIYANG', 'GY');
newcCtys[53] = new Array('GYU', '固原', 'GUYUAN', 'GY');
newcCtys[54] = new Array('HRB', '哈尔滨', 'HAERBIN', 'HEB');
newcCtys[55] = new Array('HAK', '海口', 'HAIKOU', 'HK');
newcCtys[56] = new Array('HLD', '海拉尔', 'HAILAR', 'HLE');
newcCtys[57] = new Array('HMI', '哈密', 'HAMI', 'HM');
newcCtys[58] = new Array('HDG', '邯郸', 'HANDAN', 'HD');
newcCtys[59] = new Array('HGH', '杭州', 'HANGZHOU', 'HZ');
newcCtys[60] = new Array('HZG', '汉中', 'HANZHONG', 'HZ');
newcCtys[61] = new Array('HFE', '合肥', 'HEFEI', 'HF');
newcCtys[62] = new Array('HEK', '黑河', 'HEIHE', 'HH');
newcCtys[63] = new Array('HNY', '衡阳', 'HENGYANG', 'HY');
newcCtys[64] = new Array('HTN', '和田', 'HETIAN', 'HT');
newcCtys[65] = new Array('HET', '呼和浩特', 'HOHHOT', 'HHHT');
newcCtys[66] = new Array('HKG', '香港', 'HONGKONG', 'XG');
newcCtys[67] = new Array('HIA', '淮安', 'HUAIAN', '');
newcCtys[68] = new Array('TXN', '黄山', 'HUANGSHANG', 'HS');
newcCtys[69] = new Array('HUZ', '徽州', 'HUIZHOU', 'HZ');
newcCtys[70] = new Array('JMU', '佳木斯', 'JIAMUSI', 'JMS');
newcCtys[71] = new Array('KNC', '吉安', 'JIAN', 'JA');
newcCtys[72] = new Array('JGN', '嘉峪关', 'JIAYUGUAN', 'JYG');
newcCtys[73] = new Array('JIL', '吉林', 'JILIN', 'JL');
newcCtys[74] = new Array('TNA', '济南', 'JINAN', 'JN');
newcCtys[75] = new Array('JDZ', '景德镇', 'JINGDEZHEN', 'JDZ');
newcCtys[76] = new Array('JGS', '井冈山', 'jinggangshan', 'JGS');
newcCtys[77] = new Array('JNG', '济宁', 'JINING', 'JN');
newcCtys[78] = new Array('JJN', '泉州晋江', 'JINJIANG', 'QZJJ');
newcCtys[79] = new Array('JNZ', '锦州', 'JINZHOU', 'JZ');
newcCtys[80] = new Array('JIU', '九江', 'JIUJIANG', 'JJ');
newcCtys[81] = new Array('CHW', '酒泉', 'JIUQUAN', 'JQ');
newcCtys[82] = new Array('JZH', '九寨沟', 'JIUZHAIGOU', 'JZG');
newcCtys[83] = new Array('JXA', '鸡西', 'JIXI', 'JX');
newcCtys[84] = new Array('KJI', '喀纳斯', 'KANASI', 'KLS');
newcCtys[85] = new Array('KGT', '康定', 'KANGDING', 'KD');
newcCtys[86] = new Array('KHH', '高雄', 'KAOHSIUNG', 'GX');
newcCtys[87] = new Array('KHG', '喀什', 'KASHI', 'KS');
newcCtys[88] = new Array('KRY', '克拉玛依', 'KELAMAYI', 'KLMY');
newcCtys[89] = new Array('KRL', '库尔勒', 'KORLA', 'KEL');
newcCtys[90] = new Array('KMG', '昆明', 'KUNMING', 'KM');
newcCtys[91] = new Array('KCA', '库车', 'KUQA', 'KC');
newcCtys[92] = new Array('KWJ', '光州', 'KWANGJU', 'GZ');
newcCtys[93] = new Array('LHW', '兰州', 'LANZHOU', 'LZ');
newcCtys[94] = new Array('LXA', '拉萨', 'LHASA', 'LS');
newcCtys[95] = new Array('LIA', '梁平', 'LIANGPING', 'LP');
newcCtys[96] = new Array('LYG', '连云港', 'LIANYUNGANG', 'LYG');
newcCtys[97] = new Array('LJG', '丽江', 'LIJIANG', 'LJ');
newcCtys[98] = new Array('LNJ', '临沧', 'LINCANG', 'LC');
newcCtys[99] = new Array('LXI', '林西', 'LINXI', 'LX');
newcCtys[100] = new Array('LYI', '临沂', 'LINYI', 'LY');
newcCtys[101] = new Array('LZY', '林芝', 'LINZHI', 'LZ');
newcCtys[102] = new Array('HZH', '黎平', 'LIPING', 'LP');
newcCtys[103] = new Array('LHN', '梨山', 'LISHAN', 'LS');
newcCtys[104] = new Array('LZH', '柳州', 'LIUZHOU', 'LZ');
newcCtys[105] = new Array('LYA', '洛阳', 'LUOYANG', 'LY');
newcCtys[106] = new Array('LUZ', '庐山', 'LUSHAN', 'LS');
newcCtys[107] = new Array('LZO', '泸州', 'LUZHOU', 'LZ');
newcCtys[108] = new Array('MFM', '澳门', 'MACAU', 'AM');
newcCtys[109] = new Array('LUM', '芒市', 'MANGSHI', 'MS');
newcCtys[110] = new Array('NZH', '满洲里', 'manzhouli', 'MZL');
newcCtys[111] = new Array('MXZ', '梅县', 'MEIXIAN', 'MX');
newcCtys[112] = new Array('MIG', '绵阳', 'MIANYANG', 'MY');
newcCtys[113] = new Array('OHE', '漠河', 'MOHE', 'MH');
newcCtys[114] = new Array('MDG', '牡丹江', 'MUDANJIANG', 'MDJ');
newcCtys[115] = new Array('NLT', '那拉提', 'NANATI', 'NLT');
newcCtys[116] = new Array('KHN', '南昌', 'NANCHANG', 'NC');
newcCtys[117] = new Array('NAO', '南充', 'NANCHONG', 'NC');
newcCtys[118] = new Array('NKG', '南京', 'NANJING', 'NJ');
newcCtys[119] = new Array('NNG', '南宁', 'NANNING', 'NN');
newcCtys[120] = new Array('NTG', '南通', 'NANTONG', 'NT');
newcCtys[121] = new Array('NNY', '南阳', 'NANYANG', 'NY');
newcCtys[122] = new Array('NAY', '南苑', 'NANYUAN', 'NY');
newcCtys[123] = new Array('NGB', '宁波', 'NINGBO', 'NB');
newcCtys[124] = new Array('PZI', '攀枝花', 'PANZHIHUA', 'PZH');
newcCtys[125] = new Array('JIQ', '黔江', 'QIANJIANG', 'QIANJ');
newcCtys[126] = new Array('IQM', '且末', 'QIEMO', 'QM');
newcCtys[127] = new Array('TAO', '青岛', 'QINGDAO', 'QD');
newcCtys[128] = new Array('SHP', '秦皇岛', 'QINGHUANGDAO', 'QHD');
newcCtys[129] = new Array('IQN', '庆阳', 'QINGYANG', 'QY');
newcCtys[130] = new Array('NDG', '齐齐哈尔', 'QIQIHAR', 'QQHE');
newcCtys[131] = new Array('JUZ', '衢州', 'QUZHOU', 'HZ');
newcCtys[132] = new Array('RKZ', '日喀则', 'RIKAZE', 'RKZ');
newcCtys[133] = new Array('SYX', '三亚', 'SANYA', 'SY');
newcCtys[134] = new Array('SHA', '上海虹桥', 'SHANGHAIHONGQIAO', 'SHHQ');
newcCtys[135] = new Array('PVG', '上海浦东', 'SHANGHAIPUDONG', 'SHPD');
newcCtys[136] = new Array('SWA', '汕头', 'SHANTOU', 'ST');
newcCtys[137] = new Array('SHS', '沙市', 'SHASHI', 'SS');
newcCtys[138] = new Array('SHE', '沈阳', 'SHENYANG', 'SY');
newcCtys[139] = new Array('SZX', '深圳', 'SHENZHEN', 'SZ');
newcCtys[140] = new Array('SJW', '石家庄', 'SHIJIAZHUANG', 'SJZ');
newcCtys[141] = new Array('SYM', '思茅', 'SIMAO', 'SM');
newcCtys[142] = new Array('SZV', '苏州', 'SUZHOU', 'SZ');
newcCtys[143] = new Array('TCG', '塔城', 'TACHENG', 'TC');
newcCtys[144] = new Array('TXG', '台中', 'TAICHUNG', 'TZ');
newcCtys[145] = new Array('TNN', '台南', 'TAINAN', 'TN');
newcCtys[146] = new Array('TPE', '台北', 'TAIPEI', 'TB');
newcCtys[147] = new Array('TYN', '太原', 'TAIYUAN', 'TY');
newcCtys[148] = new Array('HYN', '台州黄岩', 'TAIZHOUHUANGYAN', 'TZHY');
newcCtys[149] = new Array('TVS', '唐山', 'TANGSHAN', 'TS');
newcCtys[150] = new Array('TCZ', '腾冲', 'TENGCHONG', 'TC');
newcCtys[151] = new Array('TSN', '天津', 'TIANJIN', 'TJ');
newcCtys[152] = new Array('THQ', '天水', 'TIANSHUI', 'TS');
newcCtys[153] = new Array('TNH', '通化', 'TONGHUA', 'TH');
newcCtys[154] = new Array('TGO', '通辽', 'TONGLIAO', 'TL');
newcCtys[155] = new Array('TEN', '铜仁', 'TONGREN', 'TR');
newcCtys[156] = new Array('TOY', '富山', 'TOYAMA', 'FS');
newcCtys[157] = new Array('HLH', '乌兰浩特', 'ULANHOT', 'WLHT');
newcCtys[158] = new Array('URC', '乌鲁木齐', 'URUMQI', 'WLMQ');
newcCtys[159] = new Array('VTE', '万象', 'VIENTIANE', 'WX');
newcCtys[160] = new Array('WXN', '万州', 'WANXIAN', 'WZ');
newcCtys[161] = new Array('WXN', '万县', 'WANXIAN', 'WX');
newcCtys[162] = new Array('WEF', '潍坊', 'WEIFANG', 'WF');
newcCtys[163] = new Array('WEH', '威海', 'WEIHAI', 'WH');
newcCtys[164] = new Array('WNH', '文山', 'WENSHAN', 'WS');
newcCtys[165] = new Array('WNZ', '温州', 'WENZHOU', 'WZ');
newcCtys[166] = new Array('WUA', '乌海', 'WUHAI', 'WH');
newcCtys[167] = new Array('WUH', '武汉', 'WUHAN', 'WH');
newcCtys[168] = new Array('WHU', '芜湖', 'WUHU', 'WH');
newcCtys[169] = new Array('WUX', '无锡', 'WUXI', 'WX');
newcCtys[170] = new Array('WUS', '武夷山', 'WUYISHAN', 'WYS');
newcCtys[171] = new Array('WUZ', '梧州', 'WUZHOU', 'WZ');
newcCtys[172] = new Array('XMN', '厦门', 'XIAMEN', 'XM');
newcCtys[173] = new Array('XIY', '西安', 'XIAN', 'XA');
newcCtys[174] = new Array('XFN', '襄樊', 'XIANGFAN', 'XF');
newcCtys[175] = new Array('DIG', '香格里拉', 'XIANGGELILA', 'XGLL');
newcCtys[176] = new Array('XIC', '西昌', 'XICHANG', 'XC');
newcCtys[177] = new Array('XIL', '锡林浩特', 'XILINHOT', 'XLHT');
newcCtys[178] = new Array('XEN', '兴城', 'XINGCHENG', 'XC');
newcCtys[179] = new Array('XIN', '兴宁', 'XINGNING', 'XN');
newcCtys[180] = new Array('XNT', '邢台', 'XINGTAI', 'XT');
newcCtys[181] = new Array('ACX', '兴义', 'XINGYI', 'XY');
newcCtys[182] = new Array('XNN', '西宁', 'XINING', 'XN');
newcCtys[183] = new Array('JHG', '西双版纳', 'XISHUANGBANNA', 'XSBN');
newcCtys[184] = new Array('XUZ', '徐州', 'XUZHOU', 'XZ');
newcCtys[185] = new Array('ENY', '延安', 'YANAN', 'YA');
newcCtys[186] = new Array('YNZ', '盐城', 'YANCHENG', 'YC');
newcCtys[187] = new Array('YNJ', '延吉', 'YANJI', 'YJ');
newcCtys[188] = new Array('YNT', '烟台', 'YANTAI', 'YT');
newcCtys[189] = new Array('YBP', '宜宾', 'YIBIN', 'YB');
newcCtys[190] = new Array('YIH', '宜昌', 'YICHANG', 'YC');
newcCtys[191] = new Array('LDS', '伊春', 'YICHUN', 'YC');
newcCtys[192] = new Array('YLN', '铱兰', 'YILAN', 'YL');
newcCtys[193] = new Array('INC', '银川', 'YINCHUAN', 'YC');
newcCtys[194] = new Array('YIN', '伊宁', 'YINING', 'YN');
newcCtys[195] = new Array('YIW', '义乌', 'YIWU', 'YW');
newcCtys[196] = new Array('LLF', '永州', 'YONGZHOU', 'YZ');
newcCtys[197] = new Array('UYN', '榆林', 'YULIN', 'YL');
newcCtys[198] = new Array('YCU', '运城', 'YUNCHENG', 'YC');
newcCtys[199] = new Array('YUS', '玉树', 'YUSHU', 'YS');
newcCtys[200] = new Array('DYG', '张家界', 'ZHANGJIAJIE', 'ZJJ');
newcCtys[201] = new Array('ZHA', '湛江', 'ZHANJIANG', 'ZJ');
newcCtys[202] = new Array('ZAT', '昭通', 'ZHAOTONG', 'ZT');
newcCtys[203] = new Array('CGO', '郑州', 'ZHENGZHOU', 'ZZ');
newcCtys[204] = new Array('HJJ', '芷江', 'ZHIJIANG', 'ZJ');
newcCtys[205] = new Array('ZHY', '中卫', 'ZHONGWEI', 'ZW');
newcCtys[206] = new Array('HSN', '舟山', 'ZHOUSHAN', 'ZS');
newcCtys[207] = new Array('ZUH', '珠海', 'ZHUHAI', 'ZH');
//用于排序
function sortBy(arr, prop, desc) {
    var props = [], ret = [], i = 0, len = arr.length;
    if (typeof prop == 'string') {
        for (; i < len; i++) {
            var oI = arr[i];
            (props[i] = new String(oI && oI[prop] || ''))._obj = oI;
        }
    } else if (typeof prop == 'function') {
        for (; i < len; i++) {
            var oI = arr[i];
            (props[i] = new String(oI && prop(oI) || ''))._obj = oI;
        }
    } else if (typeof prop == 'number') {
        for (; i < len; i++) {
            var oI = arr[i];
            (props[i] = new String(oI && oI[prop] || ''))._obj = oI;
        }
    }
    else {
        throw '参数类型错误';
    }
    props.sort();
    for (i = 0; i < len; i++) {
        ret[i] = props[i]._obj;
    }
    if (desc) ret.reverse();
    return ret;
};
//对城市对排序 按照城市三字码排序
newcCtys=sortBy(newcCtys, 0);

//加载,hidcity 默认城市
function onLoadCity(selid, txtCityCodeId, hidcityCode, hidcity, hidDefalutFromCity) {
    var selOpt = null;
    var selText = "";
    var selValue = "";
    opt = new Option("--请选择--", "");
    document.getElementById(selid).options[0] = opt;
    //添加城市数据
    for (var i = 1; i <= newcCtys.length; i++) {
        if (newcCtys[i] != undefined && newcCtys[i] != null && newcCtys[i] != "") {
            selValue = newcCtys[i][0] + "--" + newcCtys[i][1] + "--" + newcCtys[i][3];
            selText = newcCtys[i][0];
            opt = new Option(selValue, selText);
            document.getElementById(selid).options[i] = opt;
        }
    }
    if (hidDefalutFromCity != undefined && hidDefalutFromCity != null && hidDefalutFromCity != "") {
        //加载默认出发城市
        var fromcity = document.getElementById(hidDefalutFromCity).value;
        if (fromcity != undefined && fromcity != null && fromcity != "") {
            var sel = document.getElementById(selid);
            for (var i = 0; i < sel.options.length; i++) {
                if (sel.options[i].text.toUpperCase().indexOf(fromcity.toUpperCase()) != -1) {
                    sel.options[i].selected = true;
                    document.getElementById(txtCityCodeId).value = sel.value;
                    document.getElementById(hidcityCode).value = sel.value; //三字码
                    document.getElementById(hidcity).value = sel.options[i].text.split('-')[2]; //城市
                    break;
                }
            }
        }
        else {
            //                   document.getElementById(selid).value = "";
            //                   document.getElementById(txtCityCodeId).value = "";
            //                   document.getElementById(hidcityCode).value = "";
            //                   document.getElementById(hidcity).value = "";
        }
    }
    if (hidcityCode != undefined && hidcityCode != null && hidcityCode != "") {
        //加载城市三字码
        var cityCode = document.getElementById(hidcityCode).value;
        if (cityCode != undefined && cityCode != null && cityCode != "") {
            var selObj = document.getElementById(selid);
            selObj.value = cityCode;
            document.getElementById(txtCityCodeId).value = cityCode;
            document.getElementById(hidcityCode).value = cityCode;
            document.getElementById(hidcity).value = selObj.options[selObj.selectedIndex].text.split('-')[2]; //城市
        }
    }
}
//选择
function selChange(selObj, txtId, hidcitycode, hidcity) {
        document.getElementById(txtId).value = selObj.value;
        document.getElementById(hidcitycode).value = selObj.value;
        document.getElementById(hidcity).value = selObj.options[selObj.selectedIndex].text.split('-')[2]; //城市
}
//输入
function txtChange(txtValue, selId, hidcitycode, hidcity) {
    try {
        var sel = document.getElementById(selId);
        if (txtValue != null && txtValue != "") {
            for (var i = 0; i < sel.options.length; i++) {
                if (sel.options[i].text.toUpperCase().indexOf(txtValue.toUpperCase()) != -1) {
                    sel.options[i].selected = true;
                    document.getElementById(hidcitycode).value = sel.value;
                    document.getElementById(hidcity).value = sel.options[i].text.split('-')[2]; //城市
                    break;
                }
            }
        } else {
            sel.value = "";
            var citycode = document.getElementById(hidcitycode);
            if (citycode != null) {
                citycode.value = "";
            }
            var city = document.getElementById(hidcity);
            if (city != null) {
                citycode.value = "";
            }
        }
    } catch (e) {

    }
}
//只能输入字母
function txtCode(obj) {
    //先把非字母的都替换掉，除了字母
    obj.value = obj.value.replace(/[^a-zA-Z]*/, "");
}