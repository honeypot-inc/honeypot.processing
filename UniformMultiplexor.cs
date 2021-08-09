using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Honeypot.Processing.Pipeline
{
    public class UniformMultiplexor<TInput, TIntermediateResult, TResult>
    {
        public delegate Task<TIntermediateResult> Handler(
            TInput input, 
            CancellationToken token);

        public delegate Task<TResult> Aggregate(
            IEnumerable<TIntermediateResult> results, 
            CancellationToken token);

        private readonly List<Handler> handlers;
        private readonly Aggregate aggregator;

        public UniformMultiplexor(Aggregate aggregator, params Handler[] handlers)
        {
            if (handlers.Any(h => h is null))
            {
                throw new ArgumentException("handler is null", nameof(handlers));
            }

            this.aggregator = aggregator
                ?? throw new ArgumentNullException(nameof(aggregator));
            this.handlers = new List<Handler>(handlers);
        }

        public async Task<TResult> Handle(TInput input, CancellationToken token) =>
            await this
                .aggregator(
                    await Task
                        .WhenAll(this.handlers.Select(h => h(input, token)))
                        .ConfigureAwait(false),
                    token)
                .ConfigureAwait(false);
    }
}
