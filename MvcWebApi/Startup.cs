using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cross.Models;
using DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MvcWebApi.Hubs;
using MvcWebApi.Hubs.Abstractions;
using MvcWebApi.Models;
using MvcWebApi.Providers;

namespace MvcWebApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        //private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public Startup(IConfiguration configuration /*Microsoft.AspNetCore.Hosting.IHostingEnvironment env*/)
        {
            Configuration = configuration;
            //_env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // Add Custom Anti forgery
            services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            
            services.AddSignalR();

            // Add Custom Cors
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins("http://localhost:44338","http://localhost:3000") //Note:  The URL must be specified without a trailing slash (/).
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("AccessToken")
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials());

            });

            // Add CustomJwt Bearer
            services.AddAuthorization(options =>
            {
                options.AddPolicy(CustomRoles.Admin, policy => policy.RequireRole(CustomRoles.Admin));
                options.AddPolicy(CustomRoles.User, policy => policy.RequireRole(CustomRoles.User));
                options.AddPolicy(CustomRoles.DeveloperSupport, policy => policy.RequireRole(CustomRoles.DeveloperSupport));
            });

            // Needed for jwt auth.
            services
                .AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["BearerTokens:Issuer"], // site that makes the token
                        ValidateIssuer = false, // TODO: change this to avoid forwarding attacks
                        ValidAudience = Configuration["BearerTokens:Audience"], // site that consumes the token
                        ValidateAudience = false, // TODO: change this to avoid forwarding attacks
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["BearerTokens:Key"])),
                        ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                        ValidateLifetime = true, // validate the expiration
                        ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                    };
                    cfg.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context => Task.CompletedTask,
                        OnTokenValidated = context =>
                        {
                            var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                            return tokenValidatorService.ValidateAsync(context);
                        },
                        // On Message Receive
//                        const string tokenKey = "Authorization";
//                        context.HttpContext.Request.Headers.TryGetValue(tokenKey, out var authorizationToken);
//                        context.Token = authorizationToken;
                        OnMessageReceived = context => Task.CompletedTask,
                        OnChallenge = context => Task.CompletedTask
                    };
                });


            // Add Custom Options
            //services.Configure<BearerTokensOptions>(options => Configuration.GetSection("BearerTokens").Bind(options));
            services.AddOptions<BearerTokensOptions>()
                .Bind(Configuration.GetSection("BearerTokens"))
                .Validate(bearerTokens =>
                {
                    return bearerTokens.AccessTokenExpirationMinutes < bearerTokens.RefreshTokenExpirationMinutes;
                }, "RefreshTokenExpirationMinutes is less than AccessTokenExpirationMinutes. Obtaining new tokens using the refresh token should happen only if the access token has expired.");
            services.AddOptions<ApiSettings>()
                .Bind(Configuration.GetSection("ApiSettings"));

            services.AddOptions<EmailSettings>().Bind(Configuration.GetSection("EmailSettings"));

            // Dependency Injection
            var connectionString = Configuration.GetConnectionString("Transport_Db");
            services.AddCrossService();
            services.AddDataService(connectionString);
            services.AddBusinessLogicService();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IAntiForgeryCookieService, AntiForgeryCookieService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<ITokenValidatorService, TokenValidatorService>();
            services.AddScoped<ITokenFactoryService, TokenFactoryService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPanelHub, PanelHub>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    if (error?.Error is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            State = 401,
                            Msg = "token expired"
                        }));
                    }
                    else if (error?.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            State = 500,
                            Msg = error.Error.Message
                        }));
                    }
                    else
                    {
                        await next();
                    }
                });
            });

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<PanelHub>("/panelhub");
            });
        }
    }
}
