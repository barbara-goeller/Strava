namespace Strava.Pages
{
    using BlazorDateRangePicker;
    using ChartJs.Blazor;
    using ChartJs.Blazor.PieChart;
    using Microsoft.AspNetCore.Components;
    using Strava.Extensions;
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

        [Inject]
        public IChartService ChartService { get; set; }

        DateTimeOffset? StartDate { get; set; } = DateTime.Today.FirstDayOfMonth();

        DateTimeOffset? EndDate { get; set; } = DateTime.Today.LastDayOfMonth();

        public IEnumerable<Activity> UserActivities { get; set; }

        private int _activityCount;
        private float _totalDistance;
        private float _totalDuration;

        private float _aitLaufChallengeDistance;

        private PieConfig _countChartConfig;
        private PieConfig _distanceChartConfig;
        private PieConfig _durationChartConfig;
        private Chart _countChart;
        private Chart _distanceChart;
        private Chart _durationChart;

        protected async override Task OnInitializedAsync()
        {
            await EnsureAuthenticatedAsync();

            Console.WriteLine($"{DateTime.Now} Initialized Activities");

            UserActivities = (await StravaService.GetAllActivities(StartDate, EndDate)).ToList();
            CalculateStatistics();
            InitializeCharts();
        }

        private void CalculateStatistics()
        {
            _activityCount = UserActivities.Count();
            _totalDistance = UserActivities.Sum(activity => activity.Distance);
            _totalDuration = UserActivities.Sum(activity => activity.MovingTime);

            var bikeConversionRatio = 0.23f;
            _aitLaufChallengeDistance = UserActivities.Where(activity => activity.Type == ActivityType.Hike ||
                                                                         activity.Type == ActivityType.Run ||
                                                                         activity.Type == ActivityType.Walk ||
                                                                         activity.Type == ActivityType.Ride)
                                                     .Select(activity => activity.Type == ActivityType.Ride ? bikeConversionRatio * activity.Distance : activity.Distance)
                                                     .Sum();
        }

        private void InitializeCharts()
        {
            var activityGroupsByType = UserActivities.GroupBy(a => a.Type);
            Dictionary<string, int> activityTypesWithCount = activityGroupsByType.ToDictionary(group => group.Key.ToString(), group => group.Count());
            Dictionary<string, float> activityTypesWithDistances = activityGroupsByType.ToDictionary(group => group.Key.ToString(), group => group.Sum(a => a.Distance));
            Dictionary<string, float> activityTypesWithDuration = activityGroupsByType.ToDictionary(group => group.Key.ToString(), group => group.Sum(a => a.MovingTime));

            _distanceChartConfig = ChartService.DrawPieChart("Overall distance by type", new SortedDictionary<string, float>(activityTypesWithDistances));
            _countChartConfig = ChartService.DrawPieChart("Activity count by type", new SortedDictionary<string, int>(activityTypesWithCount));
            _durationChartConfig = ChartService.DrawPieChart("Overall duration by type", new SortedDictionary<string, float>(activityTypesWithDuration));
        }

        public void OnDateRangeSelect(DateRange dateRange)
        {
            Task.Run(async () => {
                UserActivities = (await StravaService.GetAllActivities(StartDate, EndDate)).ToList();

                CalculateStatistics();

                var activityGroupsByType = UserActivities.GroupBy(a => a.Type);
                Dictionary<string, int> activityTypesWithCount = activityGroupsByType.ToDictionary(group => group.Key.ToString(), group => group.Count());
                Dictionary<string, float> activityTypesWithDistances = activityGroupsByType.ToDictionary(group => group.Key.ToString(), group => group.Sum(a => a.Distance));
                Dictionary<string, float> activityTypesWithDuration = activityGroupsByType.ToDictionary(group => group.Key.ToString(), group => group.Sum(a => a.MovingTime));

                ChartService.UpdatePieChartData(_countChart, _countChartConfig, new SortedDictionary<string, int>(activityTypesWithCount));
                ChartService.UpdatePieChartData(_distanceChart, _distanceChartConfig, new SortedDictionary<string, float>(activityTypesWithDistances));
                ChartService.UpdatePieChartData(_durationChart, _durationChartConfig, new SortedDictionary<string, float>(activityTypesWithDuration));

                StateHasChanged();
            });
        }
    }
}
