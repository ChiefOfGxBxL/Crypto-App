using System.IO;

namespace App2
{
    public class Pad
    {
        public string Path { get; private set; }
        public string Username { get; private set; }

        private StreamReader sr;

        public Pad(string path, string username = "")
        {
            Path = path;
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
            // TODO: appends more pads to this current padfile. This is 
            // not a destructive action -- it does not overwrite the current
            // pads that are currently in the file.
        }

        public void DestroyPad()
        {
            // TODO: destroys all the pads in this file
            // this may be for security reasons, initiated by a user request, etc.
            // it destroys the pad.txt file as well
        }
    }
}