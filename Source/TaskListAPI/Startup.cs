using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSwag;

namespace TaskListAPI
{
    public class Startup
    {
        private IConfiguration Config { get; }
        private IWebHostEnvironment CurrentEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Config = configuration;
            CurrentEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddControllers().AddNewtonsoftJson(options => SetSerializerSettings(options.SerializerSettings));

            services.AddControllers(opts => opts.Filters.Add(new TransactionFilter()));

            services.AddOpenApiDocument();
            services.AddSwaggerDocument(settings =>
            {
                settings.DocumentName = "swagger";
                settings.PostProcess = document =>
                {
                    document.Info.Title = "TaskList API";
                    document.Info.Version = "v1";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "MyName",
                        Email = "MyName@MyCompany.com"
                    };
                };
            });

            // The dependency injection container doesn't know anything about the controllers and so can't check if their dependencies are registered.
            // Registering controllers as services allows the dependency injection container to check their dependencies. 
            // http://asp.net-hacker.rocks/2017/05/08/add-custom-ioc-in-aspnetcore.html
            // https://andrewlock.net/new-in-asp-net-core-3-service-provider-validation/
            services.AddControllers().AddControllersAsServices();

            // The HttpContextAccessor is used to provide access to the HttpContext from custom services.
            // This can be required e.g. to read the request headers. See the Microsoft docs on accessing the HttpContext.
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-5.0
            var httpContextAccessor = new HttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor>(httpContextAccessor);
        }

        public static void SetSerializerSettings(JsonSerializerSettings settings)
        {
            settings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            settings.Converters.Add(new IsoDateTimeConverter());
            settings.Converters.Add(new StringEnumConverter());
            settings.MissingMemberHandling = MissingMemberHandling.Error;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandlingPath =
                    env.IsDevelopment() ? ErrorController.ErrorDev : ErrorController.ErrorProduction,
                AllowStatusCode404Response = true
            });

            app.UseRouting();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}