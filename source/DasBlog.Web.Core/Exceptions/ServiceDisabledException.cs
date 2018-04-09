using System;

namespace DasBlog.Core.Exceptions
{
    public class ServiceDisabledException : Exception
    {
        public ServiceDisabledException() : base("Service disabled")
        {
        }
    }
}
