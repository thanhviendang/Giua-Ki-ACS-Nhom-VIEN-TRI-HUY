using ASC.Web.Data;
using ASC.Web.Models;
using ASC.Web.Web.Configuration;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ASC.Web
{
    public class Startup
    {
        [Obsolete]
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Elcamino Azure Table Identity services.
            services.AddIdentity<ApplicationUser, ApplicationRole>((options) =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddAzureTableStores<ApplicationDbContext>(new Func<IdentityConfiguration>(() =>
            {
                IdentityConfiguration idconfig = new IdentityConfiguration();
                idconfig.TablePrefix = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration: TablePrefix").Value;
                idconfig.StorageConnectionString = Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:StorageConnectionString").Value;
                idconfig.LocationMode = Configuration.GetSection("IdentityAzureTable: IdentityConfiguration: LocationMode").Value;
            return idconfig;
            }))
            .AddDefaultTokenProviders()
            .CreateAzureTablesIfNotExists<ApplicationDbContext>();
            services.AddSingleton<IIdentitySeed, IdentitySeed>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddOptions();
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.Configure<ApplicationSettings>(Configuration.GetSection("AppSettings"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IIdentitySeed storageSeed)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            using (var scope = app.ApplicationServices.CreateScope())
            {
                await storageSeed.Seed(scope.ServiceProvider.GetService<UserManager<ApplicationUser>>(),
                scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>(),
                scope.ServiceProvider.GetService<IOptions<ApplicationSettings>>());
            }
        }
    }
}
