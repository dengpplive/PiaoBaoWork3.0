var AirCarrierS = new Array(25);
AirCarrierS[0] = new Array('BK', '奥凯');
AirCarrierS[1] = new Array('EU', '成都');
AirCarrierS[2] = new Array('G5', '华夏');
AirCarrierS[3] = new Array('KN', '中联');
AirCarrierS[4] = new Array('3U', '川航');
AirCarrierS[5] = new Array('FM', '上航');
AirCarrierS[6] = new Array('MF', '厦航');
AirCarrierS[7] = new Array('MU', '东航');
AirCarrierS[8] = new Array('SC', '山航');
AirCarrierS[9] = new Array('8L', '祥鹏');
AirCarrierS[10] = new Array('HU', '海航');
AirCarrierS[11] = new Array('PN', '西部');
AirCarrierS[12] = new Array('GS', '天津');
AirCarrierS[13] = new Array('JD', '首都');
AirCarrierS[14] = new Array('CZ', '南航');
AirCarrierS[15] = new Array('NS', 'NS');
AirCarrierS[16] = new Array('JR', '幸福');
AirCarrierS[17] = new Array('HO', '吉祥');
AirCarrierS[18] = new Array('CN', '新华');
AirCarrierS[19] = new Array('ZH', '深航');
AirCarrierS[20] = new Array('VD', '河南');
AirCarrierS[21] = new Array('KY', '昆明');
AirCarrierS[22] = new Array('CA', '国航');
AirCarrierS[23] = new Array('8C', '东星');
AirCarrierS[24] = new Array('9C', '春秋');
function getCarrier(code) {
    var Carrier = '';
    try {
        for (var i = 0; i < AirCarrierS.length; i++) {
            if (code == AirCarrierS[i][0]) {
                Carrier = AirCarrierS[i][1];
                break;
            }
        }
    }
    catch (e) {
    }
    return Carrier;
}
