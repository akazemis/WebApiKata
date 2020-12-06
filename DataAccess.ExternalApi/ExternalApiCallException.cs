using System;

namespace DataAccess.ExternalApi
{
    public class ExternalApiCallException: Exception
    {
        public ExternalApiCallException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
