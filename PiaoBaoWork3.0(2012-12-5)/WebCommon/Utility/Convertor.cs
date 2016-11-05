using System;

namespace PbProject.WebCommon.Utility
{
    public class Converter
    {
        public static byte[] GetBytes(int[] intArray)
        {
            byte[] result = null;
            if (intArray == null || intArray.Length < 1)
                return result;


            int intSize = 4, pos = 0;
            result = new byte[intArray.Length * intSize];
            byte[] intBuf = new byte[intSize];

            for (int i = 0; i < intArray.Length; i++)
            {
                intBuf = BitConverter.GetBytes(intArray[i]);
                Array.Reverse(intBuf);
                Array.Copy(intBuf, 0, result, pos, intSize);
                pos += intSize;
            }

            return result;
        }

    }
}
