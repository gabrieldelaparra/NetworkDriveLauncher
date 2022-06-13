using System.Collections.Generic;

namespace NetworkDriveLauncher.Core
{
    public interface IIndexConfiguration
    {
        IEnumerable<string> RootDirectories { get; }
        int Depth { get; }
        bool OverwriteIndex { get; }
    }
}