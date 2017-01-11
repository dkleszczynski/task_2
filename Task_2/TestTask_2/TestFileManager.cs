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
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            FileManager fm = new FileManager();
            string directoryPath = "";

            if (isWindows)
                directoryPath = DriveManager.LogicalDrive + @":\";
            else
                directoryPath += "/home";

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
                Assert.Inconclusive("Cannot perform this test for UNIX type OS.");

            //Arrange
            DirectoryInfo dirInfo = Directory.CreateDirectory(DriveManager.LogicalDrive + ":\\testDir");
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
                filePath = GetFakeFilePath(DriveManager.LogicalDrive + ":\\", true);
            else
                filePath = GetFakeFilePath("/home", false);

            //Act
            fm.GetFile(filePath);
            
        }
        
        [TestMethod]
        [DataRow("home/", false)]
        [DataRow(@"/home/ab\0c.xml", false)]
        [DataRow(@"/home/ab/", false)]
        [DataRow(":\\tes:t.xml", true)]
        [DataRow(":\\test>.xml", true)]
        [DataRow(":\\</test.xml", true)]
        public void GetFile_IncorrectFilePathSyntax_ThrowsException(string filePath, bool isWindows)
        {
            //DataRow is not for detected OS ==> skip the test
            if (isWindows != System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Inconclusive("Cannot test because of OS type.");

            //Arrange
            FileManager fm = new FileManager();

            //Add logical drive name
            if (isWindows)
                filePath = filePath.Insert(0, DriveManager.LogicalDrive);
           
            //Act and Assert
            try
            {
                fm.GetFile(filePath);
                Assert.Fail();
            }
            catch(NotSupportedException nsEx)
            {
                Assert.IsInstanceOfType(nsEx, typeof(NotSupportedException));
            }
            catch(ArgumentException aEx)
            {
                Assert.IsInstanceOfType(aEx, typeof(ArgumentException));
            }
            
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
                path = "z:\\ftp";
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
