using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterSimulator.RateLimiting
{
    public interface IRateLimiter
    {
        bool AllowRequest(string clientId);

    }

}
