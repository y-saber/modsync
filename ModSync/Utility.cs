using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aki.Common.Utils;

namespace ModSync
{
    public static class Utility
    {
        public static List<string> GetFilesInDir(string dir)
        {
            var files = new List<string>();

            foreach (var directory in VFS.GetDirectories(dir))
            {
                foreach (var file in GetFilesInDir(directory))
                    files.Add(file);
            }

            foreach (var file in VFS.GetFiles(dir))
                files.Add(file);

            return files;
        }

        public static bool NoSyncInTree(string baseDir, string dir)
        {
            var noSyncPath = Path.Combine(dir, ".nosync");
            if (VFS.Exists(noSyncPath) || VFS.Exists($"{noSyncPath}.txt"))
                return true;
            else if (dir == baseDir || dir == string.Empty)
                return false;

            return NoSyncInTree(baseDir, Path.GetDirectoryName(dir));
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (File.Exists(tempDirectory))
                return GetTemporaryDirectory();
            else
            {
                Directory.CreateDirectory(tempDirectory);
                return tempDirectory;
            }
        }

        public static void CopyFilesRecursively(string source, string target) => CopyFilesRecursively(new DirectoryInfo(source), new DirectoryInfo(target));

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

        public static async Task WriteFileAsync(string filepath, byte[] data)
        {
            var fileExists = VFS.Exists(filepath);
            if (!fileExists)
            {
                VFS.CreateDirectory(filepath.GetDirectory());
            }

            using FileStream stream = File.Open(filepath, fileExists ? FileMode.Truncate : FileMode.CreateNew, FileAccess.Write);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
