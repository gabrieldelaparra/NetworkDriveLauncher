using System.Collections.Generic;

namespace NetworkDriveLauncher.Core.Models
{
    public interface IIndexConfiguration
    {
        IEnumerable<string> RootDirectories { get; }
        int Depth { get; }
        bool OverwriteIndex { get; }
    }
}