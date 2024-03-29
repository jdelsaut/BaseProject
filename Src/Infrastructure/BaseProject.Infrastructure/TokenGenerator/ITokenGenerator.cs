﻿using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BaseProject.Infrastructure
{
    public interface ITokenGenerator
    {
        Task<AuthenticationHeaderValue> GetAuthenticationHeaderValueAsync(string clientId, string clientSecret);
    }
}