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
using System.IO;

namespace App2
{
    public static class Identity
    {
        public static string Username { get; private set; }

        public static void LoadUsername(string externalDir)
        {
            if (Username == null)
            {
                string metxtFileLoc = Path.Combine(externalDir, "me.txt");

                if (!File.Exists(metxtFileLoc))
                {
                    // Cannot load username
                    return;
                }

                StreamReader sr = new StreamReader(metxtFileLoc);
                string username = sr.ReadToEnd();
                sr.Close();
            }
        }

        public static void SetUsername(string externalDir, string username)
        {
            // Update this class' record of the username
            Username = username;

            // Write name to me.txt
            string metxtFileLoc = Path.Combine(externalDir, "me.txt");

            // Create and use StreamWriter to output the text to the file
            StreamWriter sw = new StreamWriter(metxtFileLoc, false);
            sw.Write(username);
            sw.Close();
        }
    }
}