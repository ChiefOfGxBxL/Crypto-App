using System.Collections.Generic;
using System.IO;

namespace App2
{
    public class Padbook
    {
        public string padFilePath { get; private set; }
        public string Username { get; private set; }

        private StreamReader sr;

        List<string> pads;

        public Padbook(string path, string username)
        {
            padFilePath = Path.Combine(path, Username, "pad.txt");
            Username = username;

            pads = new List<string>();

            // Read all pads into memory
            sr = new StreamReader(padFilePath);
            while (sr.Peek() > 0)
            {
                pads.Add(sr.ReadLine());
            }
        }

        /// <summary>
        /// Retrieves the Nth pad in the pad file.
        /// Note that n should start at 0 for the 1st pad.
        /// </summary>
        /// <param name="n">Index of pad to retrieve, starting at n=0 for 1st pad</param>
        /// <returns></returns>
        public string GetNthPad(int n)
        {
            return pads[n];
        }

        public string GetNextPad()
        {
            if (pads.Count == 0) return "";

            string nextPad = pads[0];
            pads.RemoveAt(0); // delete the first pad from the list, since we can only use it once
            SaveMemPadsToFile(); // save the pad file - we removed the next pad

            return nextPad;
        }

        public StreamWriter OpenPadStreamWriter()
        {
            return new StreamWriter(new FileStream(padFilePath, FileMode.Append));
        }

        private void SaveMemPadsToFile()
        {
            var sw = OpenPadStreamWriter();
            foreach (string s in pads)
            {
                sw.WriteLine(s);
            }
            sw.Close();
        }

        public void AppendPads(string[] pads)
        {
            var sw = OpenPadStreamWriter();
            for (int i = 0; i < pads.Length; i++)
            {
                sw.WriteLine(pads[i]);
            }
            sw.Close();
        }

        public void DestroyPad()
        {
            // TODO: destroys all the pads in this file
            // this may be for security reasons, initiated by a user request, etc.
            // it destroys the pad.txt file as well
        }
    }
}