using System;
using System.Threading;
using System.Threading.Tasks;

namespace Honeypot.Processing.Pipeline
{
    public class NonUniformMultiplexor<TInput, TIntermediateResultLeft, TIntermediateResultRight, TResult>
    {
        public delegate Task<TIntermediateResultLeft> LeftHandler(
            TInput input, 
            CancellationToken token);

        public delegate Task<TIntermediateResultRight> RightHandler(
            TInput input, 
            CancellationToken token);

        public delegate Task<TResult> Aggregate(
            TIntermediateResultLeft leftResult,
            TIntermediateResultRight rightResult,
            CancellationToken token);

        private readonly LeftHandler leftHandler;
        private readonly RightHandler rightHandler;
        private readonly Aggregate aggregator;

        public NonUniformMultiplexor(
            Aggregate aggregator, 
            LeftHandler leftHandler,
            RightHandler rightHandler)
        {
            this.aggregator = aggregator
                ?? throw new ArgumentNullException(nameof(aggregator));
            this.leftHandler = leftHandler
                ?? throw new ArgumentNullException(nameof(leftHandler));
            this.rightHandler = rightHandler
                ?? throw new ArgumentNullException(nameof(rightHandler));
        }

        public async Task<TResult> Handle(TInput input, CancellationToken token) =>
            await this
                .aggregator(
                    await this.leftHandler(input, token).ConfigureAwait(false),
                    await this.rightHandler(input, token).ConfigureAwait(false),
                    token)
                .ConfigureAwait(false);
    }
}
