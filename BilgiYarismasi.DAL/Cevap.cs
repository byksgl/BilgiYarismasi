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
    
    public partial class Cevap
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cevap()
        {
            this.YarismaSoru = new HashSet<YarismaSoru>();
            this.YarismaSoruCevap = new HashSet<YarismaSoruCevap>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid SoruId { get; set; }
        public bool Dogrumu { get; set; }
        public string Cevabi { get; set; }
    
        public virtual Soru Soru { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YarismaSoru> YarismaSoru { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YarismaSoruCevap> YarismaSoruCevap { get; set; }
    }
}
