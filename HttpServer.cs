using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace oris
{
    public class HttpServer
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private Task listening;

        public HttpServer(string[] prefixes)
        {
            foreach (var prefix in prefixes) listener.Prefixes.Add(prefix);
        }
    }

}
