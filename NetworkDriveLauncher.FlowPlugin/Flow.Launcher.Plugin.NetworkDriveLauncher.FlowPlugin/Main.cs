using Flow.Launcher.Plugin;
using System;
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.NetworkDriveLauncher
{
    public class NetworkDriveLauncher : IPlugin
    {
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            return new List<Result>();
        }
    }
}
