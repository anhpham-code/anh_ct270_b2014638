using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_nong_san.Code
{
    public class SessionHelper
    {
        public static void SetSession(TAIKHOAN taiKhoan)
        {
            HttpContext.Current.Session["taikhoan"] = taiKhoan;
        }

        public static TAIKHOAN GetSession()
        {
            var session = HttpContext.Current.Session["taikhoan"];
            if (session == null)
                return null;
            else
            {
                return session as TAIKHOAN;
            }
        }
        public static bool CheckSession()
        {
            var session = HttpContext.Current.Session["taikhoan"];
            if (session == null)
                return false;
            else
                return true;
        }
        public static int? GetQuyenSD()
        {
            var session = HttpContext.Current.Session["taikhoan"] as TAIKHOAN;
            return session.QUYEN;
        }

        public static string GetImages()
        {
            var session = HttpContext.Current.Session["taikhoan"] as TAIKHOAN;
            return session.HINH;
        }
    }
}