using System.Diagnostics;

namespace NetworkDriveLauncher.Core.Models
{
    [DebuggerDisplay("({Score}) {Title} -  {SubTitle} - {FullName}")]
    public class QueryResult
    {
        public int Score { get; set; }
        public string SubTitle { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
    }
}
