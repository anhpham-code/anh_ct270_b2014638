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
    
    public partial class DIACHI
    {
        public int MADIACHI { get; set; }
        public string LOAIDIACHI { get; set; }
        public string CHITIET { get; set; }
        public Nullable<int> MATK { get; set; }
    
        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
