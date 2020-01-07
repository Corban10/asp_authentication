using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
                    config.AccessDeniedPath = "/Admin/Error";
                    config.SlidingExpiration = true;
                });

            services
                .AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

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
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("Moderator", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireRole(new[] { "Admin", "Moderator" });
                });
                options.AddPolicy("User", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(new[] { "Admin", "Moderator", "User" });
                });
                // options.AddPolicy("AdminOrUser", policy =>
                // {
                //     policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                //     policy.RequireAuthenticatedUser();
                //     policy.RequireAssertion(context =>
                //         context.User.IsInRole("Admin") ||
                //         context.User.IsInRole("User"));
                // });
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddCors();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
