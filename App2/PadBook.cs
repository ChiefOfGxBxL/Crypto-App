using System.IO;

namespace App2
{
    public class Padbook
    {
        public string filePath { get; private set; }
        public string Username { get; private set; }

        private StreamReader sr;
        private StreamWriter sw;

        public Padbook(string path, string username ="")
        {
            filePath = path;
            Username = username;

            sr = new StreamReader(path);
        }

        /// <summary>
        /// Retrieves the Nth pad in the pad file.
        /// Note that n should start at 1 for the 1st pad.
        /// </summary>
        /// <param name="n">Index of pad to retrieve, starting at n=1 for 1st pad</param>
        /// <returns></returns>
        public string GetNthPad(int n)
        {
            if (n == 0) return null; // must index starting at 1
            
            // eat up the lines until we get to the one we want
            // TODO: verify there is not an off-by-one error here
            for(int i = 0; i < n-1; i++)
            {
                sr.ReadLine();
            }

            return sr.ReadLine();
        }

        public void AppendPads(string[] pads)
        {
            string padFileLoc = Path.Combine(filePath, Username, "pad.txt");
            StreamWriter sw = new StreamWriter(new FileStream(padFileLoc, FileMode.Append));
            for (int i =0; i < pads.Length; i++)
            {
                sw.Write(pads[i] + "\n");
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