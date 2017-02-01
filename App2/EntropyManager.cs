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
using Android.Hardware;
using System.Text.RegularExpressions;

namespace App2
{
    public static class EntropyManager
    {
        public const int BlockLength = 1024; // Size in bytes for a single block
        public const int BlocksToStore = 4; // At any time, keep up to 4*1024 = 4kb of random bytes stored in the SB
        private const int TotalBufferSize = BlockLength * BlocksToStore;

        private static StringBuilder sb = new StringBuilder();

        public static void FeedData(SensorType sensor, IList<float> values)
        {
            if (sb.Length < TotalBufferSize)
            {
                switch (sensor)
                {
                    case SensorType.Accelerometer:
                        sb.Append(values[0].ToString().Trim('.'));
                        sb.Append(values[1].ToString().Trim('.'));
                        sb.Append(values[2].ToString().Trim('.'));
                        break;
                    case SensorType.Gyroscope:
                        break;
                }
            }
        }

        public static string GetBlockOfEntropyBytes()
        {
            if (sb.Length >= BlockLength)
            {
                string block = sb.ToString().Substring(0, BlockLength).Trim('.');
                sb.Remove(0, 1024); // remove the block that was just taken

                return block;
            }
            else
            {
                // TODO: we've run out of bytes from sensors, so we should wait until we have more..
                //System.Threading.Thread.Sleep(200);
                //return GetBlockOfEntropyBytes();
                return "__NO_ENTROPY__";
            }
        }
    }
}