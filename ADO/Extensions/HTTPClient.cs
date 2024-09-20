using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.Extensions
{
    public static class HTTPClient
    {
        public static void setHeadersFromDictionary(this HttpClient httpClient, Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

    }
}
