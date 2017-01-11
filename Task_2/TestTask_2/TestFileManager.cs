using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task_2;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace TestTask_2
{
    [TestClass]
    public class TestFileManager
    {
        private Random random = new Random();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFiles_PathIsNull_ThrowsException()
        {
            //Arrange
            FileManager fm = new FileManager();

            //Act
            fm.GetFiles(null);
            
            //Asserted by ExpectedException Atribute Class
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectPathSyntaxException))]
        public void GetFiles_IncorrectPathSyntax_ReturnsException()
        {
            //Arrange
            FileManager fm = new FileManager();
            string directoryPath = "";
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            
            if (isWindows)
                directoryPath = "/home";
            else
                directoryPath = "c:\\";

            //Act
            fm.GetFiles(directoryPath);

        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void GetFiles_DirectoryDoesNotExist_ThrowsException()
        {
            //Arrange
            FileManager fm = new FileManager();
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string fakePath = GetFakeDirectoryPath(isWindows);

            //Act
            fm.GetFiles(fakePath);

        }
                
        [TestMethod]
        public void GetFiles_AccesibleDirectory_ReturnsFileDataList()
        {
            //Arrange
            FileManager fm = new FileManager();
            string directoryPath = @"c:\";

            //Act
            List<FileData> fileData = fm.GetFiles(directoryPath);

            //Assert
            CollectionAssert.AllItemsAreInstancesOfType(fileData, typeof(FileData));
                        
        }

        /// <summary>
        /// Test performed in Windows OS.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void GetFiles_UnaccesibleWindowsDirectory_ThrowsException()
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows == false)
                throw new UnauthorizedAccessException();

            //Arrange
            DirectoryInfo dirInfo = Directory.CreateDirectory("c:\\testDir");
            DirectorySecurity dirSecurity = new DirectorySecurity(dirInfo.FullName, AccessControlSections.All);

            var securityId = System.Security.Principal.WindowsIdentity.GetCurrent().User;

            FileSystemAccessRule rule = new FileSystemAccessRule(securityId, FileSystemRights.ListDirectory, AccessControlType.Deny);
            dirSecurity.AddAccessRule(rule);
            dirInfo.SetAccessControl(dirSecurity);

            FileManager fm = new FileManager();

            //Act
            fm.GetFiles(dirInfo.FullName);
            
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckPathSyntax_PathIsNull_ThrowsException(bool isWindows)
        {
            //Arrange
            FileManager fm = new FileManager();
            
            //Act
            fm.IsPathSyntaxCorrect(null, isWindows);
        }

        [TestMethod]
        [DataRow(true, ":\\")]
        [DataRow(true, "/home")]
        [DataRow(false, "c:\\")]
        public void CheckPathSyntax_IncorrectSyntax_ReturnsFalse(bool isWindows, string directoryPath)
        {
            //Arrange
            FileManager fm = new FileManager();
            bool expected = false;

            //Act
            bool actual = fm.IsPathSyntaxCorrect(directoryPath, isWindows);
            
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow(true, "c:\\")]
        [DataRow(false, "/home")]
        public void CheckPathSyntax_CorrectSyntax_ReturnsTrue(bool isWindows, string directoryPath)
        {
            //Arrange
            FileManager fm = new FileManager();
            bool expected = true;

            //Act
            bool actual = fm.IsPathSyntaxCorrect(directoryPath, isWindows);
           
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFile_PathIsNull_ThrowsException()
        {
            //Arrange
            FileManager fm = new FileManager();

            //Act
            fm.GetFile(null);
        }

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void GetFile_DirectoryPathNotExists_ThrowsException()
        {
            //Arrange
            FileManager fm = new FileManager();
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string fakePath = GetFakeDirectoryPath(isWindows);

            if (isWindows)
                fakePath +=  "\\abc.xml";
            else
                fakePath += "/abc.xml";

            //Act
            fm.GetFile(fakePath);

        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetFile_FileNotExistsInDirectory_ThrowsException()
        {
            //Arrange
            FileManager fm = new FileManager();
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string filePath = "";

            if (isWindows)
                filePath = GetFakeFilePath("c:\\", true);
            else
                filePath = GetFakeFilePath("/home", false);

            //Act
            fm.GetFile(filePath);
            
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [DataRow("/ho;:me")]
        [DataRow("c:\\tes:t.xml")]
        public void GetFile_WindowsIncorrectFilePathSyntax_ThrowsException(string filePath)
        {
            //Arrange
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            //Test is only for Windows
            if (isWindows == false)
                throw new NotSupportedException();
                       
            FileManager fm = new FileManager();

             //Act
             fm.GetFile(filePath);
            
        }
        
        //Returns file path which is not associated with any file.                      
        private string GetFakeFilePath(string directoryPath, bool isWindows)
        {
            string filePath = directoryPath;

            if (isWindows)
                filePath += "\\a";
            else
                filePath += "/a";

            while(File.Exists(filePath + ".xml") == true)
            {
                filePath += (char)random.Next(97, 122);
            }

            return filePath + ".xml";
        }

        //Returns directory path which is not associated with any actual directory.
        private string GetFakeDirectoryPath(bool isWindows)
        {
            string path = "";

            if (isWindows)
                path = "c:\\ftp";
            else
                path = "/home/ttt";

            while (Directory.Exists(path) == true)
            {
                path = path + (char)random.Next(97, 122);
            }

            return path;
        }










    }
}
