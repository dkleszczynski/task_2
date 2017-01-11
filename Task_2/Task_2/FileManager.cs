using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using System.Collections.Generic;

namespace Task_2
{
    /// <summary>
    /// Contains methods connected with managing files.
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// Exception messages for single files causing exception
        /// </summary>
        private List<string> errorLog = new List<string>();

        /// <summary>
        /// Returns list of objects describing metadata of files in a directory. 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public List<FileData> GetFiles(string directoryPath)
        {
            if (directoryPath == null)
                throw new ArgumentNullException("Directory path is null");

            //Remove white-space characters from the start and end of path.
            directoryPath = directoryPath.Trim();

            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (IsPathSyntaxCorrect(directoryPath, isWindows) == false)
            {
                if (isWindows)
                    throw new IncorrectPathSyntaxException("Incorrect path syntax in Windows OS.");
                else
                    throw new IncorrectPathSyntaxException("Incorrect path syntax in Unix type OS.");
            }

            directoryPath = RemoveSpacesFromFilePath(directoryPath, isWindows);

            if (Directory.Exists(directoryPath) == false)
                throw new DirectoryNotFoundException("Directory " + directoryPath + " does not exist.");
            
            string[] filePaths = Directory.GetFiles(directoryPath);
            List<FileData> fileData = new List<FileData>();

            foreach (string filePath in filePaths)
            {
                try
                {
                    fileData.Add(GetFile(filePath));
                }
                catch (Exception ex)
                {
                    AddToErrorLog(filePath, ex);
                } 
            }
            
            if (errorLog.Count > 0)
            {
                FileStream fs = null;

                if (isWindows)
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    string logicalDrive = "";

                    foreach (DriveInfo drive in drives)
                    {
                        if (drive.IsReady)
                        {
                            logicalDrive = drive.Name[0].ToString();
                            break;
                        }
                    }
                    fs = File.Create(logicalDrive + ":\\errorLog.txt");
                }
                else
                    fs = File.Create("/home/errorLog.txt");

                StreamWriter sw = new StreamWriter(fs);

                foreach (string logLine in errorLog)
                    sw.WriteLine(logLine);

                sw.Dispose();
            }

            return fileData;
        }

        /// <summary>
        /// Returns object describing file metadata for certain filepath.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public FileData GetFile(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("File path is null.");

            FileInfo fileInfo = new FileInfo(filePath);
            FileAttributes fileAttributes = File.GetAttributes(filePath);
            AuthorizationRuleCollection accessRules = null;

            try
            {
                FileSecurity fs = new FileSecurity(fileInfo.FullName, AccessControlSections.All);
                accessRules = fs.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            }
            catch (Exception ex)
            {
                return new FileData(fileInfo, fileAttributes); 
            }

            
            return new FileData(fileInfo, fileAttributes, accessRules);
            
        }
        
        /// <summary>
        /// Checks with regular expression whether path has correct syntax in defined OS. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isWindows"></param>
        /// <returns></returns>
        public bool IsPathSyntaxCorrect(string path, bool isWindows)
        {
            if (isWindows)
            {
                if (Regex.IsMatch(path, @"(^[a-zA-Z]\:\\$|^[a-zA-Z]\:\\[^/:*?<>""|]*[^\\/:*?<>""|]$)") == false)
                    return false;
            }
            //Unix type OS
            else
            {
                if (Regex.IsMatch(path, @"^\/[^\0]*[^\0/]$|^\/$") == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Adds information about exception to the errorLog list.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ex"></param>
        private void AddToErrorLog(string filePath, Exception ex)
        {
            errorLog.Add(filePath);
            errorLog.Add(" ");
            errorLog.Add(ex.Message);
            errorLog.Add(ex.StackTrace);
        }

        /// <summary>
        /// Removes white-space characters from the start and end of path segments.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isWindows"></param>
        /// <returns></returns>
        private string RemoveSpacesFromFilePath(string filePath, bool isWindows)
        {
            string trimmedPath = "";
            string[] pathSegments;

            if (isWindows)
            {
                pathSegments = filePath.Split(new char[] { '\\', }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 1; i < pathSegments.Length; i++)
                    pathSegments[i] = pathSegments[i].Trim();

                foreach (string segment in pathSegments)
                    trimmedPath += segment + "\\";

                if (pathSegments.Length > 1)
                    return trimmedPath.TrimEnd(new char[] { '\\' });
                else
                    return trimmedPath;
            }
            else
            {
                pathSegments = filePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < pathSegments.Length; i++)
                    pathSegments[i] = pathSegments[i].Trim();

                foreach (string segment in pathSegments)
                    trimmedPath += "/" + segment;

                return trimmedPath;

            }
        }
        
    }
}
