using NetworkDriveLauncher.Core.Models;
using System.Collections.Generic;

namespace NetworkDriveLauncher.Core.Index
{
    public interface IIndex<out T> where T : IIndexConfiguration
    {
        T Configuration { get; }
        void BuildIndex();
        IEnumerable<QueryResult> Query(IEnumerable<string> queryTerms);
    }
}