using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExerciseASP.Models;
using ExerciseASP.ViewModel;

namespace ExerciseASP.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer

        public static CustomerViewModel ListCust { get;  set; }

        public ActionResult CustomeResult()
        {
            var customers = new List<Customers>
            {
                new Customers {Id = 1,CustomerName = "John Smith"},
                new Customers {Id = 2,CustomerName = "Marry Williams"}
            };

            ListCust = new CustomerViewModel
            {
                ListCustomer = customers
            };
            return View(ListCust);
        }

       [Route("Customers/Detail/{id:range(1, 2)}")]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                id = 1;
            var detail = ListCust.ListCustomer.FirstOrDefault(x => x.Id == id);
            return View(detail);
        }
    }
}