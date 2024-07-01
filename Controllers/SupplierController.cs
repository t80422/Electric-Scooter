using Electric_Scooter.Models;
using Electric_Scooter.Repositories;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Electric_Scooter.Helpers;

namespace Electric_Scooter.Controllers
{
    [IsLogin]
    public class SupplierController : Controller
    {
        // GET: Supplier
        private IRepository<Supplier> _rep;

        public SupplierController()
        {
            _rep = new Repository<Supplier>(new I_ChiEntities());
        }

        public ActionResult Index(string searchTerm, string city, string sortField, string sortOrder = "asc")
        {
            var data = _rep.GetAllData();

            #region 搜尋
            //關鍵字
            if (!string.IsNullOrEmpty(searchTerm))
            {
                data = data.Where(x => (x.s_Name != null && x.s_Name.Contains(searchTerm)) ||
                                       (x.s_InchargePerson != null && x.s_InchargePerson.Contains(searchTerm)) ||
                                       (x.s_Phone1 != null && x.s_Phone1.Contains(searchTerm)) ||
                                       (x.s_Phone2 != null && x.s_Phone2.Contains(searchTerm)) ||
                                       (x.s_Phone3 != null && x.s_Phone3.Contains(searchTerm)) ||
                                       (x.s_Sales != null && x.s_Sales.Contains(searchTerm)) ||
                                       (x.s_SalesPhone1 != null && x.s_SalesPhone1.Contains(searchTerm)) ||
                                       (x.s_SalesPhone2 != null && x.s_SalesPhone2.Contains(searchTerm))
                                  );
            }

            //城市
            if (!string.IsNullOrEmpty(city))
                data = data.Where(x => x.s_City == city);
            #endregion

            // 排序
            switch (sortField)
            {
                case "Name":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.s_Name) : data.OrderByDescending(x => x.s_Name);
                    break;
                case "City":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.s_City) : data.OrderByDescending(x => x.s_City);
                    break;
                default:
                    data = data.OrderBy(x => x.s_Id);
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
            var model = new Supplier()
            {
                s_State = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            supplier.s_CreateDate = DateTime.Now;
            _rep.Add(supplier);

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
        public ActionResult Edit(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            _rep.Update(supplier);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _rep.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var data = _rep.GetDataById(id);

            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        public ActionResult ExportToPDF()
        {
            // 取得資料
            var data = Session["ExportData"] as List<Supplier>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無匯出資料");
            }

            // 生成PDF
            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf", "SupplierList.pdf");
        }

        public ActionResult Print()
        {
            var data = Session["ExportData"] as List<Supplier>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無列印資料");
            }

            // 生成PDF
            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf");
        }
    }
}