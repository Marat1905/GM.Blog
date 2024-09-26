using GM.Blog.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GM.Blog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        /// <summary>�������� �������� </summary>
        public IActionResult Index(string? returnUrl)
        {
            if (User.Identity!.IsAuthenticated)
            {
                if (returnUrl != null && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("GetPosts", "Post");
            }
            return View();
        }

        /// <summary> �������� �������� ������������������</summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>�������� ������ �� ������ ��� �������������</summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
