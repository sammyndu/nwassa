using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Nwassa.Core.Accounts;
using Nwassa.Core.Data;
using Nwassa.Core.Emails;
using Nwassa.Core.Files;
using Nwassa.Core.Helpers;
using Nwassa.Core.Products;
using Nwassa.Core.Purchases;
using Nwassa.Core.Users;
using Nwassa.Data;
using Nwassa.Data.Models;
using Nwassa.Data.Repositories;
using Nwassa.Domain_Services;
using Nwassa.Domain_Services.Account;
using Nwassa.Domain_Services.Products;
using Nwassa.Domain_Services.Users;
using Nwassa.Presentation.Models.Errors;
using static Nwassa.Data.Models.NwassaDatabaseSettings;

namespace Nwassa
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:4200");
                                      builder.AllowAnyHeader();
                                      builder.AllowAnyMethod();
                                  });
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var notificationMetadata = Configuration.GetSection("NotificationMetadata").
             Get<NotificationMetadata>();
            services.AddSingleton(notificationMetadata);

            var cloudinary = Configuration.GetSection("Cloudinary").
            Get<CloudinaryMetaData>();
            services.AddSingleton(cloudinary);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.Configure<NwassaDatabaseSettings>(
                Configuration.GetSection(nameof(NwassaDatabaseSettings)));

            services.AddSingleton<INwassaDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<NwassaDatabaseSettings>>().Value);

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddHttpContextAccessor();

            services.AddMvc();

            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ICrypto, Crypto>();

            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IPurchaseRepository, PurchaseRepository>();

            services.AddScoped<IPurchaseService, PurchaseService>();

            services.AddScoped<IEmailGenerator, EmailGenerator>();

            services.AddSingleton<IDataRepository, DataRepository>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
            }
            

            app.UseHttpsRedirection();

            app.UseExceptionHandler(builder => builder.HandleExceptions());

            //app.UseExceptionHandler(a => a.Run(async context =>
            //{
            //    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            //    var exception = exceptionHandlerPathFeature.Error;

            //    var result = JsonConvert.SerializeObject(new { error = exception.Message });
            //    context.Response.ContentType = "application/json";
            //    await context.Response.WriteAsync(result);
            //}));

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
