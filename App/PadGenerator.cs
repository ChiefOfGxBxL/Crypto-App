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

namespace App.Resources.layout
{
    public static class PadGenerator
    {
        /// <summary>
        /// Generates a random pad of given length (in bytes) using the specified
        /// entropy source.
        /// </summary>
        /// <param name="es">EntropySource to use for pad generation</param>
        /// <param name="length">Length of pad in bytes</param>
        /// <returns></returns>
        public static string generatePad(IEntropySource es, int length)
        {
            // TODO
            es.generateNoiseBytesOfLength(length);
            return "";
        }
    }
}