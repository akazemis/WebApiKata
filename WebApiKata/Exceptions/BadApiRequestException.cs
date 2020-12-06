using System;
namespace WebApiKata.Exceptions
{
    public class BadApiRequestException: Exception
    {
        public BadApiRequestException(string message = "", Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
