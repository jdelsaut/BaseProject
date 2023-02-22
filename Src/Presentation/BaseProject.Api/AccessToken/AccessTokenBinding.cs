using BoxApi.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System.Threading.Tasks;

namespace BoxApi.Api.AccessToken
{
    public class AccessTokenBinding : IBinding
    {
        private readonly ISettingsReader _settingsReader;

        public AccessTokenBinding(ISettingsReader settingsReader)
        {
            _settingsReader = settingsReader;
        }

        // access to the underlying request context so can obtain application settings and the HTTP request header.
        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            // Get the HTTP request
            var request = context.BindingData["req"] as HttpRequest;        

            // open the JWT and create the ClaimsPrinciple
            return Task.FromResult<IValueProvider>(new AccessTokenValueProvider(request, _settingsReader ?? new SettingsReader()));
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) => null;

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();

    }
}
