using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web_nong_san.Controllers
{
    public class LoaiSanPhamController : Controller
    {
        // GET: LoaiSanPham
        QL_NongSanSachEntities db_ns = new QL_NongSanSachEntities();
        public ActionResult DanhSachLoaiSanPham()
        {
            return View();
        }

       
    }
}