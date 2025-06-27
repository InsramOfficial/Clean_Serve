using AspNetCore.Reporting;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using Fastfood.Data;
using Fastfood.Models;
using Fastfood.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Fastfood.Controllers
{
    public class SalesController : Controller
    {
        private readonly IWebHostEnvironment env;
        private readonly DataDbContext db;
        public SalesController(DataDbContext _db, IWebHostEnvironment _env)
        {
            db = _db;
            env = _env;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        }
        public IActionResult Index()
        {
            var clients = db.clients.ToList();
            var categories = db.categories.ToList();
            List<Category> list = new List<Category>();
            List<SaledItems> DynamicData = new List<SaledItems>();
            List<Client> _clients = new List<Client>();
            _clients = clients;
            list = categories;
            CategoryItemVM categoryItemViewModel = new CategoryItemVM();
            categoryItemViewModel.category = list;
            categoryItemViewModel.DynamicData = DynamicData;
            categoryItemViewModel.clients = _clients;
            //var model = new CategoryItemViewModel
            //{
            //    DynamicData = new List<CartItems>()
            //};
            return View(categoryItemViewModel);
        }

        public IActionResult FilterItems(int categoryId)
        {
            var _items = db.items;
            IEnumerable<Item> data = _items.Where(item => item.CategoryId == categoryId).ToList();

            return Json(data);
        }

        public IActionResult FilterItemsByItemId(int itemId)
        {
            // Filter items based on the itemId
            var _items = db.items;

            var filteredItems = _items.Where(item => item.ItemId == itemId).ToList();

            // Return filtered items as JSON data
            return Json(filteredItems);
        }

        public IActionResult FilterItemsByRemarks(string remarks)
        {
            // Filter items based on the itemId
            var _items = db.items;
            //var filteredItems = _items.Where(item => item.ItemId == itemId).ToList();
            var filteredPizzaRemarks = _items.Where(item => item.Remarks == remarks).ToList();

            //var filteredItems = 1;
            // Return filtered items as JSON data
            return Json(filteredPizzaRemarks);
        }

        [HttpPost]
        public IActionResult DynamicalData(CategoryItemVM items , string clientId)
        {
            Sales sale = new();
            sale.SaleDate = System.DateTime.Now;
            sale.Payment = items.FinalBillTotal;
            sale.Status = items.PaymentMethod;
            sale.Cash_Received = items.CashReceived;
            sale.Paid_Back = items.CashPayBack;
            sale.Serving = items.DeliveryMethod;
            db.sales.Add(sale);
            db.SaveChanges();
            int lastRecordId = db.sales
                                     .OrderBy(e => e.SaleId)
                                     .Select(e => e.SaleId)
                                     .LastOrDefault();
            foreach (var item in items.DynamicData)
            {
                SoldItems saleditem = new();
                saleditem.SaleId = lastRecordId;
                saleditem.ItemId = int.Parse(item.ItemId);
                saleditem.ItemName = item.ItemName;
                saleditem.Qty = int.Parse(item.Quantity);
                saleditem.UnitPrice = int.Parse(item.Price);
                saleditem.Discount = int.Parse(item.Discount);
                saleditem.NetPrice = item.NetTotal;
                db.soldItems.Add(saleditem);
                db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult BankDetail(int Bin)
        {
            var banksattlement = db.bankSattlements;

            var specificbankdetail = banksattlement.Where(x => x.BIN == Bin).FirstOrDefault();

            return Json(specificbankdetail);
        }
        [HttpPost]
        public IActionResult CreateCustomer(string CustomerName)
        {
            decimal lastRecordId = db.clients
                                     .OrderBy(e => e.Clientid)
                                     .Select(e => e.Clientid)
                                     .LastOrDefault();
            Client newcustomer = new();
            newcustomer.Clientid = lastRecordId + 1;
            newcustomer.Name = CustomerName;
            db.clients.Add(newcustomer);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult BillsHistory()
        {
            //var InvoiceHistory = db.sales.ToList();

            var last500Records = db.sales
                                .OrderByDescending(e => e.SaleId)
                                .Take(500)
                                .ToList();
            return View(last500Records);
        }

        public IActionResult EditBill(int Id)
        {
            CategoryItemVM catitem = new();
            var categories = db.categories.ToList();
            List<Category> list = new List<Category>();
            List<SaledItems> DynamicData = new List<SaledItems>();
            list = categories;
            catitem.category = list;
            //catitem.DynamicData = DynamicData;

            var sales = db.sales.FirstOrDefault(r => r.SaleId ==  Id);
            if (sales != null)
            {
                catitem.PaymentMethod = sales.Status;
                catitem.DeliveryMethod = sales.Serving;
                catitem.FinalBillTotal = sales.Payment;
                catitem.CashReceived = sales.Cash_Received; 
                catitem.CashPayBack = sales.Paid_Back;
            }

            var solditems = db.soldItems.Where(e => e.SaleId == Id).ToList();

            foreach (var item in solditems)
            {
                SaledItems sold = new();
                sold.ItemId = item.ItemId.ToString();
                sold.ItemName = item.ItemName;
                sold.Price = item.UnitPrice.ToString();
                sold.Quantity = item.Qty.ToString();
                sold.Discount = item.Discount.ToString();
                sold.NetTotal = item.NetPrice;

                DynamicData.Add(sold);
            }

            catitem.DynamicData = DynamicData;
            catitem.IDforUpdateRecord = Id;

            return View(catitem);
        }
        [HttpPost]
        public IActionResult UpdateBill(CategoryItemVM editbill)
        {
            var RecordtoUpdate = editbill.IDforUpdateRecord;

            Sales sale = new();
            sale.SaleId = (int)RecordtoUpdate;
            sale.LastModified = System.DateTime.Now;
            sale.Payment = editbill.FinalBillTotal;
            sale.Status = editbill.PaymentMethod;
            sale.Cash_Received = editbill.CashReceived;
            sale.Paid_Back = editbill.CashPayBack;
            sale.Serving = editbill.DeliveryMethod;
            db.sales.Update(sale);
            db.SaveChanges();

            var recordstodelete = db.soldItems.Where(e => e.SaleId == RecordtoUpdate).ToList();

            db.soldItems.RemoveRange(recordstodelete);
            db.SaveChanges();

            //int lastRecordId = db.sales
            //                         .OrderBy(e => e.SaleId)
            //                         .Select(e => e.SaleId)
            //                         .LastOrDefault();
            foreach (var item in editbill.DynamicData)
            {
                SoldItems saleditem = new();
                saleditem.SaleId = (int)RecordtoUpdate;
                saleditem.ItemId = int.Parse(item.ItemId);
                saleditem.ItemName = item.ItemName;
                saleditem.Qty = int.Parse(item.Quantity);
                saleditem.UnitPrice = int.Parse(item.Price);
                saleditem.Discount = int.Parse(item.Discount);
                saleditem.NetPrice = item.NetTotal;
                db.soldItems.Add(saleditem);
                db.SaveChanges();
            }

            //var lastrecordtoupdate = db.sales.OrderBy(e => e.SaleId).Select(e => e.SaleId).LastOrDefault();
            return RedirectToAction(nameof(BillsHistory));
        }
        public IActionResult Print(int Id)
        {
            var sales = db.soldItems.Where(i => i.SaleId == Id).ToList();
            var InvoiceNo = Id.ToString();

            string mimType = "";
            int extension = 1;
            var path = $"{this.env.WebRootPath}/Reports/Report.rdlc";
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("Parameter1", "Welcome to the Sales Invoice Receipt");
            parameter.Add("InvoiceNo", InvoiceNo);
            parameter.Add("InvoiceName", "InvoiceNo");
            LocalReport localreport = new LocalReport(path);
            localreport.AddDataSource("DataSet", sales);
            var result = localreport.Execute(RenderType.Pdf, extension, parameter, mimType);
            return File(result.MainStream, "application/pdf");
        }
    }
}
