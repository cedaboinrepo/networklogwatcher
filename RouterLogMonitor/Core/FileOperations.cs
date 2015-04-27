using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterLogMonitor
{
    internal static class FileOperations
    {
        public static void CreateFile(string folder, string file)
        {
            // Specify a name for your top-level folder. 
            string folderName = folder;

            System.IO.Directory.CreateDirectory(folderName);

            // Create a file name for the file you want to create.  
            string fileName = file + ".txt";

            System.IO.Path.Combine(folderName, fileName);
        }

        public static void CreateFile(string path, string folder, string subfolder, string file)
        {
            // Specify a name for your top-level folder. 
            string folderName = folder;

            // To create a string that specifies the path to a subfolder under your  
            // top-level folder, add a name for the subfolder to folderName. 
            string pathString = System.IO.Path.Combine(folderName, subfolder);

            // You can extend the depth of your path if you want to. 
            //pathString = System.IO.Path.Combine(pathString, "SubSubFolder");

            // Create the subfolder. You can verify in File Explorer that you have this 
            // structure in the C: drive. 
            //    Local Disk (C:) 
            //        Top-Level Folder 
            //            SubFolder
            System.IO.Directory.CreateDirectory(pathString);

            // Create a file name for the file you want to create.  
            string fileName = file + ".txt";

            // This example uses a random string for the name, but you also can specify 
            // a particular name. 
            //string fileName = "MyNewFile.txt";

            // Use Combine again to add the file name to the path.
            pathString = System.IO.Path.Combine(pathString, fileName);
        }

        public static void WriteToFile(string filePath, string line)
        {
            using (System.IO.StreamWriter file = File.AppendText(filePath))
            {
                file.WriteLine(line);
                file.Flush();
            }
        }

        public static bool FileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
