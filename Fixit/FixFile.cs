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
       // public List<String> _OldNames = new List<string>();
       // public List<String> _NewNames = new List<string>();
        public List<Files> MyFiles = new List<Files>();

        // Note: fix to case to conform with .NET naming conventions
        //public List<string> OldNames { get { return _OldNames; } }
        //public List<string> NewNames { get { return _NewNames; } }

        public FixFile()
        {
            
        }
        public FixFile(string[] filesList, string myPath)
        {
            //MyFiles = new List<Files>();
            foreach (string file in filesList)
            {
                MyFiles.Add(new Files(JustLast(file), JustLast(file)));
                //OldNames.Add(JustLast(file));
                //NewNames.Add(JustLast(file));
            }
            Prefix = GetPrefix(filesList.Last());
            Extension = GetExtension(filesList.Last());
            MyPath = myPath;

        }
        public static void ResetNew(FixFile myFile)
        {
            for (int i = 0; i < myFile.MyFiles.Count; i++)
            {
                //myFile.NewNames[i] = myFile.OldNames[i];
                myFile.MyFiles[i].NewName = myFile.MyFiles[i].OldName;
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
        private string _oldName;
        private string _newName;

        public string OldName
        {
            get { return _oldName; }
            set { _oldName = value; }
        }

        public string NewName
        {
            get { return _newName; }
            set { _newName = value; }
        }

        public Files(string oldfile, string newfile)
        {
            this.OldName = oldfile;
            this.NewName = newfile;
        }
    }
}
