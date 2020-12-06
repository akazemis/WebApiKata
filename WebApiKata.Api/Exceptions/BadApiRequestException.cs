using System;
namespace WebApiKata.Api.Exceptions
{
    public class BadApiRequestException: Exception
    {
        public BadApiRequestException(string message = "", Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
