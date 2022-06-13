using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Wororo.Utilities;

namespace NetworkDriveLauncher.Core
{
    public class PlainTextIndexConfiguration : IIndexConfiguration
    {
        private readonly IConfiguration _configuration;

        public PlainTextIndexConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool OverwriteIndex => _configuration[nameof(OverwriteIndex)]?.ToBool() ?? false;
        public int Depth => _configuration[nameof(Depth)]?.ToInt() ?? 3;
        public IEnumerable<string> RootDirectories
        {
            get
            {
                var rootDirectory = this._configuration
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