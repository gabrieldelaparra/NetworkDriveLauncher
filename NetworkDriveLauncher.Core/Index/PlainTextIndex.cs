using NetworkDriveLauncher.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wororo.Utilities;

namespace NetworkDriveLauncher.Core.Index
{
    public class PlainTextIndex : IIndex<PlainTextIndexConfiguration>
    {
        public PlainTextIndexConfiguration Configuration { get; }
        public PlainTextIndex(PlainTextIndexConfiguration configuration)
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

        public IEnumerable<QueryResult> Query(IEnumerable<string> queryTerms)
        {
            var separators = new[] { '-', '_', '\\', ' ', '/' };

            if (!queryTerms.Any())
                yield break;

            var indexLines = Configuration.OutputFilename.ReadLines().ToArray();

            var rootDirectories = Configuration.RootDirectories.Select(x => new DirectoryInfo(x).FullName).ToArray();

            foreach (var directory in indexLines)
            {
                var depthOnly = directory;
                foreach (var rootDirectory in rootDirectories)
                {
                    //TODO: Maybe get Directory.FullName for the replace.
                    //On the Network Drive is not required, since the full path is given,
                    //but on a relative path it is required.
                    depthOnly = depthOnly.Replace(rootDirectory, "");
                }

                var split = depthOnly.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                if (!split.Any())
                    continue;
                var folderName = split.LastOrDefault();

                //var depthOnly = split.Reverse().Take(Configuration.Depth + 1).Reverse().ToArray();
                var separated = split.SelectMany(x => x.Split(separators, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                var successCount = 0;
                foreach (var word in queryTerms)
                {
                    successCount += separated.Count(x => x.Contains(word, StringComparison.OrdinalIgnoreCase));
                }
                if (successCount > 0)
                {
                    yield return new QueryResult
                    {
                        FullName = directory,
                        Title = depthOnly,
                        SubTitle = folderName,
                        Score = successCount - split.Length,
                    };
                }
            }
        }

        public IEnumerable<string> GetDirectories()
        {
            var rootDirectories = Configuration.RootDirectories.Select(x => new DirectoryInfo(x));

            var innerDirectories = rootDirectories.SelectMany(x => GetLevelDirectories(x.FullName, Configuration.Depth));

            return innerDirectories.Select(x => x.FullName);
        }

        private static IEnumerable<DirectoryInfo> GetLevelDirectories(string path, int depth, int current = 0)
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