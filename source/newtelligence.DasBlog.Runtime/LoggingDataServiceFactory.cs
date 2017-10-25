using System;
using System.Collections;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{
    public static class LoggingDataServiceFactory
    {
        static readonly Dictionary<string, ILoggingDataService> services = new Dictionary<string, ILoggingDataService>(StringComparer.OrdinalIgnoreCase);

        public static ILoggingDataService GetService(string logLocation)
        {
            ILoggingDataService service;

            lock (((ICollection)services).SyncRoot)
            {
                if (!services.TryGetValue(logLocation, out service))
                {

                    service = new LoggingDataServiceXml(logLocation);
                    services.Add(logLocation, service);
                }
            }
            return service;
        }
    }
}