using Electric_Scooter.Helpers;
using Electric_Scooter.Models;
using Electric_Scooter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Electric_Scooter.Controllers
{
    [IsLogin]
    public class CustomerController : Controller
    {
        private IRepository<Customer> _rep;

        public CustomerController()
        {
            _rep = new Repository<Customer>(new I_ChiEntities());
        }

        public ActionResult Index(string searchTerm, string city, string sortField, string sortOrder = "asc")
        {
            var data = _rep.GetAllData();

            //關鍵字
            if (!string.IsNullOrEmpty(searchTerm))
            {
                data = data.Where(x => (x.c_Name != null && x.c_Name.Contains(searchTerm)) ||
                                       (x.c_Phone1 != null && x.c_Phone1.Contains(searchTerm)) ||
                                       (x.c_Phone2 != null && x.c_Phone2.Contains(searchTerm)));
            }

            //城市
            if (!string.IsNullOrEmpty(city))
                data = data.Where(x => x.c_City == city);

            // 排序
            switch (sortField)
            {
                case "CreateDate":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.c_CreateDate) : data.OrderByDescending(x => x.c_CreateDate);
                    break;
                case "Gender":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.c_Gender) : data.OrderByDescending(x => x.c_Gender);
                    break;
                case "Birthday":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.c_Birthday) : data.OrderByDescending(x => x.c_Birthday);
                    break;
                default:
                    data = data.OrderBy(x => x.c_Id);
                    break;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.SortField = sortField;

            //暫存資料供"匯出"、"列印"使用
            Session["ExportData"] = data.ToList();

            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            customer.c_CreateDate = DateTime.Now;
            customer.c_No = GenerateCusNo();
            _rep.Add(customer);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var data = _rep.GetDataById(id);

            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            _rep.Update(customer);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var data = _rep.GetDataById(id);

            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _rep.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult ExportToPDF()
        {
            // 取得資料
            var data = Session["ExportData"] as List<Customer>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無匯出資料");
            }

            // 生成PDF
            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf", "CustomerList.pdf");
        }

        public ActionResult Print()
        {
            var data = Session["ExportData"] as List<Customer>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無列印資料");
            }

            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf");
        }

        protected override void Dispose(bool disposing)
        {
            _rep.Dispose();
            base.Dispose(disposing);
        }

        private string GenerateCusNo()
        {
            var maxId = _rep.GetAllData().Max(x => x.c_Id);
            var newId = maxId + 1;

            return "A" + newId.ToString("D5");
        }
    }
}