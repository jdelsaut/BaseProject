using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BoxApi.Infrastructure
{
    public interface ITokenGenerator
    {
        Task<AuthenticationHeaderValue> GetAuthenticationHeaderValueAsync(string clientId, string clientSecret);
    }
}