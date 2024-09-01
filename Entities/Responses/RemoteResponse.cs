using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Responses
{
    public class RemoteReponse
    {
        public bool success {  get; set; }
        public object? response { get; set; }

        public object? error { get; set; } = null;
    }
}
