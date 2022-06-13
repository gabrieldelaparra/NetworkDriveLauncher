using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wororo.Utilities;

namespace NetworkDriveLauncher.Core
{
    public class PlainTextIndexBuilder : IIndexBuilder<PlainTextIndexConfiguration>
    {
        public PlainTextIndexConfiguration Configuration { get; }
        public PlainTextIndexBuilder(PlainTextIndexConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void BuildIndex()
        {
            var indexPath = Configuration.OutputFilename;

            if (Configuration.OverwriteIndex)
                indexPath.DeleteIfExists();

            var names = GetDirectories();

            indexPath.CreatePathIfNotExists();
            File.WriteAllLines(indexPath, names);
        }

        public IEnumerable<string> GetDirectories()
        {
            var rootDirectories = Configuration.RootDirectories.Select(x => new DirectoryInfo(x));

            var innerDirectories = rootDirectories.SelectMany(x => GetLevelDirectories(x.FullName, Configuration.Depth));

            return innerDirectories.Select(x => x.FullName);
        }

        internal static IEnumerable<DirectoryInfo> GetLevelDirectories(string path, int depth, int current = 0)
        {
            var directoryInfo = new DirectoryInfo(path);
            var levelDirectories = directoryInfo.GetDirectories();

            if (current >= depth)
                yield break;

            foreach (var item in levelDirectories)
            {
                yield return item;
                    
                var directoryInfos = GetLevelDirectories(item.FullName, depth, current + 1);
                foreach (var directories in directoryInfos)
                {
                    yield return directories;
                }
            }
        }


    }
}