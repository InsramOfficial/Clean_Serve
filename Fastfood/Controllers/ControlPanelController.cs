using Fastfood.Data;
using Fastfood.Models;
using Fastfood.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fastfood.Controllers
{
    public class ControlPanelController : Controller
    {
        private readonly DataDbContext db;
        public ControlPanelController(DataDbContext _db)
        {
            db = _db;
            
        }
        #region Category
        // GET: ControlPanelController
        [HttpGet]
        public IActionResult Index()
        {
            var categories = db.categories.ToList();
            return View(categories);
        }

        // GET: ControlPanelController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: ControlPanelController/Create
        
        public IActionResult Create()
        {
            return View();
        }

        // POST: ControlPanelController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category newcat)
        {
            try
            {
                Category category = new();
                category.CategoryName = newcat.CategoryName;
                category.Root = 0;
                db.categories.Add(category);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ControlPanelController/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = db.categories.Find(id);

            return View(category);
        }

        // POST: ControlPanelController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            try
            {
                Category cat = new();
                cat.CategoryId = category.CategoryId;
                cat.CategoryName = category.CategoryName;
                cat.Root = 0;
                db.categories.Update(cat);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public IActionResult Delete(int id)
        {
           
            var RemoveCategory = db.categories.Find(id);
            db.categories.Remove(RemoveCategory);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
            
        }
        #endregion

        #region Items

        public IActionResult ItemsDetail()
        {
            var items = db.items.ToList();

            return View(items);
        }
        [HttpGet]
        public IActionResult CreateItem()
        {
            ItemsVM items = new();
            var category = db.categories.ToList();
            items.category = category;
            return View(items);
        }
        [HttpPost]
        public IActionResult CreateItem(ItemsVM item)
        {
            Item addItem = new();
            addItem.ItemName = item.ItemName;
            addItem.RecentUnitPrice = item.RecentUnitPrice;
            addItem.Discount = item.Discount;
            addItem.CategoryId = item.CategoryId;
            addItem.Remarks = item.Remarks;
            db.items.Add(addItem);
            db.SaveChanges();
            return RedirectToAction(nameof(ItemsDetail));
        }
        [HttpGet]
        public IActionResult EditItem(int id)
        {
            var item = db.items.Find(id);
            ItemsVM items = new();
            items.ItemId = id;
            items.ItemName = item.ItemName;
            items.RecentUnitPrice = item.RecentUnitPrice;
            items.Discount = item.Discount;
            items.CategoryId = item.CategoryId;
            items.Remarks = item.Remarks;
            var categories = db.categories.ToList();
            items.category = categories;
            return View(items);

        }
        [HttpPost]
        public IActionResult EditItem(ItemsVM item)
        {
            Item updateitem = new();
            updateitem.ItemId = (int)item.ItemId;
            updateitem.ItemName = item.ItemName;
            updateitem.RecentUnitPrice = item.RecentUnitPrice;
            updateitem.Discount = item.Discount;
            updateitem.CategoryId = item.CategoryId;
            updateitem.Remarks = item.Remarks;
            db.items.Update(updateitem);
            db.SaveChanges();
            return RedirectToAction(nameof(ItemsDetail));
        }
        public IActionResult DeleteItem(int id)
        {
            var deleteitem = db.items.Find(id);
            db.items.Remove(deleteitem);
            db.SaveChanges();
            return RedirectToAction(nameof(ItemsDetail));
        }
        #endregion
    }
}
