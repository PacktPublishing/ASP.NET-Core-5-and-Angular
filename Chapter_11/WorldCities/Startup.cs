using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using WorldCities.Data;
using WorldCities.Data.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Identity.UI.Services;
using WorldCities.Services;
using System;
using Serilog;

namespace WorldCities
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
            services.AddControllersWithViews()
                .AddJsonOptions(options => {
            // set this option to TRUE to indent the JSON output
            options.JsonSerializerOptions.WriteIndented = true;
            // set this option to NULL to use PascalCase instead of CamelCase (default)
            // options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });


            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // Add ApplicationDbContext and SQL Server support
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                    )
            );

            // Add ASP.NET Core Identity support
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            // IEmailSender implementation using SendGrid
            // (disabled to avoid conflicts with MailKit-based implementation)
            /*
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.Configure<SendGridEmailSenderOptions>(options =>
            {
                options.ApiKey = Configuration["ExternalProviders:SendGrid:ApiKey"];
                options.SenderEmail = Configuration["ExternalProviders:SendGrid:Sender_Email"];
                options.SenderName = Configuration["ExternalProviders:SendGrid:Sender_Name"];
            });
            */

            // IEmailSender implementation using MailKit
            // (disable it if you want to use the SendGrid-based implementation instead)
            services.AddTransient<IEmailSender, MailKitEmailSender>();
            services.Configure<MailKitEmailSenderOptions>(options =>
            {
                options.Host_Address = Configuration["ExternalProviders:MailKit:SMTP:Address"];
                options.Host_Port = Convert.ToInt32(Configuration["ExternalProviders:MailKit:SMTP:Port"]);
                options.Host_Username = Configuration["ExternalProviders:MailKit:SMTP:Account"];
                options.Host_Password = Configuration["ExternalProviders:MailKit:SMTP:Password"];
                options.Sender_EMail = Configuration["ExternalProviders:MailKit:SMTP:SenderEmail"];
                options.Sender_Name = Configuration["ExternalProviders:MailKit:SMTP:SenderName"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // add .webmanifest MIME-type support
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = (context) =>
                {
                    switch (context.File.Name)
                    {
                        // disable caching for these specific files
                        case "isOnline.txt":
                            context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                            context.Context.Response.Headers.Add("Expires", "-1");
                            break;
                        // use default rules (from appsettings.json) for all other files
                        default:
                            context.Context.Response.Headers["Cache-Control"] =
                                Configuration["StaticFiles:Headers:Cache-Control"];
                            break;
                    }
                }
            });

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            // Use the Serilog request logging middleware to log HTTP requests.
            app.UseSerilogRequestLogging();
        }
    }
}
