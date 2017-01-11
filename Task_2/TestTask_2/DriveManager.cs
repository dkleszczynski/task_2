using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestTask_2
{
    public static class DriveManager
    {
        private static string logicalDrive = null;
        public static string LogicalDrive
        {
            get
            {
                if (logicalDrive == null)
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();

                    foreach (DriveInfo drive in drives)
                    {
                        if (drive.IsReady)
                        {
                            logicalDrive = drive.Name[0].ToString();
                            break;
                        }
                    }
                }

                return logicalDrive;
            }

        }
    }
}
