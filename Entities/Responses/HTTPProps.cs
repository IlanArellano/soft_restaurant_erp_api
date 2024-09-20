using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Responses
{
    public class HTTPProps
    {
        public string baseURL {  get; set; }
        public Dictionary<string, string> headers { get; set; } = new Dictionary<string, string>();
    }
}
