using System.Net;
using System.Reflection;

namespace Core.Extensions;

public static class ExceptionExtensions
{
    public static bool IsRetryableNetworkError(this Exception ex)
    {
        if (ex is HttpRequestException httpEx)
        {
            if (httpEx.StatusCode is HttpStatusCode.InternalServerError or HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable
                or HttpStatusCode.GatewayTimeout) { return true; }
            if (httpEx.InnerException is IOException) { return true; }

            string message = httpEx.Message;

            if (message.Contains("SSL", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("forcibly closed", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("timed out", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("connection reset", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("connection aborted", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("name or service not known", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static Exception Unwrap(this Exception ex)
    {
        return ex is TargetInvocationException tie && tie.InnerException != null
            ? tie.InnerException
            : ex;
    }

    public static Exception GetInnermost(this Exception ex)
    {
        while (ex.InnerException != null)
        {
            ex = ex.InnerException;
        }

        return ex;
    }
}

