using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkDriveLauncher.Core.Models
{
    public interface IIndex<out T> where T : IIndexConfiguration
    {
        T Configuration { get; }
        Task BuildIndexAsync();
        void BuildIndex();
        IEnumerable<QueryResult> Query(IEnumerable<string> queryTerms);
    }
}