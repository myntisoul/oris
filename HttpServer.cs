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
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "search.html");
        private readonly HttpListener listener = new HttpListener();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private Task listening;

        public HttpServer(string[] prefixes)
        {
            foreach (var prefix in prefixes) listener.Prefixes.Add(prefix);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("started");
            listening = Task.Run(() => ListenAsyncLoop());
        }

        public async Task ListenAsyncLoop()
        {
            while (!cts.IsCancellationRequested && listener.IsListening)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    await ProcessRequestAsync(context);
                }
                catch (HttpListenerException) { break; }
                catch (ObjectDisposedException) { break; }
                }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            var response = context.Response;

            string responseText = File.ReadAllText(path);

            byte[] buffer = Encoding.UTF8.GetBytes(responseText);

            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/html; charset=utf-8";

            using Stream output = response.OutputStream;
            await output.WriteAsync(buffer);
            await output.FlushAsync();

            Console.WriteLine($"Запрос обработан: {context.Request.Url}");
        }

        public void Stop()
        {
            cts.Cancel();
            if (listener.IsListening)
            {
                listener.Stop();
            }
            listener.Close();
            Console.WriteLine("stopped");
        }
    }

}

public class Settings
{
    public string[] Prefixes { get; set; }
}