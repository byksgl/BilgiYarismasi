//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BilgiYarismasi.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class VKullaniciPuanTipli
    {
        public Nullable<System.Guid> Id { get; set; }
        public System.Guid KullaniciId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public int Tip { get; set; }
        public Nullable<int> PUAN { get; set; }
    }
}
