using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MagentoScanner.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Given a folder name return all parents according to level
        /// </summary>
        /// <param name="FolderName">Sub-folder name</param>
        /// <param name="level">Level to move up the folder chain</param>
        /// <returns>List of folders dependent on level parameter</returns>
        public static string UpperFolder(this string FolderName, Int32 level)
        {
            List<string> folderList = new List<string>();

            while (!string.IsNullOrEmpty(FolderName))
            {
                var temp = Directory.GetParent(FolderName);
                if (temp == null)
                {
                    break;
                }
                FolderName = Directory.GetParent(FolderName).FullName;
                folderList.Add(FolderName);
            }

            if (folderList.Count > 0 && level > 0)
            {
                if (level - 1 <= folderList.Count - 1)
                {
                    return folderList[level - 1];
                }
                else
                {
                    return FolderName;
                }
            }
            else
            {
                return FolderName;
            }
        }
        public static string CurrentProjectFolder(this string sender)
        {
            return sender.UpperFolder(3);
        }
    }
}
