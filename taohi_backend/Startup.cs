using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using taohi_backend.Services;
using taohi_backend.Interfaces;
using taohi_backend.Data;
using taohi_backend.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using taohi_backend.PolicyHandlers;

namespace taohi_backend
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration config) => _config = config;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>
                    (options => options.UseMySql(_config["MySql:ConnectionString"]));

            services
                .AddIdentity<User, UserRole>(config =>
                {
                    config.User.RequireUniqueEmail = true;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredLength = 5;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireDigit = false;
                    config.Password.RequiredUniqueChars = 0;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services
                .ConfigureApplicationCookie(config =>
                {
                    // Cookie settings
                    config.Cookie.HttpOnly = true;
                    config.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    config.Cookie.Name = "Identity.Cookie";

                    config.LoginPath = "/Admin/Login";
                    config.AccessDeniedPath = "/Admin/Login";
                    config.SlidingExpiration = true;
                });

            services
                .AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.Events = new JwtBearerEvents()
                    {
                        // for authorizing tokens from query instead of body
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            // var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken)) // && path.StartsWithSegments("/Messages/Hub")
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _config["JwtAuthentication:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = _config["JwtAuthentication:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtAuthentication:Secret"]))
                    };
                });

            services.AddAuthorization(options =>
            {
                // roles
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("Moderator", policy =>
                {
                    policy.AddAuthenticationSchemes("OAuth");
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(new[] { "Admin", "Moderator" });
                });
                options.AddPolicy("User", policy =>
                {
                    policy.AddAuthenticationSchemes("OAuth");
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(new[] { "Admin", "User" });
                });
                // claims
                options.AddPolicy("IsActive", policy =>
                {
                    policy.RequireClaim("IsActive", new[] { "True", "true" });
                });

                //options.AddPolicy("usertype", policy =>
                //    policy.RequireAssertion(claim =>
                //        claim.User.HasClaim("UserType", "Taohi") ||
                //        claim.User.HasClaim("UserType", "Rangatahi")));

                // custom claims
                options.AddPolicy("Rangatahi", policy =>
                    policy.Requirements.Add(new AgeClaimRequirement(11, 31)));
                options.AddPolicy("Taohi", policy =>
                    policy.Requirements.Add(new AgeClaimRequirement(6, 10)));
            });

            services.AddSingleton<IAuthorizationHandler, AgeClaimHandler>();
            services.AddScoped<IUsersService, UserService>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddCors();
            services
                .AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            UserManager<User> userManager,
            RoleManager<UserRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Admin/Error");
                app.UseHsts();
            }

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseStaticFiles();
            app.UseRouting();

            // who are you?
            app.UseAuthentication();
            // are you allowed?
            app.UseAuthorization();

            UserDataSeeder.SeedData(userManager, roleManager);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Admin", action = "Index" }
                );
            });
        }
    }
}
