using BilgiYarismasi.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class MasaSecViewModel
    {
        public Guid KonuId { get; set; }
        public List<MasaModel> masalar { get; set; }


        public MasaSecViewModel()
        {
            masalar = new List<MasaModel>();
        }

    }

    public class MasaModel
    {
        public Guid Id { get; set; }
        public Masa masa { get; set; }
        public int KisiSayisi { get; set; }
    }
}