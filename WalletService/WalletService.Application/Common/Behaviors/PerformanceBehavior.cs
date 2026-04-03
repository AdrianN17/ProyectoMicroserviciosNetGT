using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace WalletService.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly int _msThreshold;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, int msThreshold = 300)
        {
            _logger = logger;
            _msThreshold = msThreshold;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var response = await next();
            sw.Stop();

            if (sw.ElapsedMilliseconds > _msThreshold)
                _logger.LogWarning("Slow request {Request} took {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);

            return response;
        }
    }
}
