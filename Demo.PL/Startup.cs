using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using Demo.PL.MappingProfiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL
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

            services.AddDbContext<MvcAppG02DbContext>(Options =>
            {
                Options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            //services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
            //services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            //services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            //services.AddScoped<IEmployeeRepository , EmployeeRepository>();
            services.AddScoped<IUnitOfWork , UnitOfWork>();
            services.AddAutoMapper(M => M.AddProfile(new EmployeeProfile()));
            services.AddAutoMapper(M => M.AddProfile(new UserProfile()));
            services.AddAutoMapper(M => M.AddProfile(new RoleProfile()));
            services.AddIdentity<ApplicationUser, IdentityRole>( Options =>
            {
                Options.Password.RequireNonAlphanumeric = true; // @ # $ 
                Options.Password.RequireUppercase = true;
                Options.Password.RequireLowercase = true;
                Options.Password.RequireDigit = true;  
            })
                    .AddEntityFrameworkStores<MvcAppG02DbContext>()
                    .AddDefaultTokenProviders();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(Options =>
                {
                    Options.LoginPath = "Account/Login";
                    Options.AccessDeniedPath = "Home/Error";
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
