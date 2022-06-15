using Microsoft.Extensions.Configuration;

namespace NetworkDriveLauncher.Core.Index
{
    public class PlainTextIndexConfiguration : AppSettingsConfiguration
    {
        public PlainTextIndexConfiguration(IConfiguration configuration) : base(configuration)
        {
        }
        public string OutputFilename => Configuration[nameof(OutputFilename)];
    }
}