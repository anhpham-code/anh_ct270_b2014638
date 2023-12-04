using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_nong_san.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Nullable<long> UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}