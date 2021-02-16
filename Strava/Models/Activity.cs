namespace Strava.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class Activity
    {
        private float _distance;
        private float _movingTime;

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

        [JsonPropertyName("average_speed")]
        public float AverageSpeed { get; set; } // meters/second

        public override string ToString()
        {
            return $"{StartDate.ToString("dd/MM/yyyy")} \t{Type} \t{Distance}km \t{TimeSpan.FromMinutes(MovingTime).ToString(@"hh\:mm\:ss")}";
        }
    }
}
