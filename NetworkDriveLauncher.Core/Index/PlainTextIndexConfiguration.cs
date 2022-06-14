using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Wororo.Utilities;

namespace NetworkDriveLauncher.Core.Index
{
    public class PlainTextIndexConfiguration : IIndexConfiguration
    {
        private readonly IConfiguration _configuration;

        public PlainTextIndexConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool OverwriteIndex => _configuration[nameof(OverwriteIndex)]?.ToBool() ?? true;
        public int Depth => _configuration[nameof(Depth)]?.ToInt() ?? 3;
        public IEnumerable<string> RootDirectories
        {
            get
            {
                var rootDirectory = _configuration
                    .GetSection(nameof(RootDirectories))
                    .GetChildren()
                    .Select(x => x.Value)
                    .ToList();
                return rootDirectory;
            }
        }

        public string OutputFilename => _configuration[nameof(OutputFilename)];
    }
}