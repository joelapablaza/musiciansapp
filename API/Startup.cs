using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly string _MyCors = "MyCors";
        private readonly IConfiguration _configiguration;

        public Startup(IConfiguration configiguration)
        {
            _configiguration = configiguration;

        }

        // public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(_configiguration.GetConnectionString("Default"));
            });
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddControllers(); // autogenerado
            services.AddCors(options =>

            {
                options.AddPolicy(name: _MyCors, builder =>
                {
                    //for when you're running on localhost
                    builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyHeader().AllowAnyMethod();


                    //builder.WithOrigins("url from where you're trying to do the requests")
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_MyCors);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
