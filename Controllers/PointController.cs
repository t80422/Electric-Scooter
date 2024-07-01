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
    [IsLogin]
    public class PointController : Controller
    {
        private IRepository<Points> _pointRep = new Repository<Points>(new I_ChiEntities());
        private IRepository<Customer> _cusRep = new Repository<Customer>(new I_ChiEntities());
        private IRepository<PointSet> _psRep = new Repository<PointSet>(new I_ChiEntities());

        public ActionResult Index(string searchTerm, bool? state, DateTime? sendStart, DateTime? sentEnd, DateTime? dueStart, DateTime? dueEnd, string type, DateTime? cusInDateStart, DateTime? cusInDateEnd, string sortField, string sortOrder = "asc")
        {
            var data = _pointRep.GetAllData();

            #region 搜尋
            //關鍵字
            if (!string.IsNullOrEmpty(searchTerm))
            {
                data = data.Where(x => x.Customer.c_Name.Contains(searchTerm) ||
                                       x.Customer.c_Phone1.Contains(searchTerm) ||
                                       (x.Customer.c_Phone2 != null && x.Customer.c_Phone2.Contains(searchTerm)) ||
                                       (x.Orders != null && x.Orders.o_No.Contains(searchTerm))
                                 );
            }

            //狀態
            if (state.HasValue)
                data = data.Where(x => x.po_State == state);

            //發送日期
            if (sendStart.HasValue)
                data = data.Where(x => x.po_SentDate >= sendStart);

            if (sentEnd.HasValue)
                data = data.Where(x => x.po_SentDate <= sentEnd);

            //到期日期
            if (dueStart.HasValue)
                data = data.Where(x => x.po_DueDate >= dueStart);

            if (dueEnd.HasValue)
                data = data.Where(x => x.po_DueDate <= dueEnd);

            //分類
            if (!string.IsNullOrEmpty(type))
                data = data.Where(x => x.po_Type == type);

            //會員加入日期
            if (cusInDateStart.HasValue)
                data = data.Where(x => x.Customer.c_CreateDate >= cusInDateStart);

            if (cusInDateEnd.HasValue)
                data = data.Where(x => x.Customer.c_CreateDate <= cusInDateEnd);
            #endregion

            #region 排序
            switch (sortField)
            {
                case "Point":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.po_Quantity) : data.OrderByDescending(x => x.po_Quantity);
                    break;
                case "SendDate":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.po_SentDate) : data.OrderByDescending(x => x.po_SentDate);
                    break;
                case "DueDate":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.po_DueDate) : data.OrderByDescending(x => x.po_DueDate);
                    break;
                case "Type":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.po_Type) : data.OrderByDescending(x => x.po_Type);
                    break;
                default:
                    data = data.OrderBy(x => x.po_Id);
                    break;
            }
            #endregion

            ViewBag.SortOrder = sortOrder;
            ViewBag.SortField = sortField;

            //暫存資料供"匯出"、"列印"使用
            Session["ExportData"] = data.ToList();

            return View(data);
        }

        public ActionResult Setting()
        {
            var model = _psRep.GetAllData().FirstOrDefault();

            if (model == null)
                model = new PointSet();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Setting(PointSet model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ps = _psRep.GetAllData().FirstOrDefault();

            if (ps == null)
            {
                _psRep.Add(model);
            }
            else
            {
                using (var db = new I_ChiEntities())
                {
                    var data = db.PointSet.First();
                    data.ps_Id = model.ps_Id;
                    data.ps_Multiple = model.ps_Multiple;
                    data.ps_Validity = model.ps_Validity;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            var model = new Points()
            {
                po_State = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Points data)
        {
            if (!ModelState.IsValid)
                return View(data);

            if (data.po_DueDate == null)
            {
                var validity = _psRep.GetDataById(1).ps_Validity;
                data.po_DueDate = data.po_SentDate.Value.AddMonths(validity);
            }

            _pointRep.Add(data);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var order = _pointRep.GetDataById(id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Points model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _pointRep.Update(model);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var data = _pointRep.GetDataById(id);

            if (data == null)
                return HttpNotFound();

            return View(data);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _pointRep.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult ExportToPDF()
        {
            // 取得資料
            var data = Session["ExportData"] as List<Points>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無匯出資料");
            }

            // 生成PDF
            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf", "PointList.pdf");
        }

        public ActionResult Print()
        {
            var data = Session["ExportData"] as List<Points>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無列印資料");
            }

            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);

            return File(pdfBytes, "application/pdf");
        }

        public ActionResult GetCustomer()
        {
            var data = _cusRep.GetAllData().ToList();
            return Json(data.Select(x => new { id = x.c_Id, text = x.c_Name, phone = x.c_Phone1 }), JsonRequestBehavior.AllowGet);
        }
    }
}