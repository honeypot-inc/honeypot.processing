using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Honeypot.Processing.Parser
{
    public class JsonParser<TModel>
    {
        private readonly JsonSerializerOptions options;

        public JsonParser(JsonSerializerOptions options)
        {
            this.options = options
                ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<TModel> Deserialize(Stream stream, CancellationToken token) =>
            JsonSerializer
                .DeserializeAsync<TModel>(stream, this.options, token)
                .AsTask();
    }
}
