using Electric_Scooter.Helpers;
using Electric_Scooter.Models;
using Electric_Scooter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Electric_Scooter.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        private IProductRep _prodRep;
        private IRepository<Supplier> _supRep;

        public ProductController()
        {
            _prodRep = new ProductRep(new I_ChiEntities());
            _supRep = new Repository<Supplier>(new I_ChiEntities());
        }

        public ActionResult Index(string searchTerm, string type,int? minPrice,int? maxPrice, string sortField, string sortOrder = "asc")
        {
            var data = _prodRep.GetAllData();

            #region 搜尋
            //關鍵字
            if (!string.IsNullOrEmpty(searchTerm))
            {
                data = data.Where(x => (x.p_Name != null && x.p_Name.Contains(searchTerm)) ||
                                       (x.p_Model != null && x.p_Model.Contains(searchTerm)) ||
                                       (x.p_MotorNo != null && x.p_MotorNo.Contains(searchTerm)) ||
                                       (x.p_CarFrameNo != null && x.p_CarFrameNo.Contains(searchTerm)) ||
                                       (x.Supplier?.s_Name != null && x.Supplier.s_Name.Contains(searchTerm))
                                  );
            }

            //分類
            if (!string.IsNullOrEmpty(type))
                data = data.Where(x => x.p_Type == type);

            //價格範圍
            if (minPrice.HasValue)
                data = data.Where(x => x.p_Price >= minPrice.Value);

            if (maxPrice.HasValue)
                data = data.Where(x => x.p_Price <= maxPrice.Value);
            #endregion

            // 排序
            switch (sortField)
            {
                case "Type":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.p_Type) : data.OrderByDescending(x => x.p_Type);
                    break;
                case "Price":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.p_Price) : data.OrderByDescending(x => x.p_Price);
                    break;
                case "Supplier":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.Supplier?.s_Name) : data.OrderByDescending(x => x.Supplier?.s_Name);
                    break;
                default:
                    data = data.OrderBy(x => x.p_Id);
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
        public ActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            product.p_CreateDate = DateTime.Now;
            _prodRep.Add(product);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var data = _prodRep.GetDataById(id);

            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _prodRep.Update(product);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _prodRep.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var data = _prodRep.GetDataById(id);

            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        public ActionResult ExportToPDF()
        {
            // 取得資料
            var data = Session["ExportData"] as List<Product>;

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
            var data = Session["ExportData"] as List<Product>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無列印資料");
            }

            // 生成PDF
            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf");
        }

        public ActionResult GetColors()
        {
            var colors = _prodRep.GetColors();
            return Json(colors.Select(x => new { id = x, text = x }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSuppliers()
        {
            var suppliers = _supRep.GetAllData().ToList();
            return Json(suppliers.Select(x => new { id = x.s_Id, text = x.s_Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNames()
        {
            var names = _prodRep.GetNames();
             return Json(names.Select(x => new { id = x, text = x }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetModels()
        {
            var models = _prodRep.GetModels();
            return Json(models.Select(x => new { id = x, text = x }), JsonRequestBehavior.AllowGet);
        }
    }
}