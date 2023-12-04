using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_nong_san.Models
{
    public class ProductViewModel
    {
        public int MASP { get; set; }
        public string TENSP { get; set; }
        public Nullable<long> GIA { get; set; }
        public Nullable<int> BANCHAY { get; set; }
        public string HINH { get; set; }
    }

}