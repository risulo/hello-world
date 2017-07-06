using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Serilog;
using Serilog.Events;
using Serilog.Models;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using WebApplicationCore.Portal.Configuration;
using WebApplicationCore.Portal.Data;
using WebApplicationCore.Portal.Log4Net;
using WebApplicationCore.Portal.Models;
using WebApplicationCore.Portal.Services;

namespace WebApplicationCore.Portal
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var connectionBuilder = new ConfigurationBuilder();
            connectionBuilder.SetBasePath(Directory.GetCurrentDirectory());
            connectionBuilder.AddJsonFile("appsettings.json");
            var connectionStringConfig = connectionBuilder.Build();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEntityFrameworkConfig(connectionStringConfig["ContainerName"], options =>
                                options.UseSqlServer(connectionStringConfig.GetConnectionString("DefaultConnection")));

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddOptions();
            services.Configure<CryptographyCertificate>(Configuration.GetSection("CryptographyCertificate"));
            services.Configure<AppOptions>(Configuration.GetSection("app"));

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfiguration>(Configuration);

            //var columnOptions = new ColumnOptions
            //{
            //    AdditionalDataColumns = new Collection<DataColumn>
            //    {
            //        new DataColumn {DataType = typeof (string), ColumnName = "User", AllowDBNull = true},
            //        new DataColumn {DataType = typeof (string), ColumnName = "Other", AllowDBNull = true},
            //    }
            //};
            //columnOptions.Store.Add(StandardColumn.LogEvent);

            var x = Configuration["Serilog:ConnectionString"];
            var y = Configuration["Serilog:TableName"];

            var key1 = Configuration["key1"];

            var cryptographyCertificate = new CryptographyCertificate();
            Configuration.GetSection("CryptographyCertificate").Bind(cryptographyCertificate);
            var findValue = cryptographyCertificate.FindValue;

            var appOptions = Configuration.GetSection("app").Get<AppOptions>();
            var height = appOptions.Window.Height;

            var cryptographyCertificate2 = Configuration.GetSection("CryptographyCertificate").Get<CryptographyCertificate>();
            var findValue2 = cryptographyCertificate2.FindValue;

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Error)
                            .WriteTo.RollingFile(@"C:\temp\WebApplicationCore.Portal-Serilog-{Date}.txt")
                            .WriteTo.MSSqlServer(Configuration["Serilog:ConnectionString"]
                                , Configuration["Serilog:TableName"]
                                , LogEventLevel.Debug
                                //, columnOptions: columnOptions
                                , autoCreateSqlTable: true)
                            .CreateLogger();

            services.AddSingleton<Serilog.ILogger>(Log.Logger);

            //services.AddSingleton<Serilog.ILogger>
            //(x => new LoggerConfiguration()
            //      .MinimumLevel.Debug()
            //      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //      .MinimumLevel.Override("System", LogEventLevel.Error)
            //      .WriteTo.RollingFile(@"C:\temp\WebApplicationCore.Portal-Serilog-{Date}.txt")
            //      .CreateLogger());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            var envName = env.EnvironmentName;

            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();
            NLog.GlobalDiagnosticsContext.Set("pid", Process.GetCurrentProcess().Id);
            NLog.GlobalDiagnosticsContext.Set("TickCount", MdcTickProperty.Default);
            //NLog.MappedDiagnosticsContext.Set("TickCount", MdcTickProperty.Default);

            //add NLog.Web
            app.AddNLogWeb();
            
            log4net.GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            loggerFactory.AddLog4Net();

            //loggerFactory.AddLog4Net_Custom();

            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            env.ConfigureNLog("nlog.config");
        }
    }
}
