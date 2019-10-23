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
    
    public partial class Masa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Masa()
        {
            this.MasaKullanici = new HashSet<MasaKullanici>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid KonuId { get; set; }
        public int SoruSayisi { get; set; }
        public int SureDk { get; set; }
        public int KisiSayisi { get; set; }
        public int Tip { get; set; }
        public Nullable<System.DateTime> BaslangicTarihi { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
    
        public virtual Konu Konu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasaKullanici> MasaKullanici { get; set; }
    }
}