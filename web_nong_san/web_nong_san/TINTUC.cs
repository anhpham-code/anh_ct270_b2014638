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
    
    public partial class TINTUC
    {
        public int MATT { get; set; }
        public string TIEUDE { get; set; }
        public string URL { get; set; }
        public string HINH { get; set; }
        public Nullable<System.DateTime> NGAYDANG { get; set; }
        public string TOMTAT { get; set; }
        public string NOIDUNG { get; set; }
        public Nullable<int> MANHOM { get; set; }
        public Nullable<int> MATK { get; set; }
    
        public virtual NHOMTIN NHOMTIN { get; set; }
        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
