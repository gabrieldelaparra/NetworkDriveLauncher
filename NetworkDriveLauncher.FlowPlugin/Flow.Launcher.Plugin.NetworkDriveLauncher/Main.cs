using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NetworkDriveLauncher.Core.Index;
using Wororo.Utilities;

namespace Flow.Launcher.Plugin.NetworkDriveLauncher
{
    public class NetworkDriveLauncher : IPlugin
    {
        private PluginInitContext _context;

        //To be replaced by other types of index. Not sure if I can IoC this:
        private PlainTextIndex _index;
        private PlainTextIndexConfiguration _configuration;
        private string _executionPath = string.Empty;
        public void Init(PluginInitContext context)
        {
            _context = context;
            _executionPath = new FileInfo(_context.CurrentPluginMetadata.ExecuteFilePath).Directory.FullName;

            var configFilename = $"{_executionPath}\\appsettings.json";

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(configFilename)
                .Build();

            _configuration = new PlainTextIndexConfiguration(config);
            _index = new PlainTextIndex(_configuration);
        }

        public List<Result> Query(Query query)
        {
            // Default result: To build an index:
            var buildResult = new Result
            {
                Title = "Build Index",
                SubTitle = $"(Re)Builds a new index using appsettings.json RootDirectories",
                Score = -100,
                Action = c =>
                {
                    _index.BuildIndex();
                    return true;
                },
                IcoPath = "Images/app.png"
            };
            
            var list = new List<Result>() { buildResult };
            if (!File.Exists(_configuration.OutputFilename))
                return list;

            //Results from the query, only in the index exists
            var results = _index.Query(query.SearchTerms);
            list.AddRange(results.Select(x => new Result
            {
                Title = x.Title,
                SubTitle = x.SubTitle,
                Score = x.Score,
                Action = c =>
                {
                    System.Diagnostics.Process.Start("explorer.exe", x.FullName);
                    return true;
                }
            }));

            return list;
        }
    }
}
