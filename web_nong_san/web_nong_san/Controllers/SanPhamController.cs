using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using web_nong_san.Code;

namespace web_nong_san.Controllers
{
    public class SanPhamController : Controller
    {
        QL_NongSanSachEntities db_ns = new QL_NongSanSachEntities();

        public ActionResult DanhSachSanPham()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 2))
                return HttpNotFound();
            var ds_sp = from s in db_ns.SANPHAMs select s;
            return View(ds_sp);
        }

        public ActionResult ThemSanPham()
        {
            ViewBag.MAHSXList = new SelectList(db_ns.HANGSANXUATs, "MAHSX", "TENHSX");
            ViewBag.MALOAIList = new SelectList(db_ns.LOAISANPHAMs, "MALOAI", "TENLOAI");
            ViewBag.MAGIAMGIAList = new SelectList(db_ns.GIAMGIAs, "MAGIAMGIA", "GIAMGIA1");

            return View();
        }
        // POST: SanPham/ThemSanPham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSanPham(string MACODE, string TENSP,string URL,int GIA,int MAGIAMGIA,HttpPostedFileBase HINH,int SOLUONGTON,DateTime NGAYNHAP,int BANCHAY,string MOTANGAN, string MOTA,int ANHIEN, int MAHSX, int MALOAI)
        {
            if (ModelState.IsValid)            {
               
                try
                {
                    if (db_ns.SANPHAMs.Any(h => h.TENSP == TENSP))
                    {
                        return Json(new { success = false, message = "Tên sản phẩm đã tồn tại." });
                    }
                    SANPHAM sp = new SANPHAM();
                    if (HINH != null && HINH.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(HINH.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);

                        // Lưu tệp ảnh vào thư mục trên máy chủ
                        sp.MACODE = MACODE;
                        sp.TENSP = TENSP;
                        if(URL != null && URL.Length > 0) { 
                            sp.URL = URL; 
                        }
                        sp.GIA = GIA;
                        sp.MAGIAMGIA = MAGIAMGIA;
                        sp.SOLUONGTON = SOLUONGTON;
                        sp.NGAYNHAP = NGAYNHAP;
                        sp.BANCHAY = BANCHAY;
                        sp.MOTANGAN = MOTANGAN;
                        sp.MOTA = MOTA;
                        sp.ANHIEN = ANHIEN;
                        sp.MAHSX = MAHSX;
                        sp.MALOAI = MALOAI;
                        HINH.SaveAs(path);
                        sp.HINH = fileName; // Lưu tên tệp ảnh vào cơ sở dữ liệu
                        db_ns.SANPHAMs.Add(sp);
                        db_ns.SaveChanges();
                        return RedirectToAction("DanhSachSanPham");
                    }                
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationError in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationError.ValidationErrors)
                        {
                            var propertyName = validationError.PropertyName;
                            var errorMessage = validationError.ErrorMessage;

                            // In ra thông tin chi tiết lỗi
                            Console.WriteLine($"Property: {propertyName}, Error: {errorMessage}");

                            // Ghi log lỗi vào một tệp
                            var logFilePath = @"D:\error_log.txt"; // Đường dẫn đầy đủ trên ổ D
                            using (StreamWriter writer = new StreamWriter(logFilePath, true))
                            {
                                writer.WriteLine($"{DateTime.Now} - Property: {propertyName}, Error: {errorMessage}");
                            }
                        }
                    }
                    return Json(new { success = false, message = "Validation error occurred. See console for details." });
                }
            }
            //return Json(new { success = false, message = "Validation error occurred. See console for details." });
            return RedirectToAction("ThemSanPham");
        }


        public bool KiemTraKhoaNgoai(int id, string a)
        {
            bool ketqua = true;
            if (a.Equals("SanPham"))
            {
                var ctgh = db_ns.CHITIETDONDATs.FirstOrDefault(s => s.MASP == id);
                var ctdd = db_ns.CHITIETDONDATs.FirstOrDefault(s => s.MASP == id);
                return (ctdd == null && ctgh == null); // Đúng khi không có sản phẩm nào có MASP bằng id
            }
            /* if (a.Equals("NhaTro"))
            {
                var p = db_ns.Phongs.FirstOrDefault(s => s.id_NT == id);
                return (p == null);
            }*/
            return ketqua;
        }              

        [HttpPost]
        public ActionResult XoaSanPham(int id)
        {
            // Tìm sản phẩm cần xóa trong cơ sở dữ liệu
            SANPHAM SP = db_ns.SANPHAMs.SingleOrDefault(s => s.MASP == id);

            if (SP != null)
            {
                if (!KiemTraKhoaNgoai(id, "SanPham"))
                {
                    return Json(new { success = false, message = "" });
                }
                else
                {
                    // Xóa sản phẩm
                    db_ns.SANPHAMs.Remove(SP);
                    db_ns.SaveChanges();
                    return Json(new { success = true, message = "" });
                }
            }

            // Trả về kết quả thành công
            return RedirectToAction("DanhSachSanPham");
        }

        public ActionResult ChinhSuaSanPham(int MASP)
        {
            // Truy vấn sản phẩm từ cơ sở dữ liệu dựa trên MASP
            SANPHAM sp = db_ns.SANPHAMs.SingleOrDefault(s => s.MASP == MASP);

            if (sp == null)
            {
                // Nếu sản phẩm không tồn tại, xử lý thông báo lỗi hoặc điều hướng đến trang 404
                return HttpNotFound(); // Trả về trang lỗi 404
            }
            // Trong action ChinhSuaSanPham
            ViewBag.MAHSXList = new SelectList(db_ns.HANGSANXUATs, "MAHSX", "TENHSX");
            ViewBag.MALOAIList = new SelectList(db_ns.LOAISANPHAMs, "MALOAI", "TENLOAI");
            ViewBag.MAGIAMGIAList = new SelectList(db_ns.GIAMGIAs, "MAGIAMGIA", "GIAMGIA1");

            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]      
        
        public ActionResult LuuChinhSuaSanPham(int MASP,string MACODE, string TENSP, string URL, int GIA, int MAGIAMGIA, int SOLUONGTON, int BANCHAY, string MOTANGAN, string MOTA, int ANHIEN, int MAHSX, int MALOAI)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Truy vấn sản phẩm từ cơ sở dữ liệu dựa trên MASP
                    SANPHAM existingSP = db_ns.SANPHAMs.SingleOrDefault(s => s.MASP == MASP);

                    if (existingSP == null)
                    {
                        return HttpNotFound(); // Trả về trang lỗi 404 nếu sản phẩm không tồn tại
                    }

                    // Ánh xạ từ tên thành mã
                    existingSP.MAHSX = Convert.ToInt32(MAHSX);
                    existingSP.MALOAI = Convert.ToInt32(MALOAI);
                    existingSP.MAGIAMGIA = Convert.ToInt32(MAGIAMGIA);

                    // Cập nhật thông tin sản phẩm từ dữ liệu được gửi từ form
                    existingSP.TENSP = TENSP;                    
                    existingSP.GIA = GIA;
                    existingSP.MOTANGAN = MOTANGAN;
                    existingSP.MOTA = MOTA;
                    existingSP.SOLUONGTON = SOLUONGTON;
                    existingSP.BANCHAY = BANCHAY;
                    existingSP.MACODE = MACODE;
                    existingSP.URL = URL;
                    existingSP.ANHIEN = ANHIEN;
                    // Cập nhật các trường dữ liệu khác tương tự                    

                    db_ns.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                    return RedirectToAction("DanhSachSanPham"); // Điều hướng đến danh sách sản phẩm sau khi cập nhật thành công
                }
                catch (Exception)
                {
                    // Xử lý lỗi nếu có
                    return View(); // Hiển thị form chỉnh sửa sản phẩm lại với thông báo lỗi
                }
            }

            return View(); // Hiển thị form chỉnh sửa sản phẩm lại nếu thông tin không hợp lệ
        }
    }
}