namespace beholder_eye
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Security.Cryptography;

    public class Startup
    {
        private static readonly SHA256 _sha256 = SHA256.Create();

        public IConfigurationRoot Configuration { get; }

        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole(o =>
                {
                    o.TimestampFormat = o.TimestampFormat = "[HH:mm:ss] ";
                });
                builder.AddDebug();
            });

            services.AddSingleton(Configuration);
            services.AddSingleton<BeholderEye>();
            services.AddSingleton<HashAlgorithm>(_sha256);
        }
    }
}
