using Microsoft.AspNetCore.Http;

namespace Demo.Library.Extensions
{
    /**
     * Determines if the Incoming HttpRequest is Ajax or not
     */
    public static class RequestExtension
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request != null && request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
    }
}
