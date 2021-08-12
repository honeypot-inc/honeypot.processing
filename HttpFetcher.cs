using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Honeypot.Processing.Http
{
    public class HttpFetcher<TResult>
    {
        private readonly IHttpClient client;
        private readonly Handle handler;

        public delegate Task<TResult> Handle(HttpContent content, CancellationToken token); 

        public HttpFetcher(
            IHttpClient client, 
            Handle handler)
        {
            this.client = client 
                ?? throw new ArgumentNullException(nameof(client));
            this.handler = handler
                ?? throw new ArgumentNullException(nameof(handler));
        }

        public async Task<TResult> Fetch(Uri uri, CancellationToken token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                uri ?? throw new ArgumentNullException(nameof(uri)));
            using var response = await client
                .Execute(request, token)
                .ConfigureAwait(false);
            return await this
                .handler(response.EnsureSuccessStatusCode().Content, token)
                .ConfigureAwait(false);
        }
    }
}
