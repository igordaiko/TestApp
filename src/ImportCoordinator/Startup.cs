using System.IO;


using ImportCoordinator.Core;
using ImportCoordinator.Core.Services;
using ImportCoordinator.Infrastruture;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ImportCoordinator
{
    public class Startup
    {
        private const string HomeKey = "home";

        private const string SettingsSection = "settings";

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var home = config.GetValue(HomeKey, Directory.GetCurrentDirectory());

            var hostBuilder = new WebHostBuilder()
                .UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build());

            var environment = hostBuilder.GetSetting(WebHostDefaults.EnvironmentKey);

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(home)
                .AddJsonFile("config.json", false, true);

            if (!string.IsNullOrWhiteSpace(environment))
                configBuilder.AddJsonFile($"config.{environment.ToLower()}.json", true, true);

            var configuration = configBuilder
                .AddCommandLine(args)
                .Build();

            var host = hostBuilder
                .UseKestrel()
                .UseContentRoot(home)
                .UseConfiguration(configuration)
                .ConfigureServices(services => services.AddSingleton<IConfiguration>(configuration))
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<Session>();

            Settings.Instance = Configuration.GetSection(SettingsSection).Get<Settings>();

            services.AddTransient<EventService>();
            services.AddTransient<EmailService>();
            services.AddTransient<AzureService>();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));


            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseAuth();
            app.UseMvc();
        }
    }
}
