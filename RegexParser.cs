using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System;

namespace Honeypot.Processing.Parser
{
    //// https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices
    public class RegexParser<TInput, TResult>
    {
        public delegate Task<TResult> Handle(Match match, CancellationToken token); 

        private readonly string pattern;
        private readonly Handle handler;

        public RegexParser(string pattern, Handle handler)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("null or empty", nameof(pattern));
            }

            this.pattern = pattern;
            this.handler = handler
                ?? throw new ArgumentNullException(nameof(handler));
        }
        
        public Task<TResult> Parse(string input, TimeSpan timeout, CancellationToken token)
        {
            var match = Regex.Match(
                input, 
                this.pattern, 
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, 
                timeout);

            return this.handler(match, token);
        }
    }
}
