using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NetworkDriveLauncher.Core.Index;
using NetworkDriveLauncher.Core.Utilities;

namespace Flow.Launcher.Plugin.NetworkDriveLauncher
{
    public class NetworkDriveLauncher : IPlugin
    {
        private PluginInitContext _context;

        //To be replaced by other types of index. Not sure if I can IoC this:
        private PlainTextIndex _index;
        private PlainTextIndexConfiguration _configuration;
        private string _executionPath = string.Empty;
        private string _configurationFilename = string.Empty;
        private bool _indexFileExists = false;
        private bool _indexFileIsLocked = false;
        public void Init(PluginInitContext context)
        {
            _context = context;
            _executionPath = new FileInfo(_context.CurrentPluginMetadata.ExecuteFilePath).Directory.FullName;

            _configurationFilename = $"{_executionPath}\\appsettings.json";

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(_configurationFilename, optional: false, reloadOnChange: true)
                .Build();

            _configuration = new PlainTextIndexConfiguration(config);
            _index = new PlainTextIndex(_configuration);
        }

        public List<Result> Query(Query query)
        {
            _indexFileExists = File.Exists(_configuration.OutputFilename);
            _indexFileIsLocked = FileUtilities.IsFileLocked(new FileInfo(_configuration.OutputFilename));
            var list = new List<Result>();
            if (query.SearchTerms.Length < 1)
            {
                // Default result: To build an index
                list.Add(new Result
                {
                    Title = "Build Index",
                    SubTitle = _indexFileIsLocked
                        ? "Cannot build index now. Index is being built."
                        : $"(Re)Builds a new index using appsettings.json RootDirectories",
                    Score = 20,
                    Action = c =>
                    {
                        if (_indexFileIsLocked)
                            return false;
                        _index.BuildIndex();
                        return true;
                    },
                    IcoPath = _indexFileIsLocked
                        ? "Images/cancel.png"
                        : "Images/find.png"
                });

                // Default result: Open settings file
                list.Add(new Result
                {
                    Title = "Open settings file",
                    Score = 10,
                    Action = c =>
                    {
                        System.Diagnostics.Process.Start("explorer.exe", _configurationFilename);
                        return true;
                    },
                    IcoPath = "Images/settings.png"
                });
            }

            //Default result if there is no index file.
            if (!_indexFileExists)
            {
                list.Add(new Result
                {
                    Title = "Index does not exist.",
                    SubTitle = "Please build an index first.",
                    Score = 100,
                    Action = c => false,
                    IcoPath = "Images/cancel.png"
                });
                return list;
            }

            //Default result if index file is locked (index being built)
            if (_indexFileIsLocked)
            {
                list.Add(new Result
                {
                    Title = "Index is being built.",
                    SubTitle = "Please stand by",
                    Score = 100,
                    Action = c => false,
                    IcoPath = "Images/lock.png"
                });
                return list;
            }

            //Results from the query, only in the index exists
            var results = _index.Query(query.SearchTerms);
            list.AddRange(results.Select(x => new Result
            {
                Title = x.Title,
                SubTitle = x.SubTitle,
                Score = x.Score,
                Action = c =>
                {
                    //TODO: Check first if the file exists (?).
                    System.Diagnostics.Process.Start("explorer.exe", x.FullName);
                    return true;
                },
                IcoPath = "Images/folder.png"
            }));

            return list;
        }
    }
}
