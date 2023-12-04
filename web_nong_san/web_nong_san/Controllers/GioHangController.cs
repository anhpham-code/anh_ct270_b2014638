using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web_nong_san.Code;
using web_nong_san.Models;

namespace web_nong_san.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        QL_NongSanSachEntities db_ns = new QL_NongSanSachEntities();
        [HttpPost]
        public ActionResult GioHang(int? id)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 3))
                return HttpNotFound();
            if (id != null)
            {
                var gh = db_ns.GIOHANGs.SingleOrDefault(s => s.MATK == id);
                var ctdh = db_ns.CHITIETGIOHANGs.Where(s => s.MAGH == gh.MAGH).ToList();
                return View(ctdh);
            }
            else
            {
                var ctdh = new CHITIETGIOHANG();
                return View(ctdh);
            }

        }
        [HttpPost]
        public ActionResult XoaChiTietDonHang(int id)
        {
            // Kiểm tra xem sản phẩm đã có trong giỏ hàng hay chưa. Nếu đã có, chỉ cập nhật số lượng.
            // Đã đăng nhập hoặc có thông tin người dùng
            var userId = SessionHelper.GetSession().MATK; // Thay bằng logic để lấy ID người dùng
            var donHang = db_ns.GIOHANGs.SingleOrDefault(s => s.MATK == userId);
            // Lấy MAGH của đơn hàng vừa được thêm vào CSDL
            int MAGH = donHang.MAGH;
            CHITIETGIOHANG ctdh = db_ns.CHITIETGIOHANGs.SingleOrDefault(s => s.MASP == id && s.MAGH == MAGH);

            if (ctdh != null)
            {
                db_ns.CHITIETGIOHANGs.Remove(ctdh);
                db_ns.SaveChanges();
                TinhTongTienDonHang();
                return Json(new { success = true, message = "" });

            }
            // Trả về kết quả thành công
            return RedirectToAction("GioHang");
        }
        [HttpPost]
        public ActionResult XoaToanBoGioHang()
        {
            // Lấy mã thành viên từ Session
            var userId = SessionHelper.GetSession().MATK;

            // Tìm đơn hàng của thành viên đó
            var gioHang = db_ns.GIOHANGs.SingleOrDefault(s => s.MATK == userId);

            if (gioHang != null)
            {
                // Tìm chi tiết đơn hàng của đơn hàng đó
                var CHITIETGIOHANGs = db_ns.CHITIETGIOHANGs.Where(s => s.MAGH == gioHang.MAGH).ToList();

                // Xóa tất cả chi tiết đơn hàng
                db_ns.CHITIETGIOHANGs.RemoveRange(CHITIETGIOHANGs);
                db_ns.SaveChanges();
                TinhTongTienDonHang();
                // Redirect to the shopping cart page after removing items
                return Json(new { success = true, message = "" });
            }

            // Trả về kết quả không thành công nếu không tìm thấy đơn hàng
            return Json(new { success = false, message = "" }); // or return some other ActionResult as needed
        }

        public void TinhTongTienDonHang()
        {
            var userId = SessionHelper.GetSession().MATK; // Thay bằng logic để lấy ID người dùng
            var gioHang = db_ns.GIOHANGs.SingleOrDefault(s => s.MATK == userId);
            // Lấy MAGH của đơn hàng vừa được thêm vào CSDL
            int MAGH = gioHang.MAGH;
            var ctgh = db_ns.CHITIETGIOHANGs.Where(s => s.MAGH == MAGH).ToList();
            long? TONGTHANHTIEN = 0;
            foreach (var item in ctgh)
            {
                TONGTHANHTIEN += item.GIA * item.SOLUONG;
            };
            gioHang.TONGTIEN = TONGTHANHTIEN;
            db_ns.Entry(gioHang).State = EntityState.Modified;
            db_ns.SaveChanges();
        }

        [HttpPost]
        public ActionResult ThemVaoGioHang(int productId, int quantity)
        {
            // Lấy thông tin sản phẩm từ CSDL dựa trên productId
            var product = db_ns.SANPHAMs.Find(productId);

            if (product != null)
            {
                // Kiểm tra xem có phiên mua hàng (cart) đã tồn tại cho người dùng hay không.
                // Nếu chưa tồn tại, bạn có thể tạo phiên mới cho người dùng.
                var cart = Session["Cart"] as List<CHITIETGIOHANG> ?? new List<CHITIETGIOHANG>();

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng hay chưa. Nếu đã có, chỉ cập nhật số lượng.
                // Đã đăng nhập hoặc có thông tin người dùng
                var userId = SessionHelper.GetSession().MATK; // Thay bằng logic để lấy ID người dùng
                var gioHang = db_ns.GIOHANGs.SingleOrDefault(s => s.MATK == userId);
                // Lấy MAGH của đơn hàng vừa được thêm vào CSDL
                int MAGH = gioHang.MAGH;
                var existingItem = db_ns.CHITIETGIOHANGs.SingleOrDefault(s => s.MASP == productId && s.MAGH == MAGH);

                if (existingItem != null)
                {
                    existingItem.SOLUONG = quantity;
                    existingItem.THANHTIEN = existingItem.SOLUONG * existingItem.GIA;

                    // Kiểm tra số lượng và xóa sản phẩm nếu số lượng là 0.
                    if (existingItem.SOLUONG == 0)
                    {
                        db_ns.CHITIETGIOHANGs.Remove(existingItem);
                    }
                }
                else
                {
                    // Nếu chưa có, thêm sản phẩm mới vào giỏ hàng.
                    cart.Add(new CHITIETGIOHANG
                    {
                        MASP = productId,
                        SOLUONG = quantity,
                        GIA = product.GIA,
                    });
                }
                // Lưu giỏ hàng vào phiên
                Session["Cart"] = cart;

                // Thêm các sản phẩm trong giỏ hàng vào bảng chi tiết đơn hàng
                foreach (var item in cart)
                {
                    var CHITIETGIOHANG = new CHITIETGIOHANG
                    {
                        MAGH = MAGH,
                        MASP = productId,
                        SOLUONG = item.SOLUONG,
                        GIA = item.GIA,
                        THANHTIEN = item.GIA * item.SOLUONG,
                        // Thêm thông tin khác của chi tiết đơn hàng ở đây
                    };

                    db_ns.CHITIETGIOHANGs.Add(CHITIETGIOHANG);
                }
                db_ns.SaveChanges();
                // Lưu chi tiết đơn hàng vào CSDL
                //TINH TONG TEN CUA DON HANG MOI KHI THAY DOI
                TinhTongTienDonHang();
                // Xóa giỏ hàng sau khi đơn hàng đã được tạo
                Session["Cart"] = new List<CartItem>();
            }
            // Trả về danh sách sản phẩm người dùng và danh sách sản phẩm trong giỏ hàng
            return Json(new { success = true, message = "" });
        }

        public ActionResult ThanhToanDonHang()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 3))
                return HttpNotFound();
            // Đã đăng nhập hoặc có thông tin người dùng
            var userId = SessionHelper.GetSession().MATK; // Thay bằng logic để lấy ID người dùng
            var gioHang = db_ns.GIOHANGs.SingleOrDefault(s => s.MATK == userId);
            // Lấy MAGH của đơn hàng vừa được thêm vào CSDL
            int MAGH = gioHang.MAGH;
            GIOHANG dh = db_ns.GIOHANGs.SingleOrDefault(s => s.MAGH == MAGH);
            return View(dh);
        }

        public ActionResult DanhSachDonDaDat()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 3))
                return HttpNotFound();
            // Đã đăng nhập hoặc có thông tin người dùng
            var userId = SessionHelper.GetSession().MATK; // Thay bằng logic để lấy ID người dùng
            var donHang = db_ns.DONHANGs.Where(d => d.MATK == userId).ToList();
            return View(donHang);
        }
        [HttpPost]
        public ActionResult ThemDon(int? PHUONGTHUCTHANHTOAN, long? diemtich, long? diemsudung, string diachi)
        {
            if (PHUONGTHUCTHANHTOAN == null || diemtich == null || diemsudung == null || string.IsNullOrEmpty(diachi))
            {
                // Xử lý lỗi, có thể là hiển thị thông báo hoặc chuyển hướng đến trang lỗi
                return View("Error");
            }
            var userId = SessionHelper.GetSession().MATK;
            DONHANG dh = new DONHANG
            {
                TRANGTHAI = 0,
                TONGTIEN = db_ns.GIOHANGs.SingleOrDefault(s => s.MATK == userId).TONGTIEN,
                MATK = userId,
                HINHTHUCTHANHTOAN = PHUONGTHUCTHANHTOAN,
                DIEMSUDUNG = diemsudung,
                DIEMTICH = diemtich,
                DIACHINHANHANG = diachi,
            };
            db_ns.DONHANGs.Add(dh);
            //Them cac chi tiet cho don hang nay va cap nhat lai cac csdl lien quan
            //Lay gio hang hien tai ra, thêm các chi tiết vào chi tiết đơn, giảm số lượng sản phẩm xuống,
            var gh = db_ns.GIOHANGs.SingleOrDefault(s => s.MATK == userId);
            var ctgh = db_ns.CHITIETGIOHANGs.Where(s => s.MAGH == gh.MAGH).ToList();
            int MADH = dh.MADH;
            foreach (var item in ctgh)
            {
                var CHITIETDONDAT = new CHITIETDONDAT
                {
                    MADH = MADH,
                    MASP = item.MASP,
                    SOLUONG = item.SOLUONG,
                    GIA = item.GIA,
                    THANHTIEN = item.GIA * item.SOLUONG,
                    // Thêm thông tin khác của chi tiết đơn hàng ở đây
                };
                var sanPhamToUpdate = db_ns.SANPHAMs.Find(item.MASP);
                sanPhamToUpdate.SOLUONGTON = sanPhamToUpdate.SOLUONGTON - item.SOLUONG;
                db_ns.Entry(item).State = EntityState.Modified;
                db_ns.CHITIETDONDATs.Add(CHITIETDONDAT);
            }
            db_ns.CHITIETGIOHANGs.RemoveRange(ctgh);

            db_ns.SaveChanges();
            // Đã đăng nhập hoặc có thông tin người dùng
            // Thay bằng logic để lấy ID người dùng
            //var donHang = db_ns.DONHANGs.Where(d => d.MATK == userId).ToList();
            return RedirectToAction("DanhSachSanPhamNguoiDung", "NguoiDung");
        }
        /*[HttpPost]
        public ActionResult DatHang()
        {

            DONHANG dh = db_ns.DONHANGs.
            return View();
        }*/

        public ActionResult DuyetDon()
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 2))
                return HttpNotFound();
            var Dsdd = from s in db_ns.DONHANGs select s;
            return View(Dsdd);
        }
        [HttpGet]
        public ActionResult XemChiTietDonDuyet(int madh)
        {
            if (!SessionHelper.CheckSession())
                return RedirectToAction("Login", "Login");
            if (!(SessionHelper.GetQuyenSD() == 2))
                return HttpNotFound();
            {
                var dh = db_ns.DONHANGs.SingleOrDefault(s => s.MADH == madh);
                var ctdh = db_ns.CHITIETDONDATs.Where(s => s.MADH == dh.MADH).ToList();
                return View(ctdh);
            }            
        }

        [HttpPost]
        public ActionResult LuuDuyetDonHang(int madh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Truy vấn sản phẩm từ cơ sở dữ liệu dựa trên.
                    DONHANG existingDH = db_ns.DONHANGs.SingleOrDefault(s => s.MADH == madh);

                    if (existingDH == null)
                    {
                        return HttpNotFound(); // Trả về trang lỗi 404 nếu sản phẩm không tồn tại
                    }

                    // Cập nhật thông tin sản phẩm từ dữ liệu được gửi từ form
                    existingDH.TRANGTHAI = 1;
                    db_ns.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    return Json(new { success = true, message = "" });
                    /*return RedirectToAction("DuyetDon");*/ // Điều hướng đến danh sách sản phẩm sau khi cập nhật thành công
                }
                catch (Exception)
                {
                    // Xử lý lỗi nếu có
                    return HttpNotFound(); // Hiển thị form chỉnh sửa sản phẩm lại với thông báo lỗi
                }
            }

            return RedirectToAction("DuyetDon"); // Hiển thị form chỉnh sửa sản phẩm lại nếu thông tin không hợp lệ
        }
    }
}