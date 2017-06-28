using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace WebApplicationCore.Portal.Controllers
{
    public class HomeController : Controller
    {
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
            _serilogLogger.ForContext("User", "Aditya").ForContext("Other", "HomeController").Information("Data Added Successfully");

            _logger.LogDebug("Index page says hello");
            _logger.LogInformation(1001, "Index page says hello");
            _logger.LogWarning("Index page says hello");
            _logger.LogError("Index page says hello");
            _logger.LogCritical("Index page says hello");
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
    }
}
