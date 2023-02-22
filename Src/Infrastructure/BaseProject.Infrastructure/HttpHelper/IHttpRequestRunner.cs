using System.Threading.Tasks;

namespace BoxApi.Infrastructure
{
    public interface IHttpRequestRunner<TContent, TResponse>
    {
        Task<TResponse> ExecutePostWithRetryAsync(string endpoint);
    }
}