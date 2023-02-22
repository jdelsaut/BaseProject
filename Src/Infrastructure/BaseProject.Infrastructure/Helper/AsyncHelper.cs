using System;
using System.Threading.Tasks;

namespace BoxApi.Infrastructure.Helpers
{
    public static class AsyncHelper
    {
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return Task.Run(func).GetAwaiter().GetResult();
        }
    }
}
