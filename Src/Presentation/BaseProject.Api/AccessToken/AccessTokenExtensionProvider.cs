using BoxApi.Infrastructure.Settings;
using Microsoft.Azure.WebJobs.Host.Config;

namespace BoxApi.Api.AccessToken
{
    /// <summary>
    /// Wires up the attribute to the custom binding.
    /// </summary>
    public class AccessTokenExtensionProvider : IExtensionConfigProvider
    {
        private readonly ISettingsReader _settingsReader;

        public AccessTokenExtensionProvider(ISettingsReader settingReader)
        {
            _settingsReader = settingReader;
        }
        //Define a rule for the attribute definition that will be picked up by the Azure Functions runtime. 
        //This rule can associate the attribute with a custom binding 
        public void Initialize(ExtensionConfigContext context)
        {
            // Creates a rule that links the attribute to the binding
            var provider = new AccessTokenBindingProvider(_settingsReader);
            var rule = context.AddBindingRule<AccessTokenAttribute>().Bind(provider);
        }
    }
}
