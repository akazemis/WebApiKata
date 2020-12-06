using System;

namespace WebApiKata.Services
{
    public class ExternalApiCallException: Exception
    {
        public ExternalApiCallException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
