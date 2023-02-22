using BaseProject.Infrastructure.Settings;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System.Threading.Tasks;

namespace BaseProject.Api.AccessToken
{
    /// <summary>
    /// Provides a new binding instance for the function host.
    /// </summary>
    public class AccessTokenBindingProvider : IBindingProvider
    {
        private readonly ISettingsReader _settingsReader;

        public AccessTokenBindingProvider(ISettingsReader settingsReader)
        {
            _settingsReader = settingsReader;
        }
        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            IBinding binding = new AccessTokenBinding(_settingsReader);
            return Task.FromResult(binding);
        }
    }
}
