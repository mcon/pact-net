using Microsoft.AspNetCore.Authorization;

namespace Provider.Api.Web.Tests
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    namespace Provider.Api.Web.Tests
    {
        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public static IWebHost BuildWebHost(string[] args) =>
                CreateWebHostBuilder(args).Build();

            public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            {
                return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .UseUrls("http://localhost:9222");
            }
            public IConfiguration Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.Use(new ProviderStateMiddleware().Invoke);
                app.UseMvc();
            }
        }
    }
}