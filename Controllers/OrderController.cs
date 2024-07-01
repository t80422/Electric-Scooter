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
    /// <summary>
    /// 銷貨系統
    /// </summary>
    public class OrderController : Controller
    {
        private IRepository<Orders> _rep = new Repository<Orders>(new I_ChiEntities());
        private IRepository<Customer> _cusRep = new Repository<Customer>(new I_ChiEntities());
        private IRepository<Product> _prodRep = new Repository<Product>(new I_ChiEntities());
        private IOrderDetailRep _odRep = new OrderDetailRep(new I_ChiEntities());
        private IRepository<PointSet> _psRep = new Repository<PointSet>(new I_ChiEntities());
        private IPointRep _pointRep = new PointRep(new I_ChiEntities());

        public ActionResult Index(string searchTerm, string state, DateTime? startDate, DateTime? endDate, string sortField, string sortOrder = "asc")
        {
            var data = _rep.GetAllData();

            #region 搜尋
            //關鍵字
            if (!string.IsNullOrEmpty(searchTerm))
            {
                data = data.Where(x => x.o_No.Contains(searchTerm) ||
                                       x.Customer.c_Name.Contains(searchTerm) ||
                                       x.OrderDetail.Any(y => y.od_LicensePlate.Contains(searchTerm))
                                 );
            }

            //狀態
            if (!string.IsNullOrEmpty(state))
                data = data.Where(x => x.o_State == state);

            //訂單日期
            if (startDate.HasValue)
                data = data.Where(x => x.o_Date >= startDate);

            if (endDate.HasValue)
                data = data.Where(x => x.o_Date <= endDate);
            #endregion

            // 排序
            switch (sortField)
            {
                case "OrderNo":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.o_No) : data.OrderByDescending(x => x.o_No);
                    break;
                case "Date":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.o_Date) : data.OrderByDescending(x => x.o_Date);
                    break;
                case "State":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.o_State) : data.OrderByDescending(x => x.o_State);
                    break;
                case "Price":
                    data = sortOrder == "asc" ? data.OrderBy(x => x.o_TotalPrice) : data.OrderByDescending(x => x.o_TotalPrice);
                    break;
                default:
                    data = data.OrderBy(x => x.o_Id);
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
            var model = new OrderVM();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderVM data)
        {
            if (!ModelState.IsValid)
            {
                return View(data);
            }

            data.Orders.o_CreateDate = DateTime.Now;
            data.Orders.o_No = GenerateOrderNo(data.Orders.o_Date.Value);

            //保存訂單
            _rep.Add(data.Orders);

            //保存訂單詳情
            foreach (var detail in data.OrderDetails)
            {
                detail.od_o_Id = data.Orders.o_Id;
                _odRep.Add(detail);
            }

            //保存點數
            var pointSet = _psRep.GetDataById(1);
            var multiple = pointSet.ps_Multiple;
            var validity = pointSet.ps_Validity;

            var points = new Points()
            {
                po_o_Id = data.Orders.o_Id,
                po_c_Id = data.Orders.o_c_Id,
                po_SentDate = data.Points.po_SentDate,
                po_State = true,
                po_Quantity = Convert.ToInt32(multiple * data.Orders.o_TotalPrice),
                po_DueDate = data.Points.po_SentDate.Value.AddMonths(validity),
                po_Type = "購買"
            };

            _pointRep.Add(points);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var order = _rep.GetDataById(id);

            if (order == null)
                return HttpNotFound();

            var vm = new OrderVM()
            {
                Orders = order,
                OrderDetails = order.OrderDetail.ToList(),
                Points = _pointRep.GetDataByOrderId(id)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            _rep.Update(vm.Orders);

            //先保存新資料
            var newOD = new List<OrderDetail>();
            foreach (var detail in vm.OrderDetails)
            {
                detail.od_o_Id = vm.Orders.o_Id;
                newOD.Add(detail);
            }

            _odRep.DeleteByOrderId(vm.Orders.o_Id);

            foreach (var detail in newOD)
            {
                detail.od_o_Id = vm.Orders.o_Id;
                _odRep.Add(detail);
            }

            //重新計算點數
            var pointSet = _psRep.GetDataById(1);
            var multiple = pointSet.ps_Multiple;
            var validity = pointSet.ps_Validity;
            var points = _pointRep.GetDataByOrderId(vm.Orders.o_Id);
            points.po_SentDate = vm.Points.po_SentDate;
            points.po_DueDate = vm.Points.po_SentDate.Value.AddMonths(validity);
            points.po_Quantity = Convert.ToInt32(multiple * vm.Orders.o_TotalPrice);
            points.po_Type = "購買";

            _pointRep.Update(points);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var order = _rep.GetDataById(id);

            if (order == null)
                return HttpNotFound();

            var vm = new OrderVM()
            {
                Orders = order,
                OrderDetails = order.OrderDetail.ToList()
            };

            return View(vm);
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
            var data = Session["ExportData"] as List<Orders>;

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
            var data = Session["ExportData"] as List<Orders>;

            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無列印資料");
            }

            // 生成PDF
            var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data);
            //var pdfBytes = PDFHelper.GeneratePdfFromHtml(ControllerContext, "ExportToPdfView", data.First());

            return File(pdfBytes, "application/pdf");
        }

        public ActionResult GetCustomer()
        {
            var data = _cusRep.GetAllData().ToList();
            return Json(data.Select(x => new { id = x.c_Id, text = x.c_Name, phone = x.c_Phone1 }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductDetailsByCarFrameNo()
        {
            var product = _prodRep.GetAllData().Where(x => x.p_CarFrameNo != null).ToList();

            if (product != null)
            {
                return Json(product.Select(x => new
                {
                    id = x.p_Id,
                    text = x.p_CarFrameNo,
                    type = x.p_Type,
                    name = x.p_Name,
                    color = x.p_Color,
                    price = x.p_Price
                }), JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound("Product not found.");
        }

        public ActionResult GetProductDetailsByName()
        {
            var data = _prodRep.GetAllData().ToList();

            if (data != null)
            {
                return Json(data.Select(x => new
                {
                    id = x.p_Id,
                    text = x.p_Name,
                    type = x.p_Type,
                    carFrameNo = x.p_CarFrameNo,
                    color = x.p_Color,
                    price = x.p_Price
                }), JsonRequestBehavior.AllowGet);
            }

            return HttpNotFound("找不到商品");
        }

        /// <summary>
        /// 產生訂單編號
        /// </summary>
        /// <returns></returns>
        private string GenerateOrderNo(DateTime orderDate)
        {
            var maxOrderNo = _rep.GetAllData()
                               .Where(x => x.o_Date >= orderDate && x.o_Date < orderDate.AddDays(1))
                               .OrderByDescending(x => x.o_No)
                               .Select(x => x.o_No)
                               .FirstOrDefault();

            int newSequence = 1;
            if (maxOrderNo != null)
            {
                string maxSequenceStr = maxOrderNo.Substring(8);
                if (int.TryParse(maxSequenceStr, out int maxSequence))
                    newSequence = maxSequence + 1;
            }

            return $"{orderDate:yyyyMMdd}{newSequence:D3}";
        }
    }
}