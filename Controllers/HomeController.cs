using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Models;

namespace Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        try
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Index");
            return View("Error");
        }
    }

    public IActionResult Privacy()
    {
        try
        {
            var username = Request.Cookies["AuthCookie"];

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Privacy");
            return View("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        try
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Error");
            return View("Error");
        }
    }
}
