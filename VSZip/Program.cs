﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSZip
{
    class Program
    {
        static List<string> deletedPaths = new List<string>();

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
            usage:
                Console.WriteLine("USAGE: ");
                Console.WriteLine("vszip [Folder Name]");
                return;
            }

            try
            {
                deletedPaths.Clear();
                CleanDirectory(args[0]);

                Console.WriteLine("DELETED FOLDERS ({0}):", deletedPaths.Count);
                deletedPaths.ForEach(x => Console.WriteLine(x));

                Console.WriteLine("CREATING A ZIP FILE: " + args[0] + ".zip");
                ZipFile.CreateFromDirectory(args[0], args[0] + ".zip", CompressionLevel.Optimal, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void CleanDirectory(string path)
        {
            var di = new DirectoryInfo(path);
            bool isProjectDirectory = IsProjectDirectory(path);
            bool isSolutionDirectory = IsSolutionDirectory(path);
            string folderType = isProjectDirectory ? "Project Folder" : (isSolutionDirectory ? "Solution Folder" : "");
            Console.WriteLine("{1,15} {0}", path, folderType);

            if (isSolutionDirectory)
            {
                var vsFolder = di.GetDirectories().FirstOrDefault(x => x.Name == ".vs");
                DeleteFolder(vsFolder);
                var packagesFolder = di.GetDirectories().FirstOrDefault(x => x.Name == "packages");
                DeleteFolder(packagesFolder);
            }

            if (isProjectDirectory)
            {
                var binFolder = di.GetDirectories().FirstOrDefault(x => x.Name == "bin");
                DeleteFolder(binFolder);
                var objFolder = di.GetDirectories().FirstOrDefault(x => x.Name == "obj");
                DeleteFolder(objFolder);
            }

            foreach (var subDir in di.GetDirectories())
            {
                CleanDirectory(subDir.FullName);
            }
        }

        private static void DeleteFolder(DirectoryInfo directoryInfo)
        {
            if (directoryInfo != null)
            {
                deletedPaths.Add(directoryInfo.FullName);
                directoryInfo.Delete(true);
            }
        }

        static bool IsProjectDirectory(string path)
        {
            var di = new DirectoryInfo(path);
            return di.GetFiles().Any(x => x.Extension == ".csproj");
        }

        static bool IsSolutionDirectory(string path)
        {
            var di = new DirectoryInfo(path);
            return di.GetFiles().Any(x => x.Extension == ".sln");
        }
    }
}
