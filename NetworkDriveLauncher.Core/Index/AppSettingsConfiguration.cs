using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NetworkDriveLauncher.Core.Models;
using Wororo.Utilities;

namespace NetworkDriveLauncher.Core.Index
{
    public class AppSettingsConfiguration : IIndexConfiguration
    {
        internal readonly IConfiguration Configuration;

        protected AppSettingsConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public bool OverwriteIndex => Configuration[nameof(OverwriteIndex)]?.ToBool() ?? true;
        public int Depth => Configuration[nameof(Depth)]?.ToInt() ?? 3;
        public IEnumerable<string> RootDirectories
        {
            get
            {
                var rootDirectory = Configuration
                    .GetSection(nameof(RootDirectories))
                    .GetChildren()
                    .Select(x => x.Value)
                    .ToList();
                return rootDirectory;
            }
        }
    }
}