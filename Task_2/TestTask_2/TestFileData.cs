using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.AccessControl;

using Task_2;

namespace TestTask_2
{
    [TestClass]
    public class TestFileData
    {        
        /// <summary>
        /// Run IDE as Administrator (Unauthorized Access Issue)
        /// </summary>
        [TestMethod]
        public void GetFileAttribute_FileIsHidden_ReturnsTrue()
        {
            //Arrange
            string filePath = DriveManager.LogicalDrive + "hidden.txt";
            using (var file = File.Create(filePath)) { };

            //Make file hidden
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);
            
            //Gather file data
            FileInfo fileInfo = new FileInfo(filePath);
            FileAttributes fileAttributes = File.GetAttributes(filePath);

            File.Delete(filePath);

            FileData fd = new FileData(fileInfo, fileAttributes);
            bool expected = true;

            //Act
            bool actual = fd.GetFileAttribute(System.IO.FileAttributes.Hidden);

            //Assert
            Assert.AreEqual(expected, actual);

        }
                
    }
}
