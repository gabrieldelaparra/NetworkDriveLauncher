using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDriveLauncher.Core
{
    public class QueryResult
    {
        public int Score { get; set; }
        public string SubTitle { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
    }
}
