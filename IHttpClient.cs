using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Honeypot.Processing.Http
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> Execute(
            HttpRequestMessage request, 
            CancellationToken cancellationToken);
    }
}
