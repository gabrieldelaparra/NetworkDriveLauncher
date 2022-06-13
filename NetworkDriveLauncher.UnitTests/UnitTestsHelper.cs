using Wororo.Utilities;

namespace NetworkDriveLauncher.UnitTests
{
    public static class UnitTestsHelper
    {
        public static void CreateDirectories(string rootPath, int dirsPerLevel, int depth)
        {
            rootPath.CreatePathIfNotExists();
            CreateDirectoriesRecursive(rootPath, dirsPerLevel, depth, 0);
        }

        private static void CreateDirectoriesRecursive(string path, int dirsPerLevel, int depth, int current)
        {
            if (depth == current)
                return;

            for (var j = 0; j < dirsPerLevel; j++)
            {
                var sampleDirectory = $"{Path.Combine(path, j.ToString())}\\";
                sampleDirectory.CreatePathIfNotExists();

                for (var i = current; i < depth; i++)
                {
                    CreateDirectoriesRecursive(sampleDirectory, dirsPerLevel, depth, current + 1);
                }
            }
        }
    }
}
