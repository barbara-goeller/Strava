namespace Strava.Services
{
    using Blazored.SessionStorage;
    using Strava.Extensions;
    using Strava.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class StravaService : IStravaService
    {
        private const string BaseUri = "https://www.strava.com/api/v3";
        private readonly string ActivitiesUri = $"{BaseUri}/athlete/activities";

        private const string TokenSessionItem = "BearerToken";

        private readonly HttpClient _httpClient;
        private readonly ISessionStorageService _sessionStorage;

        public StravaService(HttpClient httpClient, ISessionStorageService sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<IEnumerable<Activity>> GetAllActivities(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _sessionStorage.GetItemAsync<string>(TokenSessionItem));

            var activities = new List<Activity>();
            int maxPageSize = 200;
            int page = 1;
            bool pageHasActivities = true;

            while (pageHasActivities)
            {
                var tokenParameters = new Dictionary<string, string>
                {
                    { "after", startDate?.ToUnixTimeSeconds().ToString() },
                    { "before", endDate?.ToUnixTimeSeconds().ToString() },
                    { "per_page", $"{maxPageSize}" },
                    { "page", $"{page}" }
                };

                var pageActivitiesStream = _httpClient.GetStreamAsync($"{ActivitiesUri}{await tokenParameters.ToQueryStringAsync()}");
                var pageActivities = await JsonSerializer.DeserializeAsync<IEnumerable<Activity>>(await pageActivitiesStream);

                activities.AddRange(pageActivities);

                pageHasActivities = pageActivities.Any();
                page++;
            }

            return activities;
        }
    }
}
