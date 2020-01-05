using Basics.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Basics
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
                .AddIdentity<IdentityUser, IdentityRole>(config =>
                {
                    config.User.RequireUniqueEmail = true;
                    config.Password.RequiredUniqueChars = 0;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services
                .ConfigureApplicationCookie(config =>
                {
                    config.Cookie.Name = "Identity.Cookie";
                    config.LoginPath = "/Admin/Login";
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
                    // options.Audience = "http://localhost:5001/";
                    // options.Authority = "http://localhost:5000/";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    // policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("User", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("User");
                });
            });

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
