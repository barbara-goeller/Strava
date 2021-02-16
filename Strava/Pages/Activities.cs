namespace Strava.Pages
{
    using BlazorDateRangePicker;
    using Microsoft.AspNetCore.Components;
    using Strava.Models;
    using Strava.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class Activities
    {
        [Inject]
        public IStravaService StravaService { get; set; }

        DateTimeOffset? StartDate { get; set; } = DateTime.Today.AddMonths(-1);

        DateTimeOffset? EndDate { get; set; } = DateTime.Today.AddDays(1).AddTicks(-1);

        public IEnumerable<Activity> UserActivities { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await EnsureAuthenticatedAsync();

            Console.WriteLine($"{DateTime.Now} Initialized Activities");

            UserActivities = (await StravaService.GetAllActivities(StartDate, EndDate)).ToList();
        }

        public void OnDateRangeSelect(DateRange dateRange)
        {
            Task.Run(async () => {
                UserActivities = (await StravaService.GetAllActivities(StartDate, EndDate)).ToList();
                StateHasChanged();
            });
        }
    }
}
