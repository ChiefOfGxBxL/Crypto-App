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
    public class ContactBook
    {
        public string BaseDirectory { get; set; }
        public string ContactsDirectory { get; set; }

        public ContactBook(string appPath)
        {
            // Use GetExternalFilesDir(null) when instantiating
            BaseDirectory = appPath;

            var contactsDir = Path.Combine(BaseDirectory, "contacts");
            if (!Directory.Exists(contactsDir))
            {
                Directory.CreateDirectory(contactsDir);
            }
            ContactsDirectory = Path.Combine(BaseDirectory, "contacts");
        }

        public List<string> GetContacts()
        {
            List<string> cleanDirs = new List<string> { };
            
            foreach(string d in Directory.EnumerateDirectories(ContactsDirectory).ToList<string>())
            {
                // The path includes some other stuff at the beginning, and we just want to grab
                // the last folder name, which is named after the contact
                cleanDirs.Add(d.ToString().Split(',').Reverse().Last());
            }

            return cleanDirs;
        }

        public void AddContact(string contact)
        {
            var path = Path.Combine(ContactsDirectory, contact);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.Combine(ContactsDirectory, contact));
            }
        }
    }
}