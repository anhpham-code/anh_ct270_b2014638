//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace web_nong_san
{
    using System;
    using System.Collections.Generic;
    
    public partial class CHITIETDONDAT
    {
        public int MADH { get; set; }
        public int MASP { get; set; }
        public Nullable<int> SOLUONG { get; set; }
        public Nullable<long> THANHTIEN { get; set; }
        public Nullable<long> GIA { get; set; }
    
        public virtual DONHANG DONHANG { get; set; }
        public virtual SANPHAM SANPHAM { get; set; }
    }
}
