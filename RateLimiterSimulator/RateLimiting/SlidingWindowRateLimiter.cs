using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace RateLimiterSimulator.RateLimiting
{
    public class SlidingWindowRateLimiter : IRateLimiter
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _windowSize;

        private readonly ConcurrentDictionary<string, LinkedList<DateTime>> _requestLogs;
        public SlidingWindowRateLimiter(int maxRequests, TimeSpan windowSize)
        {
            _maxRequests = maxRequests;
            _windowSize = windowSize;
            _requestLogs = new ConcurrentDictionary<string, LinkedList<DateTime>>();
        }

        public bool AllowRequest(string clientId) 
        {
            var now = DateTime.UtcNow;
            var log = _requestLogs.GetOrAdd(
                clientId,
                _=>new LinkedList<DateTime>()
                );
            lock(log)
            {
                while(log.First != null && now - log.First.Value > _windowSize)
                {
                    log.RemoveFirst();
                }
                if(log.Count >= _maxRequests)
                {
                    return false; //Rate limit exceeded
                }
                log.AddLast(now);
                return true;
            }
        }
    }
}
