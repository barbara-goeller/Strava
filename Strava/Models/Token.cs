namespace Strava.Models
{
    using System.Text.Json.Serialization;

    public sealed class Token
    {
        [JsonPropertyName("token_type")]
        public string Type { get; init; }

        [JsonPropertyName("expires_at")]
        // UnixTimeStamp
        public int ExpiresAt { get; init; }

        [JsonPropertyName("expires_in")] 
        // seconds
        public int ExpiresIn { get; init; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }
    }
}
