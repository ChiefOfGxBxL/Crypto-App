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
        public const int MessageSize = 160; // A message may be up to 160 bytes (excludes meta information)
        public const int BlockLength = MessageSize * 6; // A block may hold up to 6 messages worth of bytes
        public const int BlocksToStore = 4; // At any time, keep up to X blocks stored in the SB (useful for repeat calls)
        private const int TotalBufferSize = BlockLength * BlocksToStore;

        private static StringBuilder sb = new StringBuilder(TotalBufferSize);

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
                        sb.Append(values[0].ToString().Trim('.'));
                        sb.Append(values[1].ToString().Trim('.'));
                        sb.Append(values[2].ToString().Trim('.'));
                        break;
                }
            }
        }

        public static int GetSbSize()
        {
            return sb.Length;
        }
        public static int GetTotalSbSize()
        {
            return TotalBufferSize;
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