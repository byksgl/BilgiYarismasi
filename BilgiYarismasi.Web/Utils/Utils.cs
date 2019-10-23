using BilgiYarismasi.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web
{
    public static class Utils
    {
        public static Masa AktifMasa
        {
            get
            {
                if (HttpContext.Current.Session[SessionNames.MASA] == null) return null;

                Guid masaId = HttpContext.Current.Session[SessionNames.MASA].ToString().toGuid();
                if (masaId==Guid.Empty)
                {
                    return null;
                }

                using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
                {
                    var masa = ent.Masa.Where(p => p.Id == masaId).FirstOrDefault();

                    if (masa == null) return null;

                    if (masa.BitisTarihi.HasValue && masa.BitisTarihi.Value<=DateTime.Now)
                    {
                        HttpContext.Current.Session[SessionNames.MASA] = null;
                        HttpContext.Current.Session[SessionNames.KONU] = null;
                        return null;
                    }

                    return masa;
                }
            }
        }

        public static string getAyar(string ayarAdi)
        {
            using (BilgiYarismasiEntities2 ent = new BilgiYarismasiEntities2())
            {
                
            }

            return string.Empty;
        }
    }
}