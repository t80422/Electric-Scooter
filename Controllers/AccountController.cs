using Electric_Scooter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Electric_Scooter.Controllers
{
    public class AccountController : Controller
    {
        private readonly Dictionary<string, string> _users = new Dictionary<string, string>()
        {
            {"ichi","ichi" }
        };

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                if (_users.TryGetValue(model.Username, out string password) && password == model.Password)
                {
                    //登入成功
                    Session["Username"] = model.Username;
                    return RedirectToAction("Index", "Order");
                }
                else
                {
                    ModelState.AddModelError("", "無效的用戶名或密碼");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}