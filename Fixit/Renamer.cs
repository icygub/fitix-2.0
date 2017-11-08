using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixit
{
    class Renamer
    {
        public static void RenameFile(FixFile myFile)
        {
            if (myFile.NewTN == "")
            {
                myFile.NewTN = myFile.TN;
            }
            System.Configuration.Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string auditPath = config.AppSettings.Settings["AuditPath"].Value;
            string oldFileName;
            string newFileName;



            foreach (var FileName in myFile.MyFiles)
            {
                oldFileName = myFile.TN + "_" + FileName.OldName + myFile.Extension;
                newFileName = myFile.NewTN + "_" + FileName.NewName + myFile.Extension;
                try
                {
                    File.Delete(myFile.NewPath + "\\" + newFileName);
                }
                catch { }
                File.Copy(myFile.OldPath + "\\" + oldFileName, myFile.NewPath + "\\" + newFileName);
                File.Delete(myFile.OldPath + "\\" + oldFileName);
            }

        }
    }
}
