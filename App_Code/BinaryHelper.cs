using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GambitChallenge.App_Code
{
    public static class BinaryHelper
    {
        public static string IntTo16BitBinaryString(int value)
        {
            return Convert.ToString(value, 2).PadLeft(16, '0');
        }
        
        /*
        public static int BinaryStringToSignedInt(string bstring)
        {
            int i = Convert.ToInt32(bstring, 2);
            byte[] bytes = BitConverter.GetBytes(i);
            return BitConverter.ToInt32(bytes, 0);
        }
        */

        public static float BinaryStringToSingle(string bstring)
        {
            int i = Convert.ToInt32(bstring, 2);
            byte[] bytes = BitConverter.GetBytes(i);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static int BinaryStringToBCD(string bstring)
        {
            string high = bstring.Substring(0, 4);
            string low = bstring.Substring(4, 4);

            int tens = Convert.ToInt32(high, 2);
            int ones = Convert.ToInt32(low, 2);

            return tens * 10 + ones;
        }
    }
}