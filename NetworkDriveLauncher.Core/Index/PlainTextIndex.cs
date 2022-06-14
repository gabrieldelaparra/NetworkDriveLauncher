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

            foreach (var directory in indexLines)
            {
                var split = directory.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                if (!split.Any())
                    continue;

                var depthOnly = split.Reverse().Take(Configuration.Depth + 1).Reverse().ToArray();

                var name = split.LastOrDefault();
                var separated = depthOnly.SelectMany(x => x.Split(separators, StringSplitOptions.RemoveEmptyEntries)).ToArray();
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
                        Title = depthOnly.Join("\\"),
                        SubTitle = name,
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