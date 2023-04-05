using Insurance.Api.Data;
using Insurance.Api.Repositories.InsuranceRepository;
using Insurance.Api.Repositories.OrderRepo;
using Insurance.Api.Repositories.SurchargeRateRepo;
using Insurance.Api.Services;
using Insurance.Api.Services.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Insurance.Api
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
            MySettings mySettings = Configuration.GetSection("Settings").Get<MySettings>();
            services.AddDbContext<DataContext>();
            services.AddScoped<IInsuranceService, InsuranceService>();
            services.AddScoped<IInsuranceRepo, InsuranceRepo>();
            services.AddScoped<ISurchargeRateRepo, SurchargeRateRepo>();
            services.AddScoped<IBusinessRules, BusinessRules>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOrderRepo, OrderRepo>();
            services.AddSingleton(mySettings);
            services.AddHttpClient<BusinessRules>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Coolblue API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = "swagger";
                });

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
