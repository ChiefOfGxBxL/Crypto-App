using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App2
{
    public class PadManager
    {
        public string _appDir;
        public string _contactsDir;

        public PadManager(string path)
        {
            _appDir = path;
            _contactsDir = Path.Combine(_appDir, "contacts");

            CreateDirIfNotExists(Path.Combine(_appDir, "contacts")); // ensures the contacts path exists
        }

        /// <summary>
        /// Creates a directory at the specified path if it does not exist.
        /// Returns the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string CreateDirIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public Padbook GetPadbookForUsername(string username)
        {
            if (username.Length == 0) return null; // must specify username

            // The pad is located in the 'contacts' folder, then in a folder named after the user
            string userContactDir = Path.Combine(_contactsDir, username);
            string padFilePath = Path.Combine(userContactDir, "pad.txt");

            if(!File.Exists(padFilePath))
            {
                // Here the pad file does not exist for the given contact
                // We simply create an empty one; sending messages in ViewContactActivity
                // handles cases when no pads are available in the file.
                var x = File.Create(padFilePath);
                x.Close();
            }

            return new Padbook(padFilePath);
        }

        public Padbook CreateNewPadForUsername(string username, string[] pads)
        {
            // TODO: creates a new padbook for a username, initializing
            // the padbook with specified pads
            return null;
        }
    }
}