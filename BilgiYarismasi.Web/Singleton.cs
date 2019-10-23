using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web
{
    public sealed class YarismaS
    {
        private static YarismaS instance = null;

        private YarismaS()
        {
        }

        public static YarismaS Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new YarismaS();
                }
                return instance;
            }
        }
    }
}