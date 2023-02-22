using System;
using BoxApi.Infrastructure.Settings;
using System.Collections.Generic;
using System.Text;

namespace BoxApi.ApiDrivers
{
    public static class JwtClientCredentials
    {
        internal static (string ClientId, string ClientSecret) GetClientCredentialsConfigKeys(string type = null)
        {
            //No type is required for search provider than any category (roadside, domestic or vehicle_for_hire) 
            //will do the tricks
            if (type == null)
            {
                return (AppSettingsKeys.ClientId, AppSettingsKeys.ClientSecret);
            }

            switch (type)
            {
                default:
                    throw new InvalidOperationException($"GetClientCredentialsConfigKeys - {type} is not supported.");
            }
        }
    }
}
