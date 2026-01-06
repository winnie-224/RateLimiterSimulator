using RateLimiterSimulator.RateLimiting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterSimulator.Services
{
    /*
     * Simulates an API Gateway that uses a rate limiter to control incoming requests.
     */
    public class ApiGateway
    {
        private readonly IRateLimiter _rateLimiter;
        public ApiGateway(IRateLimiter rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }
        /*
         * Entry point for clients
         */
        public bool HandleRequest(string clientId)
        {
            if (!_rateLimiter.AllowRequest(clientId))
            {
                return false; //HTTP 429: Too many requests
            }
            return true;
        }
    }

}
