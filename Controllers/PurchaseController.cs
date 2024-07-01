using Electric_Scooter.Helpers;
using Electric_Scooter.Models;
using Electric_Scooter.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Electric_Scooter.Controllers
{
    [IsLogin]
    /// <summary>
    /// 進貨系統
    /// </summary>
    public class PurchaseController : Controller
    {
        private IRepository<Purchase> _rep = new Repository<Purchase>(new I_ChiEntities());
        private IRepository<Supplier> _supRep = new Repository<Supplier>(new I_ChiEntities());
        private IProductRep _prodRep = new ProductRep(new I_ChiEntities());

        public ActionResult Index(string searchTerm, string type, bool? settled, bool? paymentReceived, bool? installment, DateTime? stkInStart, DateTime? stkInEnd, DateTime? stkOutStart, DateTime? stkOutEnd, string sortField, string sortOrder = "asc")
        {
            var data = _rep.GetAllData();

            #region 搜尋
            //關鍵字
            if (!string.IsNullOrEmpty(searchTerm))
            {
                data = data.Where(x => x.Product.Supplier.s_Name != null && x.Product.Supplier.s_Name.Contains(searchTerm));
            }

            //分類
            if (!string.IsNullOrEmpty(type))
                data = data.Where(x => x.Product.p_Type == type);

            //結清
            if (settled.HasValue)
                data = data.Where(x => x.pu_IsSettled.Value == settled);

            //收款
            if (paymentReceived.HasValue)
                data = data.Where(x => x.pu_IsPaymentReceived.Value == paymentReceived);

            //分期
            if (installment.HasValue)
                data = data.Where(x => x.pu_IsInstallment.Value == installment);

            //入庫日期
            if (stkInStart.HasValue)
                data = data.Where(x => x.pu_StockInDate >= stkInStart);

            if (stkInEnd.HasValue)
                data = data.Where(x => x.pu_StockInDate <= stkInEnd);

            //出庫日期
            if (stkOutStart.HasValue)
                data = data.Where(x => x.pu_StockOutDate >= stkOutStart);

            if (stkOutEnd.HasValue)
                data = data.Where(x => x.pu_StockOutDate <= stkOutEnd);
            #endregion

            // 排序
            switch (sortField)
            {
                case "StockIn":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.pu_StockInDate) : data.OrderByDescending(x => x.pu_StockInDate);
                    break;
                case "StockOut":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.pu_StockOutDate) : data.OrderByDescending(x => x.pu_StockOutDate);
                    break;
                default:
                    data = data.OrderBy(x => x.pu_Id);
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
        public ActionResult Create(Purchase data)
        {
            if (!ModelState.IsValid)
                return View(data);

            data.pu_CreateDate = DateTime.Now;
            _rep.Add(data);

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
        public ActionResult Edit(Purchase data)
        {
            if (!ModelState.IsValid)
                return View(data);

            _rep.Update(data);

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
            var data = Session["ExportData"] as List<Purchase>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無匯出資料");
            }

            // 生成PDF
            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf", "PurchaseList.pdf");
        }

        public ActionResult Print()
        {
            var data = Session["ExportData"] as List<Purchase>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無列印資料");
            }

            // 生成PDF
            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf");
        }

        public ActionResult GetSuppliers()
        {
            var suppliers = _supRep.GetAllData().ToList();
            return Json(suppliers.Select(x => new { id = x.s_Id, text = x.s_Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProducts(int? supId, string type)
        {
            var query = _prodRep.GetAllData();

            if (supId.HasValue && supId > 0)
                query = query.Where(x => x.p_s_Id == supId.Value);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(x => x.p_Type == type);

            return Json(query.Select(x => new { id = x.p_Id, text = x.p_Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}
