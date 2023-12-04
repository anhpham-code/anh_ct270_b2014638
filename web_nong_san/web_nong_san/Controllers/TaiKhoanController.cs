using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using web_nong_san.Code;

namespace web_nong_san.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: TaiKhoan
        public ActionResult Index()
        {
            return View();
        }

        QL_NongSanSachEntities db_ns = new QL_NongSanSachEntities();         
        public ActionResult AboutAccount(int id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            var account = db_ns.TAIKHOANs.SingleOrDefault(s => s.MATK == id);
            return View(account);
        }
      
    }
}