﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Mvc.ModuleActivator
{
    public class DashboardOptions
    {
        public DashboardOptions()
        {
            AppPath = "/";
            Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() };
            StatsPollingInterval = 2000;
        }

        /// <summary>
        /// The path for the Back To Site link. Set to <see langword="null" /> in order to hide the Back To Site link.
        /// </summary>
        public string AppPath { get; set; }
 

        public IEnumerable<IDashboardAuthorizationFilter> Authorization { get; set; }

        /// <summary>
        /// The interval the /stats endpoint should be polled with.
        /// </summary>
        public int StatsPollingInterval { get; set; }
    }

}
