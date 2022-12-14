using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TaskListAPI.Test
{
    public class ControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        public ControllerTest(WebApplicationFactory<Startup> factory)
        {
            Factory = factory;
        }

        protected WebApplicationFactory<Startup> Factory
        {
            get;
        }
    }
}