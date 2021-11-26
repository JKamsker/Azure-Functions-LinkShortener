using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;

namespace LinkShortener.Extensions
{
    public static class HttpExtensions
    {
        public static string GetHostPath(this HttpRequest req, bool forceHttps = true)
        {
            var scheme = forceHttps ? "https" : req.Scheme;
            return $"{scheme}://{req.Host}";
        }
    }
}
