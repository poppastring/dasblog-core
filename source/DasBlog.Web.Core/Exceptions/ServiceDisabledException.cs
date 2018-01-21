using System;

namespace DasBlog.Web.Core.Exceptions
{
    public class ServiceDisabledException : Exception
    {
        public ServiceDisabledException() : base("Service disabled")
        {
        }
    }
}
