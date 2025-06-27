using AspNetCore.Reporting;
using Fastfood.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace Fastfood.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment env;
        public ReportController(IWebHostEnvironment _env)
        {
            env = _env;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Print()
        {
            var datasource = new List<Student>()
            {
                 new Student{StudentId = "1",StudentName= "Naseer"},
                 new Student{StudentId = "2",StudentName= "Insraam"},
                 new Student{StudentId = "3",StudentName= "Naseer"},
                 new Student{StudentId = "4",StudentName= "Insraam"},
                 new Student{StudentId = "5",StudentName= "Zaheer"}
            };

            string mimType = "";
            int extension = 1;
            var path = $"{this.env.WebRootPath}/Reports/Report.rdlc";
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("Parameter1", "Welcome to the coding World");
            LocalReport localreport = new LocalReport(path);
            localreport.AddDataSource("DataSet", datasource);
            var result = localreport.Execute(RenderType.Pdf, extension, parameter, mimType);
            return File(result.MainStream, "application/pdf");
        }
    }
}
