using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages.Html;
using web_nong_san.Code;

namespace web_nong_san.Controllers
{
    public class HangSanXuatController : Controller
    {
        // GET: HangSanXuat
        QL_NongSanSachEntities db_ns = new QL_NongSanSachEntities();
       
        public ActionResult DanhSachHangSanXuat()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 1))
                return HttpNotFound();

            var ds_hsx = from s in db_ns.HANGSANXUATs select s;
            return View(ds_hsx);
        }          

       

        // POST: HangSanXuat/ThemHangSanXuat
        [HttpPost]
        public ActionResult ThemHangSanXuat(string TENHSX, string GIOITHIEU, string URL, string KEYWORD, HttpPostedFileBase HINH)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 1))
                return HttpNotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    if (db_ns.HANGSANXUATs.Any(h => h.TENHSX == TENHSX))
                    {
                        return Json(new { success = false, message = "Tên hãng sản xuất đã tồn tại." });
                    }
                    HANGSANXUAT hsx = new HANGSANXUAT();
                    if (HINH != null && HINH.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(HINH.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);

                        // Lưu tệp ảnh vào thư mục trên máy chủ
                        hsx.TENHSX = TENHSX;
                        hsx.URL = URL;                        
                        hsx.GIOITHIEU = GIOITHIEU;
                        HINH.SaveAs(path);
                        hsx.HINH = fileName; // Lưu tên tệp ảnh vào cơ sở dữ liệu
                    }

                    db_ns.HANGSANXUATs.Add(hsx);
                    db_ns.SaveChanges();
                    return Json(new { success = true, message = "Thêm hãng sản xuất thành công." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                }
            }
            return RedirectToAction("DanhSachHangSanXuat");
        }           

        public bool KiemTraKhoaNgoai(int id, string a)
        {
            bool ketqua = true;
            if (a.Equals("HangSanXuat"))
            {
                var gh = db_ns.SANPHAMs.FirstOrDefault(s => s.MAHSX == id);
                return (gh == null); // Đúng khi không có sản phẩm nào có MAHSX bằng id
            }
            return ketqua;
        }


        [HttpGet]
        public ActionResult XoaHangSanXuat(int id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 1))
                return HttpNotFound();
            HANGSANXUAT hsx = db_ns.HANGSANXUATs.SingleOrDefault(s => s.MAHSX == id);
            if (hsx == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hsx);
        }

        [HttpPost, ActionName("XoaHangSanXuat")]
        public ActionResult XacNhanXoaHangSanXuat(int id)
        {
            HANGSANXUAT hsx = db_ns.HANGSANXUATs.SingleOrDefault(s => s.MAHSX == id);
            if (hsx == null || !KiemTraKhoaNgoai(id, "HangSanXuat"))
            {
                ModelState.AddModelError("", "Ban phai xoa het cac san pham cua nha san xuat nay!");
                return View("XoaHangSanXuat", hsx);
            }
            db_ns.HANGSANXUATs.Remove(hsx);
            db_ns.SaveChanges();
            return RedirectToAction("DanhSachHangSanXuat");
        }        

        [HttpPost]
        public ActionResult XoaSanXuat(int id)
        {
            // Tìm hãng sản xuất cần xóa trong cơ sở dữ liệu
            HANGSANXUAT hsx = db_ns.HANGSANXUATs.SingleOrDefault(s => s.MAHSX == id);         
                      
            if (hsx != null)
            {
                if (!KiemTraKhoaNgoai(id, "HangSanXuat"))
                {
                    return Json(new { success = false, message = "" });
                }
                else
                {
                    // Xóa hãng sản xuất
                    db_ns.HANGSANXUATs.Remove(hsx);
                    db_ns.SaveChanges();
                    return Json(new { success = true, message = "" });
                }    
            }
            // Trả về kết quả thành công
            return RedirectToAction("DanhSachHangSanXuat");
        }       
    }
}