using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using web_nong_san.Code;
using web_nong_san.Models;

namespace web_nong_san.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        QL_NongSanSachEntities db_ns = new QL_NongSanSachEntities();
        /*public ActionResult DanhSachSanPhamNguoiDung(int? page,string search,string filter)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 3))
                return HttpNotFound();


            // Gửi thông tin tài khoản đến View bằng ViewBag
            ViewBag.UserInfo = SessionHelper.GetSession();
            ViewBag.ac_img = SessionHelper.GetImages();
            
            // Lấy danh sách sản phẩm từ database
            var products = db_ns.SANPHAMs.ToList();
            //lấy danh sách mã loại

            var uniqueCategories = products.Select(p => p.LOAISANPHAM.TENLOAI).Distinct().ToList();
            *//*var uniqueCategories = db_ns.LOAISANPHAMs?.ToList();*//*
            ViewBag.UniqueCategories = uniqueCategories;

            // Xử lý search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                products = products.Where(p =>
                    p.TENSP.ToLower().Contains(search) ||                    
                    p.LOAISANPHAM.TENLOAI.ToLower().Contains(search)
                ).ToList();
            }

            // Xử lý filter
            if (!string.IsNullOrEmpty(filter) && filter != "all")
            {
                products = products.Where(p => p.LOAISANPHAM.TENLOAI == filter).ToList();
            }

            // Sắp xếp sản phẩm (ví dụ: theo MASP)
            products = products.OrderBy(p => p.MASP).ToList();

            // Tính toán thông tin phân trang
            int pageSize = 12;
            int currentPage = page ?? 1;
            var totalProducts = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            var paginatedProducts = products
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Đưa thông tin vào ViewBag để sử dụng trong View
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            if (SessionHelper.GetSession().GIOHANGs.FirstOrDefault()?.MAGH != null)
                ViewBag.CartId = SessionHelper.GetSession().GIOHANGs.FirstOrDefault().MAGH;
            else
                ViewBag.CartId = 0;
            // Trả về View với danh sách sản phẩm đã lọc và phân trang
            return View(paginatedProducts);
        }
*/
        public ActionResult DanhSachSanPhamNguoiDung(int? page, string search, string filter)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 3))
                return HttpNotFound();

            // Gửi thông tin tài khoản đến View bằng ViewBag
            ViewBag.UserInfo = SessionHelper.GetSession();
            ViewBag.ac_img = SessionHelper.GetImages();

            // Lấy danh sách sản phẩm từ database
            var products = db_ns.SANPHAMs.ToList();

            // Lấy danh sách mã loại
            var uniqueCategories = products.Select(p => p.LOAISANPHAM.TENLOAI).Distinct().ToList();
            ViewBag.UniqueCategories = uniqueCategories;

            // Xử lý search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                products = products.Where(p =>
                    p.TENSP.ToLower().Contains(search) //||
                    //p.LOAISANPHAM.TENLOAI.ToLower().Contains(search)
                ).ToList();
            }

            // Xử lý filter
            if (!string.IsNullOrEmpty(filter) && filter != "all")
            {
                products = products.Where(p => p.LOAISANPHAM.TENLOAI == filter).ToList();
            }

            // Sắp xếp sản phẩm (ví dụ: theo MASP)
            products = products.OrderBy(p => p.MASP).ToList();

            // Tính toán thông tin phân trang cho dữ liệu đã lọc và đã tìm kiếm
            int pageSize = 12;
            int currentPage = page ?? 1;
            var totalProducts = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            var paginatedProducts = products
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Đưa thông tin vào ViewBag để sử dụng trong View
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Search = search;
            ViewBag.Filter = filter;
            ViewBag.CartId = SessionHelper.GetSession().GIOHANGs.FirstOrDefault()?.MAGH ?? 0;

            // Trả về View với danh sách sản phẩm đã lọc và phân trang
            return View(paginatedProducts);
        }

        public ActionResult CHiTietSanPhamNguoiDung(int? id)
        {
            if (id != null)
            {
                var sp = db_ns.SANPHAMs.SingleOrDefault(s => s.MASP.Equals(id));
            }
            return View("sp");
        }

        public ActionResult GetProducts(int currentPage, int pageSize)
        {
            // Lấy danh sách sản phẩm từ nguồn dữ liệu, ví dụ từ cơ sở dữ liệu
            List<SANPHAM> products = db_ns.SANPHAMs.ToList();

            // Áp dụng phân trang dựa trên currentPage và pageSize
            var pagedProducts = products.Skip((currentPage - 1) * pageSize).Take(pageSize);

            return PartialView("_ProductList", pagedProducts);
        }
        public ActionResult XemChiTietSanPham(int? productId)
        {
            var product = db_ns.SANPHAMs.SingleOrDefault(s => s.MASP == productId);

            return PartialView("XemChiTietSanPham", product);
        }
        public ActionResult SlickGridExample()
        {
            var productList = db_ns.SANPHAMs.AsNoTracking().ToList();
            return View(productList);
        }


        // Các hàm xử lý AJAX để lấy dữ liệu (nếu cần)
        public ActionResult GetSlickGridData()
        {
            var productList = db_ns.SANPHAMs.Select(p => new {
                p.MASP,
                p.TENSP,
                p.GIA,
                // Thêm các thuộc tính cần thiết khác
            }).ToList();
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult XemTucTin()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            ViewBag.UserInfo = SessionHelper.GetSession();
            ViewBag.ac_img = SessionHelper.GetImages();
            ViewBag.CartId = SessionHelper.GetSession().GIOHANGs.FirstOrDefault()?.MAGH ?? 0;
            var dstt = from s in db_ns.TINTUCs select s;
            return View(dstt);
        }
        
    }
}
