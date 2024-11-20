using MutareDeleteVoicemail.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MutareDeleteVoicemail.Services;
using NLog.Extensions.Logging;
using NLog.Web;
using MutareDeleteVoicemail.Processors;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RockLib.Logging;
using RockLib.Logging.DependencyInjection;
using System.Threading.Tasks;

namespace MutareDeleteVoicemail
{
    public class Program
    {
        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        static async Task Main()
        {
            IHost host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var rockLibEnvironment = context.Configuration["AppSettings:RockLib.Environment"];
                services.AddSingleton<ITokenService, TokenService>();
                services.AddSingleton<ILogSettingsUsed, LogSettingsUsed>();
                services.AddSingleton<IApplicationSettings, ApplicationSettings>();
                services.AddHttpClient<IMutareVoicemailProcessor, MutareVoicemailServices>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:44323/");
                });
                services.AddSingleton<IExpiredVoicemails, ExpiredVoicemails>();
                services.AddHttpClient();
                services.AddScoped<TestApiService>();
                services.AddLogger(processingMode: Logger.ProcessingMode.Background)
                        .AddCoreLogProvider(applicationId: 219765);
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddNLog();
                });
            })
            .Build();

            Application svc = ActivatorUtilities.CreateInstance<Application>(host.Services);
            await svc.Run();
        }
    }
}