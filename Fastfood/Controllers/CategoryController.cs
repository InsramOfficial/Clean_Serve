using Fastfood.Data;
using Fastfood.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fastfood.Controllers
{
    public class CategoryController : Controller
    {
		private readonly DataDbContext db;
		public CategoryController(DataDbContext _db)
		{
			db = _db;
		}
        public IActionResult Index()
        {
            var Sale = db.sales;
            return View(Sale);
        }
    }
}
