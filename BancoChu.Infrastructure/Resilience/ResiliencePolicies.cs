using Polly;
using Polly.Extensions.Http;

namespace BancoChu.Infrastructure.Resilience
{
    public static class ResiliencePolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetDefaultHttpPolicy()
        {
            var policy = Policy.WrapAsync(GetRetryPolicy(), GetCircuitBreakerPolicy(), GetTimeoutPolicy());
            return policy;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(10)
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            var timeOut = Policy.TimeoutAsync<HttpResponseMessage>(10);
            return timeOut;
        }
    }
}