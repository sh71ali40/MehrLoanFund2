using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using DataLayer.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Services.Components;
using Services.Mapper;
using Services.Services;

namespace MehrLoanFund2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {


            #region Localization
            services.AddControllers();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                // define the list of cultures your app will support
                var supportedCultures = new List<CultureInfo>()
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("fa-IR")
                };

                // set the default culture
                options.DefaultRequestCulture = new RequestCulture("fa-IR");

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            #endregion Localization

            services.AddRazorPages();
            services.AddDbContext<MehrLoanFundContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddTelerikBlazor();
            services.AddMudServices();


            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            #region AddAppInterfaces

            services.AddScoped<IUnitOfWork, MehrLoanFundContext>();
            services.AddScoped<IUserServcie, UserService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IInstallmentService, InstallmentService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            
            services.AddScoped<LoadingService>();
            services.AddScoped<ToastService>();
            services.AddScoped<ConfirmService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            #endregion AddAppInterfaces


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddServerSideBlazor();
             
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

            #region Localization

            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()?.Value);

            #endregion
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
