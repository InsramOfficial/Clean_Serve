using Fastfood.Data;
using Fastfood.Models;
using Fastfood.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace Fastfood.Controllers
{
    public class HomeController : Controller
	{
        public IActionResult Index()
        {
            return View();
        }
    }
}