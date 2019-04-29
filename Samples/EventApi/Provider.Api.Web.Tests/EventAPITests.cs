using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using PactNet;
using PactNet.Infrastructure.Outputters;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Api.Web.Tests
{
    public class EventApiTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private WebApplicationFactory<Startup> _appFactory;

        public EventApiTests(ITestOutputHelper output)
        {
            _output = output;
//            _appFactory = new WebApplicationFactory<Startup>();
        }

        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            //Arrange
            const string serviceUri = "http://localhost:9222";
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_output)
                }
            };

            var webHostBuilder = Provider.Api.Web.Tests.Startup.CreateWebHostBuilder(new string[] { });

            var server = new Thread(() => webHostBuilder.Build().Run());
            server.Start();
            Task.Delay(TimeSpan.FromSeconds(3)).Wait(); // TODO MC: Remove this wait, put a proper thing in here
            
            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ProviderState($"{serviceUri}/provider-states")
                .ServiceProvider("Event API", serviceUri)
                .HonoursPactWith("Event API Consumer")
                .PactUri(
                    $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Consumer.Tests{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}event_api_consumer-event_api.json")
                .Verify();
        }

        private RequestDelegate Middleware(RequestDelegate arg)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
        }
    }
}
