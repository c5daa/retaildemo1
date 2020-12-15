// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Contoso.Retail.NextGen.UserProfile.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string myPolicy = "AllowAnyOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: myPolicy,
                    builder =>
                                {
                                    builder.AllowAnyOrigin();
                                    builder.AllowAnyHeader();
                                    builder.AllowAnyMethod();
                                });
            });

            services.AddControllers();

            //Register swagger gen
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contoso Retail's User Profile Service API Endpoint", Version = "v1" });

                c.CustomOperationIds(d => (d.ActionDescriptor as ControllerActionDescriptor)?.ActionName);
            });

            services.AddSwaggerGenNewtonsoftSupport();



            services.AddTransient<IUserProfileManager, UserProfileManager>(c =>
                    {
                        return new UserProfileManager(Configuration["Values:DBConnectionString"], "Users");
                    }
            );


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(myPolicy);

            app.UseMiddleware(typeof(Contoso.HttpHost.Moddleware.Exception.ExceptionHandler));

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {

                if (env.IsDevelopment())
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contoso Retail's User Profile Service API V1");
                }
                else
                {
                    //for fontdoor service
                    c.SwaggerEndpoint("./v1/swagger.json", "Contoso Retail's User Profile Service API V1");
                    // c.RoutePrefix = "profile";
                }

            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });




        }
    }
}
