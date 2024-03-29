﻿using NetworkDriveLauncher.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NetworkDriveLauncher.Core.Utilities;
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

        private IEnumerable<string> PrepareForBuild()
        {
            var indexPath = Configuration.OutputFilename;

            if (Configuration.OverwriteIndex)
                indexPath.DeleteIfExists();

            var names = GetDirectories();
            indexPath.CreatePathIfNotExists();
            return names;
        }

        public Task BuildIndexAsync()
        {
            var names = PrepareForBuild();
            //Using async to prevent the UI from locking (while maybe it should).
            return File.WriteAllLinesAsync(Configuration.OutputFilename, names);
        }

        public void BuildIndex()
        {
            var names = PrepareForBuild();
            File.WriteAllLines(Configuration.OutputFilename, names);
        }

        public IEnumerable<QueryResult> Query(IEnumerable<string> queryTerms)
        {
            var separators = new[] { '-', '_', '\\', ' ', '/' };

            if (!queryTerms.Any())
                yield break;

            if (FileUtilities.IsFileLocked(new FileInfo(Configuration.OutputFilename)))
                yield break;

            var indexLines = Configuration.OutputFilename.ReadLines().ToArray();

            var rootDirectories = Configuration.RootDirectories
                .Select(x => new DirectoryInfo(x).FullName)
                .ToArray();

            foreach (var directory in indexLines)
            {
                var depthOnly = directory;
                foreach (var rootDirectory in rootDirectories)
                {
                    depthOnly = depthOnly.Replace(rootDirectory, "");
                }

                var split = depthOnly.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                if (!split.Any())
                    continue;
                var folderName = split.LastOrDefault() ?? string.Empty;

                //var depthOnly = split.Reverse().Take(Configuration.Depth + 1).Reverse().ToArray();
                var separated = split.SelectMany(x => x.Split(separators, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                var successCount = 0;
                foreach (var word in queryTerms)
                {
                    var found = separated.Any(x => x.Contains(word, StringComparison.OrdinalIgnoreCase));
                    if (found)
                    {
                        //Random number.
                        //Want to give some additional weight to the the results, for each of the words found
                        //Instead of when the same word is found more than once.
                        successCount += 77;
                    }
                    var count = separated.Count(x => x.Contains(word, StringComparison.OrdinalIgnoreCase));
                    if (count > 0)
                    {
                        successCount += count;
                    }
                }
                if (successCount > 0)
                {
                    yield return new QueryResult
                    {
                        FullName = directory,
                        Title = depthOnly,
                        SubTitle = folderName,
                        Score = successCount * 1000 - split.Length,
                    };
                }
            }
        }

        public IEnumerable<string> GetDirectories()
        {
            var rootDirectories = Configuration.RootDirectories.Select(x => new DirectoryInfo(x)).Where(x => x.Exists);

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