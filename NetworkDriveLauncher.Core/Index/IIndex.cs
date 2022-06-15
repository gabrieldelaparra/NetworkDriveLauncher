using NetworkDriveLauncher.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkDriveLauncher.Core.Index
{
    public interface IIndex<out T> where T : IIndexConfiguration
    {
        T Configuration { get; }
        Task BuildIndexAsync();
        void BuildIndex();
        IEnumerable<QueryResult> Query(IEnumerable<string> queryTerms);
    }
}