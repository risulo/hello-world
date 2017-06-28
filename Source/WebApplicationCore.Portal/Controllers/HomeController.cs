using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebApplicationCore.Portal.Services;
using System.Xml;
using System.Reflection;

namespace WebApplicationCore.Portal.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));

        private static readonly NLog.ILogger nlog = NLog.LogManager.GetLogger(nameof(HomeController));

        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Serilog.ILogger _serilogLogger;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, Serilog.ILogger serilogLogger)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _serilogLogger = serilogLogger;
        }

        public IActionResult Index()
        {
            //_serilogLogger.ForContext("User", "Aditya").ForContext("Other", "HomeController").Information("Data Added Successfully");

            //_logger.LogDebug("Index page says hello");
            _logger.LogInformation(1001, "Index page says hello");
            //_logger.LogWarning("Index page says hello");
            //_logger.LogError("Index page says hello");
            //_logger.LogCritical("Index page says hello");

            var parameters = new Dictionary<string, object>();
            var parameter1 = new
            {
                Id = Guid.NewGuid(),
                FirstName = "FN",
                LastName = "LN",
                Address = new
                {
                    AddressLine1 = "Yellow 5",
                    City = "London",
                    PostCode = "A1N L35",
                    Country = "UK"
                }
            };
            parameters.Add("parameter1", JsonConvert.SerializeObject(parameter1, Formatting.Indented));

            var parameter2 = 5;
            parameters.Add("parameter2", parameter2);

            var parameter3 = DateTime.Today;
            parameters.Add("parameter3", parameter3);

            //_logger.LogCritical(new Exception("Test error"), "LogCriticalTest", parameters);

            try
            {
                var sender = new AuthMessageSender();
                sender.SendEmailAsync("", "", "");
            }
            catch (Exception ex)
            {
                XmlDocument log4netConfig = new XmlDocument();
                log4netConfig.Load(System.IO.File.OpenRead("log4net.config"));

                var repo = log4net.LogManager.CreateRepository(
                    Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

                log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
                log.Fatal("LOG4NET Send email error", ex);

                _logger.LogError(0, ex, "Send email error");
                _logger.LogCritical(ex, "Send email error", parameters);

                nlog.Fatal(ex, "NLOG Send email error {0}", parameter1.FirstName);

                var person = new Person { FirstName = "Richrad", LastName = "Castle" };
                var book = new Book { Author = "Dominik Dan", Title = "Basnik" };
                nlog.Fatal<Person, Book>("NLOG Fatal with 2 classes", person, book);
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public class Book
        {
            public string Author { get; set; }

            public string Title { get; set; }
        }
    }
}
