using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Fixit
{
    class FixFile
    {
        private string _OldPath { get; set; }
        private string _NewPath { get; set; }
        private string _Extension { get; set; }
        private string _TN { get; set; }
        private string _NewTN { get; set; }
        public List<Files> MyFiles = new List<Files>();

        public string TN
        {
            get { return _TN; }
            set { _TN = value; }
        }

        public string Extension
        {
            get { return _Extension; }
            set { _Extension = value; }
        }

        public string NewPath
        {
            get { return _NewPath; }
            set { _NewPath = value; }
        }

        public string OldPath
        {
            get { return _OldPath; }
            set { _OldPath = value; }
        }
        public string NewTN
        {
            get { return _NewTN; }
            set { _NewTN = value; }
        }


        //This is the tester constructor.
        public FixFile(string oldPath, string newPath, string extension, string TN, string NewTN, string file1, string file2)
        {
            OldPath = oldPath;
            NewPath = newPath;
            Extension = extension;
            TN = TN;
            NewTN = NewTN;
            MyFiles.Add(new Files(file1, file1));
            MyFiles.Add(new Files(file2, file2));
        }

        //No Args constructor so we can instanciate the object in the beginning of the Main Window application.
        public FixFile()
        {
            NewTN = "";

        }
        //This constructor is actually the one with all the algorythms to actually create the object in the main window.
        public FixFile(string[] filesList, string oldPath, string newPath)
        {
            Extension = "";
            foreach (string file in filesList)
            {
                if (Extension == "" | Extension == GetExtension(file))
                {
                    MyFiles.Add(new Files(JustLast(file), JustLast(file)));
                    Extension = GetExtension(file);
                    TN = GetPrefix(file);
                }
                
            }
            NewTN = TN;
            OldPath = oldPath;
            NewPath = newPath;

        }

        public void ResetNew(FixFile myFile)
        {
            for (int i = 0; i < myFile.MyFiles.Count; i++)
            {
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
            set
            {
                _newName = value;
                int myCount = 0;
                foreach (Match m in Regex.Matches(_newName, @"[a-zA-Z]")) {
                    myCount += 1;
                }
                myCount += OldName.Length;
                if (NewName.Length < myCount) {
                    for (int i = 0; NewName.Length < myCount; i++) {
                        NewName = "0" + NewName;
                    }
                }
            }
        }

        public Files(string oldfile, string newfile)
        {
            this.OldName = oldfile;
            this.NewName = newfile;
        }
    }
}
