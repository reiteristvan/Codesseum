using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codesseum.Web.Areas.Beta.Models;

namespace Codesseum.Web.Areas.Beta.Controllers
{
    public class SubscribeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(SubscribeModel model)
        {
            return Redirect("");
        }
    }
}