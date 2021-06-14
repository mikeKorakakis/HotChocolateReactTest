// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using API;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfig { get; private set; }
        public static IWebHostEnvironment StaticEnv { get; private set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
            StaticConfig = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            services.AddDbContext<DataContext>(options =>
             options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));

            services.AddIdentity<AppUser, IdentityRole>(options =>
         {
             options.SignIn.RequireConfirmedAccount = false;
         }).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

            var SpaClientUri = Configuration["SpaClientUri"];
             services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("WWW-Authenticate")
                        .WithOrigins(SpaClientUri);

                });
            });

         
            var builder = services.AddIdentityServer()
                .AddDeveloperSigningCredential()        //This is for dev only scenarios when you don’t have a certificate to use.
                .AddInMemoryClients(Config.Clients())
                .AddInMemoryApiScopes(Config.ApiScopes())
                .AddInMemoryApiResources(Config.Apis())
                .AddInMemoryIdentityResources(Config.IdentityResources())
                .AddAspNetIdentity<AppUser>();
            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            // services.AddTransient<IEndpointRouter, CustomEndpointRouter>();

        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
