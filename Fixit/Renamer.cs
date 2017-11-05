using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixit
{
    class Renamer
    {

        public static void RenameFile(string OldDirectory, string NewDirectory, List<FixFile> Files)
        {
            foreach (var FileName in Files)
            {
                /*var SplitName = FileName.RealName.Split('_');
                string myFilename = "";
                for (int i = 0; i < SplitName.Length - 1; i++)
                {
                    myFilename += SplitName[i];

                    myFilename += "_";
                }
                myFilename += FileName.NewName;
                SplitName = FileName.RealName.Split('.');
                if (SplitName.Length > 0)
                {
                    myFilename += "." + SplitName[1];
                }
                if (File.Exists(NewDirectory + "\\" + myFilename))
                {
                    File.Delete(NewDirectory + "\\" + myFilename);
                }
                File.Copy(OldDirectory + "\\" + FileName.RealName, NewDirectory + "\\" + myFilename);
                File.Delete(OldDirectory + "\\" + FileName.RealName);*/
            }

        }
    }
}
