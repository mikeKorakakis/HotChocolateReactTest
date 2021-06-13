using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HotChocolate.AspNetCore;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace API
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
            services.AddPooledDbContextFactory<DataContext>(opt =>
          {
              opt.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));
          });

            services.AddScoped<DataContext>(p =>
                    p.GetRequiredService<IDbContextFactory<DataContext>>().CreateDbContext());
            var builder = services.AddIdentityCore<AppUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();

            var SpaClientUri = Configuration["SpaClientUri"];
            services.AddCors(opt =>
           {
               opt.AddPolicy("CorsPolicy", policy =>
               {
                   policy.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .WithExposedHeaders("WWW-Authenticate")
                       .WithOrigins(SpaClientUri);

               });
           });

            var Handler = new HttpClientHandler();
            Handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            services.AddAuthentication(options =>
           {
               options.DefaultScheme = "Bearer";

               options.DefaultAuthenticateScheme = "Bearer";
           })
               .AddIdentityServerAuthentication("Bearer", options =>
               {
                   options.ForwardDefaultSelector = context =>
                   {
                       if (!context.Items.ContainsKey(AuthenticationSocketInterceptor.HTTP_CONTEXT_WEBSOCKET_AUTH_KEY) &&
                           context.Request.Headers.TryGetValue("Upgrade", out var value) &&
                           value.Count > 0 &&
                           value[0] is string stringValue &&
                           stringValue == "websocket")
                       {
                           return "Websockets";
                       }
                       return "Bearer";
                   };
                   options.TokenRetriever = new Func<HttpRequest, string>(req =>
                   {
                       if (req.HttpContext.Items.TryGetValue(
                               AuthenticationSocketInterceptor.HTTP_CONTEXT_WEBSOCKET_AUTH_KEY,
                               out object token) &&
                           token is string stringToken)
                       {
                           return stringToken;
                       }
                       var fromHeader = TokenRetrieval.FromAuthorizationHeader();
                       var fromQuery = TokenRetrieval.FromQueryString(); // Query string auth
                       return fromHeader(req) ?? fromQuery(req);
                   });
                   options.Authority = "https://localhost:5001";
                   options.RequireHttpsMetadata = false;
                   options.ApiName = "api1";
                   options.JwtBackChannelHandler = Handler;

               }).AddJwtBearer("Websockets", ctx => { });

            IdentityModelEventSource.ShowPII = true;
            services.AddSingleton<ISocketSessionInterceptor, AuthenticationSocketInterceptor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserAccessor, UserAccessor>();

            services
            .AddGraphQLServer()
              .AddQueryType(d => d.Name("Query"))
                 .AddTypeExtension<UserQueries>()
                 .AddTypeExtension<MessageQueries>()
              .AddMutationType(d => d.Name("Mutation"))
                 .AddTypeExtension<MessageMutations>()
               .AddSubscriptionType(d => d.Name("Subscription"))
                 .AddTypeExtension<MessageSubscriptions>()
               .AddDataLoader<UserByIdDataLoader>()
              .AddInMemorySubscriptions()
               .AddAuthorization()
            //   .EnableRelaySupport()
              .AddSocketSessionInterceptor<AuthenticationSocketInterceptor>();


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
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
        }
    }
}
