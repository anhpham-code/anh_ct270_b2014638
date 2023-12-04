using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web_nong_san.Code;

namespace web_nong_san.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        QL_NongSanSachEntities db = new QL_NongSanSachEntities();
        int strQuyenSD;
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(TAIKHOAN taiKhoan)
        {
            var myUser = db.TAIKHOANs.FirstOrDefault(u => u.TENDANGNHAP == taiKhoan.TENDANGNHAP && u.MATKHAU == taiKhoan.MATKHAU);
            if (myUser != null)
            {
                strQuyenSD = myUser.QUYEN;
                if (strQuyenSD == 1)
                {
                    SessionHelper.SetSession(myUser);
                    return RedirectToAction("DanhSachHangSanXuat", "HangSanXuat");
                }
                else
                {
                    if (strQuyenSD == 2)
                    {
                        SessionHelper.SetSession(myUser);
                        return RedirectToAction("DanhSachSanPham", "SanPham");
                    }
                    else
                    {
                        if (strQuyenSD == 3)
                            SessionHelper.SetSession(myUser);
                        return RedirectToAction("DanhSachSanPhamNguoiDung", "NguoiDung");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng, vui lòng thử lại!");

            }
            return View(taiKhoan);
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public ActionResult Register(TAIKHOAN taikhoan)
        {
            var user = db.TAIKHOANs.FirstOrDefault(u => u.TENDANGNHAP == taikhoan.TENDANGNHAP);
            if (user != null)
            {
                ModelState.AddModelError("", "Tài khoản đã tồn tại");
            }
            else
            {
                string confirmpw = Request.Form.Get("confirmpw").ToString().Trim();
                if (confirmpw != taikhoan.MATKHAU)
                {
                    ModelState.AddModelError("", "Mật khẩu không khớp");
                }
                else
                {
                    taikhoan.QUYEN = 3;                    
                    db.TAIKHOANs.Add(taikhoan);
                    db.SaveChanges();
                    var dh = new DONHANG
                    {
                        MATK = taikhoan.MATK,
                    };
                    SessionHelper.SetSession(taikhoan);
                    return RedirectToAction("DanhSachSanPhamNguoiDung", "NguoiDung");
                }
                SessionHelper.SetSession(taikhoan);
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

    public ActionResult ForgetAccount()
    {
        return View();
    }
    public ActionResult LogOut()
    {
        Session.RemoveAll();
        return RedirectToAction("Login", "Login");
    }
}
}