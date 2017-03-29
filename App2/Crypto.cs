using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App2
{
    public static class Crypto
    {
        public static byte[] XorStrings(byte[] b1, byte[] b2)
        {
            byte[] xord = new byte[b1.Length];

            for (int i = 0; i < b1.Length; i++)
            {
                xord[i] = Convert.ToByte(Convert.ToInt32(b1[i]) ^ Convert.ToInt32(b2[i]));
            }

            return xord;
        }

        public static byte[] XorStrings(string str1, string str2)
        {
            var encodedA = Encoding.UTF8.GetBytes(str1);
            var encodedB = Encoding.UTF8.GetBytes(str2);

            return XorStrings(encodedA, encodedB);
        }
    }
}