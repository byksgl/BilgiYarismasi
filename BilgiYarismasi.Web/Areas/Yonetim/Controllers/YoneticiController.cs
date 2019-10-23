using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BilgiYarismasi.Web.Areas.Yonetim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class YoneticiController : Controller
    {
        [Authorize(Roles ="Admin")]
        // GET: Yonetim/Yonetici
        public ActionResult Index()
        {
            return View();
        }
    }
}