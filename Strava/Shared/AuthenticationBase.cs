namespace Strava.Shared
{
    using Blazored.SessionStorage;
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Configuration;
    using Strava.Extensions;
    using Strava.Models;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Web;

    public class AuthenticationBase : ComponentBase
    { 
        [Inject]
        public IConfiguration Configuration { get; set; }

        [Inject]
        public ISessionStorageService SessionStorage { get; set; }

        [Inject]
        public AuthorizationConfiguration AuthConfig { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        private const string TokenSessionItem = "BearerToken";

        public async Task EnsureAuthenticatedAsync() 
        {
            var sessionStorageContainsToken = await SessionStorage.ContainKeyAsync(TokenSessionItem);
            string authorizationCode = GetQueryParam("code");
            var isAuthorizationCodeAvailable = authorizationCode != string.Empty;

            if (!sessionStorageContainsToken && !isAuthorizationCodeAvailable)
            {
                var authorizationQueryParameters = new Dictionary<string, string>
                {
                    { "client_id", AuthConfig.ClientId },
                    { "response_type", "code"},
                    { "redirect_uri", NavigationManager.Uri },
                    { "scope", AuthConfig.Scope },
                };
                NavigationManager.NavigateTo($"{AuthConfig.AuthorizationEndpoint}{await authorizationQueryParameters.ToQueryStringAsync()}");
            }
            else if (!sessionStorageContainsToken && isAuthorizationCodeAvailable)
            {
                var tokenParameters = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", authorizationCode },
                    { "client_id", AuthConfig.ClientId },
                    { "client_secret", AuthConfig.ClientSecret }
                };

                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await HttpClient.PostAsync(AuthConfig.TokenEndpoint, new FormUrlEncodedContent(tokenParameters));

                var token = await JsonSerializer.DeserializeAsync<Token>(await response.Content.ReadAsStreamAsync());

                await SessionStorage.SetItemAsync(TokenSessionItem, token.AccessToken);
            }
            // TODO: here we could check if token is still valid (when sessionStorageContainsToken) and get new one otherwise
        }

        private string GetQueryParam(string paramName)
        {
            var currentUriBuilder = new UriBuilder(NavigationManager.Uri);
            var queryParameters = HttpUtility.ParseQueryString(currentUriBuilder.Query);
            return queryParameters[paramName] ?? string.Empty;
        }
    }
}
