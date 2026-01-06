using System;
using System.Collections.Concurrent;

namespace RateLimiterSimulator.RateLimiting
{
    public class TokenBucketRateLimiter : IRateLimiter
    {
        private readonly int _capacity;
        private readonly int _refillRatePerSecond;

        //One bucket per client
        private readonly ConcurrentDictionary<string, Bucket> _buckets;
        public TokenBucketRateLimiter(int capacity, int refillRatePerSecond)
        {
            _capacity = capacity;
            _refillRatePerSecond = refillRatePerSecond;
            _buckets = new ConcurrentDictionary<string, Bucket>();
        }
        ///<summary>
        ///Core decision method used by API Gateway to allow or block requests.
        /// </summary>
        public bool AllowRequest(string clientId)
        {
            var bucket = _buckets.GetOrAdd(
                clientId,
                _=>new Bucket(_capacity)
                );
            bucket.Refill(_refillRatePerSecond);
            return bucket.TryConsume();

        }
        ///<summary>
        /// Read only method for UI/montitoring to get current token count for a client.
        ///</summary>
        public int GetAvailableTokens(string clientId)
        {
            if(_buckets.TryGetValue(clientId, out var bucket))
            {
                bucket.Refill(_refillRatePerSecond);
                return bucket.PeekTokens();
            }
            return _capacity;
        }
        //====Nested Bucket Class====
        private class Bucket
        {
            private int _tokens;
            private DateTime _lastRefill;
            private readonly int _capacity;
            private readonly object _lock = new();

            public Bucket(int capacity)
            {
                _capacity = capacity;
                _tokens = capacity;
                _lastRefill = DateTime.UtcNow;
            }
            //Refill tokens
            public void Refill(int refillRatePerSecond)
            {
                lock (_lock)
                {
                    var now = DateTime.UtcNow;
                    var elapsedSeconds = (now - _lastRefill).TotalSeconds;

                    if(elapsedSeconds <= 0)
                    {
                        return;
                    }
                    int tokensToAdd = (int)(elapsedSeconds * refillRatePerSecond);
                    if (tokensToAdd > 0)
                    {
                        _tokens = Math.Min(_capacity, _tokens + tokensToAdd);
                        _lastRefill = now;
                    }
                }
            }
            //Consume  a token if available
            public bool TryConsume()
            {
                lock (_lock)
                {
                    if (_tokens > 0)
                    {
                        _tokens--;
                        return true;
                    }
                    return false;
                }
            }
            //Current token status
            public int PeekTokens()
            {
                lock (_lock)
                {
                    return _tokens;
                }
            }
        }
    }
}
