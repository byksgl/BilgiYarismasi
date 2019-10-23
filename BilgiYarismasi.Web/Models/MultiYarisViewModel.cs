using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class MultiYarisViewModel
    {
        public int SiraNo { get; set; }
        public string Sorusu { get; set; }
        public List<CevapKisaModel> cevaplar { get; set; }

        public Guid cevapId { get; set; }

        public int OncekiDogrumu { get; set; }

        public MultiYarisViewModel()
        {
            cevaplar = new List<CevapKisaModel>();
        }
    }
}