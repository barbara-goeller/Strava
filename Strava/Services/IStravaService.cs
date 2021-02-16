namespace Strava.Services
{
    using Strava.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStravaService
    {
        Task<IEnumerable<Activity>> GetAllActivities(DateTimeOffset? startDate, DateTimeOffset? endDate);
    }
}
