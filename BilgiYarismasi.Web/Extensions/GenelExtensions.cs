using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web
{
    public static class GenelExtensions
    {
        public static Guid toGuid(this string o)
        {
            Guid g;
            if (Guid.TryParse(o,out g))
            {
                return g;
            }
            return Guid.Empty;
        }
    }
}