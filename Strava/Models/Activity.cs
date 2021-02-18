namespace Strava.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class Activity
    {
        private float _distance;
        private float _movingTime;
        private float _averageSpeed;

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ActivityType Type { get; set; }

        [JsonPropertyName("distance")]      
        public float Distance 
        { 
            get => _distance;
            // distance comes in meters from API - we want kilometers
            set => _distance = value / 1000;  
        }

        [JsonPropertyName("start_date_local")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("moving_time")]
        public float MovingTime
        { 
            get => _movingTime;
            // moving time comes in seconds from API - we want minutes
            set => _movingTime = value / 60;
        }

        /// <summary>
        /// The average speed comes from the API as meter/seconds, we want min/km
        /// The calculation is the following:
        ///   m/s * 3,6 = km/h, 
        ///   3600/(km/h) = s/km, 
        ///   s/km : 60 = min/km     ==> x m/s = (1000/x)/60 min/km = 16,66666667/x min/km         
        /// </summary>
        [JsonPropertyName("average_speed")]
        public float AverageSpeed
        {
            get => _averageSpeed;
            set => _averageSpeed = (1000 / value) / 60;
        }

        public override string ToString()
        {
            return $"{StartDate.ToString("dd/MM/yyyy")} \t{Type} \t{Distance}km \t{TimeSpan.FromMinutes(MovingTime).ToString(@"hh\:mm\:ss")}";
        }
    }
}
