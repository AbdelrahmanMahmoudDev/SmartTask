using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.Services
{
    public class MyAccessTokenProvider : IAuthenticationProvider
    {
        private readonly string _accessToken;

        public MyAccessTokenProvider(string accessToken)
        {
            _accessToken = accessToken;
        }

        public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            request.Headers.Add("Authorization", new AuthenticationHeaderValue("Bearer", _accessToken).ToString());
            return Task.CompletedTask;
        }
    }
}
