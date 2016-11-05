namespace DataBase.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    public static class StringUtils
    {
        private static readonly string[] cHighPinyins = new string[] { 
            "cjwgnspgcgnesypbtyyzdxykygtdjnmjqmbsgzscyjsyyzpgkbzgycywykgkljswkpjqhyzwddzlsgmrypywwcckznkydg", "ttnjjeykkzytcjnmcylqlypyqfqrpzslwbtgkjfyxjwzltbncxjjjjzxdttsqzycdxxhgckbphffssyybgmxlpbylllhlx", "spzmyjhsojnghdzqyklgjhxgqzhxqgkezzwyscscjxyeyxadzpmdssmzjzqjyzcdjewqjbdzbxgznzcpwhkxhqkmwfbpby", "dtjzzkqhylygxfptyjyyzpszlfchmqshgmxxsxjjsdcsbbqbefsjyhxwgzkpylqbgldlcctnmayddkssngycsgxlyzaybn", "ptsdkdylhgymylcxpycjndqjwxqxfyyfjlejbzrxccqwqqsbzkymgplbmjrqcflnymyqmsqyrbcjthztqfrxqhxmjjcjlx", "qgjmshzkbswyemyltxfsydsglycjqxsjnqbsctyhbftdcyzdjwyghqfrxwckqkxebptlpxjzsrmebwhjlbjslyysmdxlcl", "qkxlhxjrzjmfqhxhwywsbhtrxxglhqhfnmcykldyxzpwlggsmtcfpajjzyljtyanjgbjplqgdzyqyaxbkysecjsznslyzh", "zxlzcghpxzhznytdsbcjkdlzayfmydlebbgqyzkxgldndnyskjshdlyxbcghxypkdqmmzngmmclgwzszxzjfznmlzzthcs", "ydbdllscddnlkjykjsycjlkohqasdknhcsganhdaashtcplcpqybsdmpjlpcjoqlcdhjjysprchnknnlhlyyqyhwzptczg", "wwmzffjqqqqyxaclbhkdjxdgmmydjxzllsygxgkjrywzwyclzmssjzldbydcpcxyhlxchyzjqsqqagmnyxpfrkssbjlyxy", "syglnscmhcwwmnzjjlxxhchsyd ctxrycyxbyhcsmxjsznpwgpxxtaybgajcxlysdccwzocwkccsbnhcpdyznfcyytyckx", "kybsqkkytqqxfcwchcykelzqbsqyjqcclmthsywhmktlkjlycxwheqqhtqhzpqsqscfymmdmgbwhwlgsllysdlmlxpthmj", "hwljzyhzjxhtxjlhxrswlwzjcbxmhzqxsdzpmgfcsglsxymjshxpjxwmyqksmyplrthbxftpmhyxlchlhlzylxgsssstcl", "sldclrpbhzhxyyfhbbgdmycnqqwlqhjjzywjzyejjdhpblqxtqkwhlchqxagtlxljxmslxhtzkzjecxjcjnmfbycsfywyb", "jzgnysdzsqyrsljpclpwxsdwejbjcbcnaytwgmpabclyqpclzxsbnmsggfnzjjbzsfzyndxhplqkzczwalsbccjxjyzhwk", "ypsgxfzfcdkhjgxdlqfsgdslqwzkxtmhsbgzmjzrglyjbpmlmsxlzjqqhzsjczydjwbmjklddpmjegxyhylxhlqyqhkycw", 
            "cjmyyxnatjhyccxzpcqlbzwwytwbqcmlpmyrjcccxfpznzzljplxxyztzlgdldcklyrlzgqtgjhhgjljaxfgfjzslcfdqz", "lclgjdjcsnclljpjqdcclcjxmyzftsxgcgsbrzxjqqctzhgyqtjqqlzxjylylbcyamcstylpdjbyregkjzyzhlyszqlznw", "czcllwjqjjjkdgjzolbbzppglghtgzxyghzmycnqsycyhbhgxkamtxyxnbskyzzgjzlqjdfcjxdygjqjjpmgwgjjjpkqsb", "gbmmcjssclpqpdxcdyykywcjddyygywrhjrtgznyqldkljszzgzqzjgdykshpzmtlcpwnjafyzdjcnmwescyglbtzcgmss", "llyxqsxsbsjsbbggghfjlypmzjnlyywdqshzxtyywhmcyhywdbxbtlmsyyyfsxjcsdxxlhjhf sxzqhfzmzcztqcxzxrtt", "djhnnyzqqmnqdmmglydxmjgdhcdyzbffallztdltfxmxqzdngwqdbdczjdxbzgsqqddjcmbkzffxmkdmdsyyszcmljdsyn", "sprskmkmpcklgdbqtfzswtfgglyplljzhgjjgypzltcsmcnbtjbqfkthbyzgkpbbymtdssxtbnpdkleycjnycdykzddhqh", "sdzsctarlltkzlgecllkjlqjaqnbdkkghpjtzqksecshalqfmmgjnlyjbbtmlyzxdcjpldlpcqdhzycbzsczbzmsljflkr", "zjsnfrgjhxpdhyjybzgdljcsezgxlblhyxtwmabchecmwyjyzlljjyhlgbdjlslygkdzpzxjyyzlwcxszfgwyydlyhcljs", "cmbjhblyzlycblydpdqysxqzbytdkyyjyycnrjmpdjgklcljbctbjddbblblczqrppxjcglzcshltoljnmdddlngkaqhqh", "jhykheznmshrp qqjchgmfprxhjgdychghlyrzqlcyqjnzsqtkqjymszswlcfqqqxyfggyptqwlmcrnfkkfsyylqbmqamm", "myxctpshcptxxzzsmphpshmclmldqfyqxszyjdjjzzhqpdszglstjbckbxyqzjsgpsxqzqzrqtbdkyxzkhhgflbcsmdldg", "dzdblzyycxnncsybzbfglzzxswmsccmqnjqsbdqsjtxxmbltxzclzshzcxrqjgjylxzfjphyxzqqydfqjjlzznzjcdgzyg", "ctxmzysctlkphtxhtlbjxjlxscdqxcbbtjfqzfsltjbtkqbxxjjljchczdbzjdczjdcprnpqcjpfczlclzxbdmxmphjsgz", "gszzqlylwtjpfsyasmcjbtzyycwmytcsjjlqcqlwzmalbxyfbpnlsfhtgjwejjxxglljstgshjqlzfkcgnndszfdeqfhbs", "aqtgylbxmmygszldydqmjjrgbjtkgdhgkblqkbdmbylxwcxyttybkmrtjzxqjbhlmhmjjzmqasldcyxyqdlqcafywyxqhz"
         };
        private static readonly char[] cLowChineseChars = new char[] { 
            '啊', '芭', '擦', '搭', '蛾', '发', '噶', '哈', '击', '喀', '垃', '妈', '拿', '欧', '啪', '期', 
            '然', '撒', '塌', '挖', '昔', '压', '匝'
         };
        private static readonly char[] cLowPinyins = new char[] { 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 
            'r', 's', 't', 'w', 'x', 'y', 'z'
         };
        private static readonly object[] cPinyinMap = new object[] { 
            0x4e01, "dz", 0x4e07, "wm", 0x4e14, "qj", 0x4e50, "ly", 0x4e58, "cs", 0x4e5c, "mn", 0x4e7e, "qg", 0x4e9f, "qj", 
            0x4ec7, "cq", 0x4ee1, "yg", 0x4f1a, "hk", 0x4f20, "cz", 0x4f3a, "cs", 0x4f3d, "jgq", 0x4f43, "dt", 0x4f5a, "yd", 
            0x4f74, "ne", 0x4f97, "dt", 0x4fa5, "jy", 0x4fa7, "cz", 0x4fbf, "bp", 0x4fde, "ys", 0x4fdf, "sq", 0x5018, "tc", 
            0x5080, "kg", 0x5176, "qj", 0x5179, "zc", 0x51af, "fp", 0x5018, "tc", 0x5080, "kg", 0x5176, "qj", 0x5179, "zc", 
            0x51f9, "aw", 0x5228, "pb", 0x5238, "qx", 0x5239, "cs", 0x5261, "ys", 0x527f, "jc", 0x52fa, "sb", 0x5319, "sc", 
            0x5339, "py", 0x533a, "qo", 0x5352, "zc", 0x5355, "dcs", 0x5361, "kq", 0x5382, "ca", 0x5395, "cs", 0x53a6, "sx", 
            0x53c2, "cs", 0x53e5, "jg", 0x53e8, "dt", 0x53ec, "zs", 0x53f6, "yx", 0x5401, "yx", 0x5408, "hg", 0x5413, "xh", 
            0x5421, "pb", 0x5426, "fp", 0x542d, "kh", 0x5446, "da", 0x5454, "td", 0x5472, "zc", 0x5475, "kha", 0x5480, "jz", 
            0x5496, "gk", 0x54af, "lkg", 0x54b3, "kh", 0x54d7, "hy", 0x54e6, "woe", 0x5514, "wn", 0x552c, "hx", 0x558b, "dz", 
            0x558f, "nr", 0x5594, "wo", 0x55b3, "zc", 0x55c4, "sa", 0x55cc, "ya", 0x55d2, "td", 0x560f, "jg", 0x5618, "xs", 
            0x562c, "zc", 0x5632, "cz", 0x563f, "hm", 0x5671, "xj", 0x56a3, "xa", 0x56dd, "nj", 0x56e4, "td", 0x5708, "qj", 
            0x571c, "yh", 0x5729, "xw", 0x573b, "yq", 0x5747, "jy", 0x574f, "hp", 0x577b, "dc", 0x578c, "td", 0x57d4, "pb", 
            0x57e4, "pb", 0x5806, "dz", 0x5815, "dh", 0x5821, "bp", 0x5854, "td", 0x58f3, "kq", 0x592f, "hb", 0x5939, "jg", 
            0x5947, "qj", 0x5951, "qx", 0x59e5, "lm", 0x5a29, "mw", 0x5bbf, "sx", 0x5c06, "jq", 0x5c09, "wy", 0x5c22, "yw", 
            0x5c3e, "wy", 0x5c3f, "ns", 0x5c4f, "pb", 0x5c5e, "sz", 0x5c6f, "tz", 0x5c79, "yg", 0x5cd2, "dt", 0x5cd9, "zs", 
            0x5ce4, "qj", 0x5d4c, "qk", 0x5df7, "xh", 0x5e31, "dc", 0x5e62, "cz", 0x5e7f, "ga", 0x5ed1, "qj", 0x5f04, "nl", 
            0x5f39, "td", 0x5f3a, "qj", 0x5f77, "pf", 0x5fea, "zs", 0x606b, "td", 0x6076, "ew", 0x609d, "lk", 0x615d, "tn", 
            0x6206, "zg", 0x620c, "xq", 0x620f, "xh", 0x6241, "bp", 0x6252, "bp", 0x625b, "kg", 0x6273, "bp", 0x6298, "zs", 
            0x62c2, "fb", 0x62d7, "na", 0x62ec, "kg", 0x62fd, "zy", 0x631d, "wz", 0x631f, "xj", 0x63b8, "sd", 0x63ba, "cs", 
            0x63d0, "td", 0x64ae, "cz", 0x6512, "zc", 0x65bc, "yw", 0x65e0, "wm", 0x66b4, "bp", 0x66dd, "pb", 0x66f3, "zy", 
            0x66fe, "cz", 0x671d, "cz", 0x671f, "qj", 0x672f, "sz", 0x6753, "sb", 0x679e, "cz", 0x67b8, "jg", 0x67c1, "td", 
            0x67dc, "gj", 0x67e5, "cz", 0x6805, "zs", 0x680e, "ly", 0x6816, "qx", 0x681d, "kg", 0x6821, "xj", 0x6867, "gh", 
            0x690e, "zc", 0x6939, "zs", 0x6942, "zc", 0x6977, "kj", 0x69db, "kj", 0x6b39, "yq", 0x6b59, "xs", 0x6b96, "zs", 
            0x6c0f, "zs", 0x6c64, "ts", 0x6c88, "sc", 0x6c8c, "zd", 0x6c93, "td", 0x6cca, "bp", 0x6ccc, "mb", 0x6cf7, "ls", 
            0x6cfa, "lp", 0x6d45, "qj", 0x6d4d, "kh", 0x6d52, "hx", 0x6d5a, "xj", 0x6d8c, "yc", 0x6da1, "wg", 0x6e11, "sm", 
            0x6e6b, "qj", 0x6e83, "kh", 0x6eb1, "zq", 0x6f2f, "tl", 0x6f84, "dc", 0x6fb9, "td", 0x7011, "pb", 0x7085, "jg", 
            0x7094, "qg", 0x70ae, "pb", 0x7118, "td", 0x712f, "zc", 0x728d, "qj", 0x7387, "sl", 0x73a2, "fb", 0x753a, "td", 
            0x755c, "cx", 0x756a, "pf", 0x758b, "ysp", 0x759f, "yn", 0x7615, "xj", 0x76db, "sc", 0x7701, "sx", 0x77a7, "qy", 
            0x77bf, "qj", 0x77dc, "qjg", 0x77f3, "sd", 0x7809, "xh", 0x784c, "lg", 0x78c5, "bp", 0x7962, "mn", 0x796d, "jz", 
            0x7985, "cs", 0x79cd, "zc", 0x79d8, "mlb", 0x7a3d, "jq", 0x7aa8, "yx", 0x7b60, "jy", 0x7c98, "zn", 0x7ca5, "zy", 
            0x7cfb, "xj", 0x7e41, "fp", 0x7e47, "yz", 0x7ea2, "hg", 0x7ea4, "xq", 0x7ea5, "hg", 0x7eb6, "lg", 0x7ed9, "gj", 
            0x7ee8, "dt", 0x7f09, "jq", 0x7f0f, "pb", 0x7f32, "zsq", 0x7f34, "jz", 0x7fdf, "zd", 0x8019, "pb", 0x812f, "pf", 
            0x814a, "lx", 0x814c, "ya", 0x8180, "bp", 0x81ed, "cx", 0x822c, "bp", 0x8235, "dt", 0x827e, "ay", 0x8292, "mw", 
            0x8298, "pb", 0x82a5, "jg", 0x82ce, "zn", 0x82d5, "st", 0x82e3, "jq", 0x8304, "qj", 0x8308, "zc", 0x831c, "qx", 
            0x8351, "yt", 0x8360, "jq", 0x8364, "hx", 0x8365, "yx", 0x839e, "gw", 0x83a9, "fp", 0x83c0, "wy", 0x845a, "sr", 
            0x8513, "mw", 0x851a, "wy", 0x8549, "jq", 0x85cf, "cz", 0x85d3, "xl", 0x8679, "hj", 0x867e, "xh", 0x86b5, "kh", 
            0x86c7, "sy", 0x86e4, "gh", 0x86f8, "sx", 0x86fe, "ey", 0x8721, "lz", 0x8764, "qy", 0x884c, "hx", 0x8870, "sc", 
            0x88b7, "jq", 0x88e8, "bp", 0x88f3, "sc", 0x891a, "cz", 0x8983, "tq", 0x89c1, "jx", 0x89e3, "jx", 0x8bc6, "sz", 
            0x8bf4, "sy", 0x8c03, "td", 0x8c37, "gy", 0x8c89, "hm", 0x8d3e, "jg", 0x8d84, "qj", 0x8e4a, "qx", 0x8e72, "dc", 
            0x8f66, "cj", 0x8f67, "zyg", 0x8f9f, "bp", 0x9002, "sk", 0x9057, "yw", 0x90aa, "xy", 0x90c7, "hx", 0x9162, "cz", 
            0x91cd, "zc", 0x94af, "bp", 0x94bf, "dt", 0x94c5, "qy", 0x94db, "dc", 0x94e4, "td", 0x94eb, "yd", 0x9550, "hg", 
            0x9561, "xtc", 0x957f, "cz", 0x960f, "ye", 0x961a, "kh", 0x963d, "yd", 0x963f, "ae", 0x9642, "pb", 0x964d, "jx", 
            0x9676, "ty", 0x9697, "wk", 0x96b9, "zc", 0x9730, "xs", 0x9753, "lj", 0x9769, "gj", 0x9798, "qs", 0x9888, "jg", 
            0x9889, "jx", 0x988c, "hg", 0x98a4, "cz", 0x9967, "xt", 0x9a6e, "td", 0x9a80, "td", 0x9aa0, "bp", 0x9adf, "sb", 
            0x9b32, "lg", 0x9b44, "pbt", 0x9cad, "zq", 0x9e1f, "nd", 0x9e44, "hg", 0x9e58, "hg", 0x9e87, "qj", 0x9f3d, "yq", 
            0x9f50, "qj", 0x9f88, "yk", 0x9f9f, "gjq"
         };
        private static Encoding gb2312 = Encoding.GetEncoding("GB2312");
        private static Hashtable pinyinMap;

        public static string ByteArrayToHex(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private static int CompareChineseChar(byte[] bytes1, char char2)
        {
            byte[] bytes = gb2312.GetBytes(new char[] { char2 });
            if (bytes1[0] > bytes[0])
            {
                return 1;
            }
            if (bytes1[0] == bytes[0])
            {
                return (bytes1[1] - bytes[1]);
            }
            return -1;
        }

        public static string Decrypt(string hex)
        {
            if (!string.IsNullOrEmpty(hex))
            {
                return DecryptFromByteArray(HexToByteArray(hex));
            }
            return string.Empty;
        }

        public static string DecryptFromByteArray(byte[] data)
        {
            string str;
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (CryptoStream stream2 = new CryptoStream(stream, new TripleDESCryptoServiceProvider().CreateDecryptor(Key(), Iv()), CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[data.Length];
                    int count = stream2.Read(buffer, 0, buffer.Length);
                    str = new UTF8Encoding().GetString(buffer, 0, count);
                }
            }
            return str;
        }

        public static string Encrypt(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return ByteArrayToHex(EncryptToByteArray(str));
            }
            return string.Empty;
        }

        public static byte[] EncryptToByteArray(string str)
        {
            byte[] buffer;
            byte[] bytes = new UTF8Encoding().GetBytes(str);
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream stream2 = new CryptoStream(stream, new TripleDESCryptoServiceProvider().CreateEncryptor(Key(), Iv()), CryptoStreamMode.Write))
                {
                    stream2.Write(bytes, 0, bytes.Length);
                    stream2.FlushFinalBlock();
                    buffer = stream.ToArray();
                }
            }
            return buffer;
        }

        public static bool EndsWithIgnoreCase(string s1, string s2)
        {
            return s1.EndsWith(s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsIgnoreCase(string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsIgnoreCase(string s1, int index1, string s2, int index2, int length)
        {
            return (string.Compare(s1, index1, s2, index2, length, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private static char? GetCharPinyinCode(char chineseChar)
        {
            if (pinyinMap == null)
            {
                pinyinMap = new Hashtable();
                for (int i = 0; i < (cPinyinMap.Length / 2); i++)
                {
                    pinyinMap[cPinyinMap[i * 2]] = cPinyinMap[(i * 2) + 1];
                }
            }
            string str = (string) pinyinMap[(int) chineseChar];
            if (str != null)
            {
                return new char?(str[0]);
            }
            return GetSingleCharPinyinCode(chineseChar);
        }

        public static string GetHashCode(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            return str.GetHashCode().ToString("x", CultureInfo.InvariantCulture);
        }

        private static char GetHighPinyins(byte[] bytes)
        {
            return cHighPinyins[bytes[0] - 0xd8][(bytes[1] - 160) - 1];
        }

        public static string GetPinyinCode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str)
            {
                char c = ch;
                char? charPinyinCode = null;
                if ((ch >= 0xff01) && (ch <= 0xff5e))
                {
                    c = (char) (ch - 0xfee0);
                }
                else if (ch > '\x007f')
                {
                    charPinyinCode = GetCharPinyinCode(ch);
                }
                char? nullable2 = charPinyinCode;
                int? nullable3 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault()) : null;
                if (nullable3.HasValue)
                {
                    builder.Append(charPinyinCode);
                }
                else if ((((c >= '0') && (c <= '9')) || ((c >= 'A') && (c <= 'Z'))) || ((c >= 'a') && (c <= 'z')))
                {
                    builder.Append(char.ToLower(c));
                }
            }
            return builder.ToString();
        }

        private static void GetSimpleEncryptKey(string key, out int startKey, out int multKey, out int addKey)
        {
            if (string.IsNullOrEmpty(key) || (key.Length != 6))
            {
                throw new InvalidOperationException("key 必须是6位数字");
            }
            startKey = int.Parse(key.Substring(0, 2));
            multKey = int.Parse(key.Substring(2, 2));
            addKey = int.Parse(key.Substring(4, 2));
        }

        private static char? GetSingleCharPinyinCode(char chineseChar)
        {
            byte[] bytes = gb2312.GetBytes(new char[] { chineseChar });
            if (bytes[0] >= 0xd8)
            {
                return new char?(GetHighPinyins(bytes));
            }
            for (int i = cLowChineseChars.Length - 1; i >= 0; i--)
            {
                char ch = cLowChineseChars[i];
                if (CompareChineseChar(bytes, ch) >= 0)
                {
                    return new char?(cLowPinyins[i]);
                }
            }
            return null;
        }

        public static byte[] HexToByteArray(string hex)
        {
            int num = hex.Length / 2;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                buffer[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return buffer;
        }

        private static byte[] Iv()
        {
            return new byte[] { 0x60, 0x37, 0x53, 220, 0x61, 0x51, 0xac, 170 };
        }

        private static byte[] Key()
        {
            return new byte[] { 
                0x7b, 0x61, 0xb1, 0x6d, 240, 0x83, 0xa4, 0xed, 0x57, 0x6c, 0x76, 0xd6, 230, 0xb5, 0x17, 0xf6, 
                80, 0x67, 0xd9, 0x34, 0x25, 0x95, 0x2f, 0xd0
             };
        }

        public static string MD5(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return ByteArrayToHex(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        public static string Repeat(string str, int times)
        {
            StringBuilder builder = new StringBuilder(str.Length * times);
            for (int i = 0; i < times; i++)
            {
                builder.Append(str);
            }
            return builder.ToString();
        }

        public static string Replace(string template, string placeholder, string replacement, StringComparison comparisonType)
        {
            if (template == null)
            {
                return null;
            }
            int index = template.IndexOf(placeholder, comparisonType);
            if (index < 0)
            {
                return template;
            }
            return new StringBuilder(template.Substring(0, index)).Append(replacement).Append(Replace(template.Substring(index + placeholder.Length), placeholder, replacement, comparisonType)).ToString();
        }

        public static string ReplaceFirst(string template, string placeholder, string replacement, StringComparison comparisonType)
        {
            if (template == null)
            {
                return null;
            }
            int index = template.IndexOf(placeholder, comparisonType);
            if (index < 0)
            {
                return template;
            }
            return new StringBuilder(template.Substring(0, index)).Append(replacement).Append(template.Substring(index + placeholder.Length)).ToString();
        }

        public static string SimpleDecrypt(string str, string key)
        {
            int num;
            int num2;
            int num3;
            int num4;
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            GetSimpleEncryptKey(key, out num, out num2, out num3);
            if (!int.TryParse(str.Substring(0, 2), NumberStyles.HexNumber, null, out num4))
            {
                throw new InvalidOperationException("密文错误");
            }
            num += num4;
            num2 += num4;
            num3 += num4;
            int num5 = (str.Length / 2) - 1;
            byte[] bytes = new byte[num5];
            for (int i = 0; i < num5; i++)
            {
                byte num7 = byte.Parse(str.Substring((i * 2) + 2, 2), NumberStyles.HexNumber);
                bytes[i] = (byte) (num7 ^ (num >> 8));
                num = ((num7 + num) * num2) + num3;
            }
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static string SimpleEncrypt(string str, string key)
        {
            int num;
            int num2;
            int num3;
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            GetSimpleEncryptKey(key, out num, out num2, out num3);
            int num4 = new Random().Next(100);
            num += num4;
            num2 += num4;
            num3 += num4;
            string str2 = num4.ToString("x2");
            foreach (byte num5 in Encoding.UTF8.GetBytes(str))
            {
                byte num6 = (byte) (num5 ^ (num >> 8));
                str2 = str2 + num6.ToString("x2");
                num = ((num6 + num) * num2) + num3;
            }
            return str2;
        }

        public static string[] SplitByByte(string str, int len)
        {
            int num;
            if (string.IsNullOrEmpty(str))
            {
                return new string[] { string.Empty };
            }
            IList<StringBuilder> list = new List<StringBuilder>();
            StringBuilder item = null;
            int num2 = 0;
            for (num = 0; num < str.Length; num++)
            {
                char ch = str[num];
                int num3 = (ch < '\x0080') ? 1 : 2;
                if ((num2 + num3) > len)
                {
                    item = null;
                    num2 = 0;
                }
                if (item == null)
                {
                    item = new StringBuilder();
                    list.Add(item);
                }
                item.Append(ch);
                num2 += num3;
            }
            string[] strArray = new string[list.Count];
            for (num = 0; num < list.Count; num++)
            {
                strArray[num] = list[num].ToString();
            }
            return strArray;
        }

        public static bool StartsWithIgnoreCase(string s1, string s2)
        {
            return s1.StartsWith(s2, StringComparison.OrdinalIgnoreCase);
        }

        public static string[] TrimAll(string[] array)
        {
            string[] strArray = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                strArray[i] = array[i].Trim();
            }
            return strArray;
        }

        private static void WriteToFile()
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            builder2.Append('{');
            int num = 0;
            for (int i = 0; i < (cPinyinMap.Length / 2); i++)
            {
                int num3 = (int) cPinyinMap[i * 2];
                char ch = (char) num3;
                string str = (string) cPinyinMap[(i * 2) + 1];
                if (i > 0)
                {
                    builder2.Append(',');
                }
                builder2.Append('"').Append(num3).Append("\":\"").Append(str).Append('"');
                builder.Append(ch).Append(' ').Append(num3).Append(' ').Append(str);
                char? singleCharPinyinCode = GetSingleCharPinyinCode(ch);
                if (singleCharPinyinCode.HasValue && (str[0] != singleCharPinyinCode.Value))
                {
                    builder.Append(' ').Append(singleCharPinyinCode);
                    num++;
                }
                if ((str.Length == 1) && (str[0] == singleCharPinyinCode.Value))
                {
                    builder.Append(" <-");
                }
                builder.AppendLine();
            }
            builder.AppendLine(num + " 个多音字第一音与字典中的不同");
            builder2.AppendLine("};");
            File.WriteAllText(@"E:\trd\Carpa.NET\docs\ref\多音字表.txt", builder.ToString() + builder2.ToString(), Encoding.Default);
        }
    }
}

