using System.Threading.Tasks;

namespace BaseProject.Infrastructure
{
    public interface IHttpRequestRunner<TContent, TResponse>
    {
        Task<TResponse> ExecutePostWithRetryAsync(string endpoint);
    }
}