using System;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace DailyUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dirA = new DirectoryInfo(args[1]);
            DirectoryInfo dirB = new DirectoryInfo(args[0]);

            ClearDirectory(dirA);
            CopyDirectoryBIntoA(dirB, dirA);

            UpdateSnapshot(args[1]);
        }

        public static void ClearDirectory(DirectoryInfo dirA)
        {
            foreach (FileInfo file in dirA.GetFiles())
            {
                Console.WriteLine("Deleting " + file);
                file.Delete();
            }

            foreach (DirectoryInfo dir in dirA.GetDirectories())
            {
                Console.WriteLine("Deleting " + dir);
                dir.Delete(true);
            }
        }
        public static void CopyDirectoryBIntoA(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo file in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, file.Name);
                file.CopyTo(Path.Combine(target.ToString(), file.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectoryBIntoA(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void UpdateSnapshot(string dirA)
        {
            //Load the xml from file
            var config = XElement.Load(dirA + "/web.config");

            //Pattern to find what needs replacing in ConnectionString
            Regex catalogPattern = new Regex(@"Initial Catalog=\w+;");

            //Replacement string with updated datetime
            var updatedInitialCatalog = "Initial Catalog=Mach1CloudSnapshot_" + DateTime.Today.AddDays(-1).ToString("yyyyMMdd") + ";";

            //Replaces initial catalog with updated initial catalog
            var updatedConfigString = catalogPattern.Replace(config.ToString(), updatedInitialCatalog);

            //Parses updated string of config file into xml
            XElement updatedConfigXml = XElement.Parse(updatedConfigString);

            //Replaces old xml with new xml
            config.ReplaceAll(updatedConfigXml);

            //Saves updated xml to file
            config.Save(dirA + "/web.config");
        }
    }


}
