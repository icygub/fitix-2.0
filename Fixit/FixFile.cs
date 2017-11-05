using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Fixit
{
    class FixFile
    {
        public string MyPath { get; set; }
        public string Extension { get; set; }
        public string Prefix { get; set; }
        public List<String> _OldNames = new List<string>();
        public List<String> _NewNames = new List<string>();
        // Note: fix to case to conform with .NET naming conventions
        public IList<string> OldNames { get { return _OldNames; } }
        public IList<string> NewNames { get { return _NewNames; } }

        public FixFile()
        {
            
        }
        public FixFile(string[] filesList, string myPath)
        {
            //MyFiles = new List<Files>();
            foreach (string file in filesList)
            {
                //MyFiles.Add(new Files(JustLast(Path.GetFileName(file)), JustLast(Path.GetFileName(file))));
                OldNames.Add(JustLast(file));
                NewNames.Add(JustLast(file));
            }
            Prefix = GetPrefix(filesList.Last());
            Extension = GetExtension(filesList.Last());
            MyPath = myPath;

        }
        public void ResetNew(FixFile myFile)
        {
            for (int i = 0; i < myFile.NewNames.Count; i++)
            {
                myFile.NewNames[i] = myFile.OldNames[i];
            }

        }

        static string JustLast(string myString)
        {
            try
            {
                int idx = myString.LastIndexOf('_');
                myString = myString.Substring(idx + 1);
                
                return myString.Split('.')[0];
            }
            catch
            {
                return myString;
            }
        }

        static string GetPrefix(string myString)
        {
            try
            {
                int idx = myString.LastIndexOf('_');
                myString = myString.Substring(0, idx);
                idx = myString.LastIndexOf('\\');

                return myString.Substring(idx + 1);

            }
            catch
            {
                return myString;
            }
        }
        static string GetExtension(string myString)
        {
            try
            {
                int idx = myString.LastIndexOf('.');
                return myString.Substring(idx);
            }
            catch
            {
                return myString;
            }
        }

    }

    public class Files
    {
        public string OldName;
        public string NewName;

        public Files(string oldfile, string newfile)
        {
            this.OldName = oldfile;
            this.NewName = newfile;
        }
    }
}
