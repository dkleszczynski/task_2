using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.AccessControl;


namespace Task_2
{
    /// <summary>
    /// Model representation of file metadata.
    /// </summary>
    public class FileData : IFileData
    {
        public FileInfo FileInfo { get; private set; }
             
        /// <summary>
        /// AccessControlSections.All: Entire Security Descriptor
        /// </summary>
        public AuthorizationRuleCollection AccessRules { get; private set; }
        
        private FileAttributes fileAttributes;


        public FileData(FileInfo fileInfo, FileAttributes fileAttributes)
        {
            FileInfo = fileInfo;
            this.fileAttributes = fileAttributes;
            AccessRules = null;
         }

        public FileData(FileInfo fileInfo, FileAttributes fileAttributes, AuthorizationRuleCollection accessRules)
        {
            FileInfo = fileInfo;
            this.fileAttributes = fileAttributes;
            AccessRules = accessRules;
        }

        /// <summary>
        /// Returns boolean value of bitwise FileAttribute.
        /// </summary>
        /// <param name="fileAttribute">Choose attribute from FileAttributes enum.</param>
        /// <returns></returns>
        public bool GetFileAttribute(FileAttributes fileAttribute)
        {
            if ((fileAttributes & fileAttribute) == fileAttribute)
                return true;
            else
                return false;


        }
             
    }
}

