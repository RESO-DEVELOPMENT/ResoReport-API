using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Reso.Sdk.Core.Custom;
using Reso.Sdk.Core.Extension;
using ResoReport.Middlewares;
using ResoReportDataAccess.Models;
using ResoReportDataService.Models;
using StackExchange.Redis;

namespace ResoReport
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            });
            services.AddMvc().ConfigureApiBehaviorOptions(options =>
                    // options.SuppressModelStateInvalidFilter = true
                {
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        var modelState = actionContext.ModelState.Values;
                        throw new ErrorResponse(400, modelState.First().Errors.First().ErrorMessage);
                    };
                }
            );
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ResoReport API", Version = "v1.0" });
                    c.SwaggerDoc("v2", new OpenApiInfo { Title = "ResoReport API", Version = "v2.0" });
                }
            );
            services.ConfigureAutoMapper();
            services.AddDbContext<PosSystemContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(Configuration.GetConnectionString("ConnectionString"));
            });
            services.AddDbContext<DataWareHouseReportingContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(Configuration.GetConnectionString("DataWareHouseReportingConnectionString"));
            });
            services.ConfigureFilter<ErrorHandlingFilter>();
            services.ConfigureDI();
            
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(new ConfigurationOptions()
                {
                    EndPoints = { Configuration["Endpoint:RedisEndpoint"] },
                    Password = Configuration["Endpoint:Password"]
                }));
            var redisConfigString = Configuration["Endpoint:RedisEndpoint"] + ",password=" +
                                    Configuration["Endpoint:Password"];

            services.ConfigMemoryCacheAndRedisCache(redisConfigString);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}