namespace Strava.Extensions
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class DictionaryExtensions
    {
        public async static Task<string> ToQueryStringAsync(this Dictionary<string,string> dict)
        {
            var dictFormUrlEncoded = new FormUrlEncodedContent(dict);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            return $"?{queryString}";
        }
    }
}
