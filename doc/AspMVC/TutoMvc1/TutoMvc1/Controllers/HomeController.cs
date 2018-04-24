using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TutoMvc1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Route("")]
        [Route("Home")]
        [Route("Home/Home")]
        public ActionResult GotoHome()
        {
            return View("MyHomePage");
        }
    }
}