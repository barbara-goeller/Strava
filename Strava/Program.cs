namespace Strava
{
    using Blazored.SessionStorage;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Strava.Models;
    using Strava.Services;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient());

            builder.Services.AddBlazoredSessionStorage();

            builder.Services.AddSingleton(new AuthorizationConfiguration
            {
                AuthorizationEndpoint = builder.Configuration["Authorization:AuthorizationEndpoint"],
                TokenEndpoint = builder.Configuration["Authorization:TokenEndpoint"],
                ClientId = builder.Configuration["Authorization:ClientId"],
                ClientSecret = builder.Configuration["Authorization:ClientSecret"],
                Scope = builder.Configuration["Authorization:Scope"],
            });

            builder.Services.AddScoped<IStravaService, StravaService>();
            builder.Services.AddScoped<IChartService, ChartService>();

            await builder.Build().RunAsync();
        }
    }
}
