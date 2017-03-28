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
        private string _appDir;
        private string _contactsDir;

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

            if (File.Exists(padFilePath))
            {
                return new Padbook(_contactsDir, username);
            }
            else
            {
                // Here the pad file does not exist for the given contact
                // We could inform the user that something went wrong, and that they
                // need to generate more pads for this contact
                Console.WriteLine("Error: could not find pad.txt file for username " + username);
                return null;
            }
        }

        public Padbook CreateNewPadForUsername(string username, string[] pads)
        {
            // TODO: creates a new padbook for a username, initializing
            // the padbook with specified pads
            return null;
        }
    }
}