using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
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
            services.AddControllersWithViews();
            /*this is the method where we introduce authentication and authorization 
             *services 
             */
            /*
             * for our project we use authentication with cookie scheme
             */
            services.AddAuthentication(options=>
            {
                //these 2 services have to set for google accounts
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/denied";
                    options.LoginPath = "/login";
                    options.Events = new CookieAuthenticationEvents()
                    {
                        OnSignedIn = async context =>
                        {//calls on singin process
                            await Task.CompletedTask;
                        },
                        OnSigningOut = async context =>
                        { //
                            await Task.CompletedTask;
                        },
                        OnValidatePrincipal = async context =>
                        {//on validate principal-->it fetch cookie(inside of which is ticket) and check 
                         //whether there is cookie which is expired or not.
                            await Task.CompletedTask;
                        }
                    };
                })
                .AddGoogle(options =>
                {
                    //to add google auth service (have to provide secret,client id)
                    //normally we put this in user config file and use configuration manager to get
                    //these value backup but for simplicity we directly copy paste it.

                    //these 2 will be provided by google developer account once you setup your app for auth service
                    options.ClientId = "431349360298-02i52k29715rm93hr9lg4rq8enu6ubb4.apps.googleusercontent.com";
                    options.ClientSecret = "BmD_bApJdOVvPlazeh81uEH_"; 
                    
                    options.CallbackPath = "/auth";  //calls after auth  is made by google 
                    
                    /*
                     * This will allow google user to select different google account,else it will
                     * automatically pick current logged in google account as your default account
                     * to enter this app
                     */
                    options.AuthorizationEndpoint += "?prompt=consent";
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();//to run authentication scheme
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
