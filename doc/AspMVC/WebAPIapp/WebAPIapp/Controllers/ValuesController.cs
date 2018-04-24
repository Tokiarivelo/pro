using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPIapp.Models;

namespace WebAPIapp.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<Customer> Get()
        {
            return new List<Customer>
            {
                new Customer{CustomerId = 1, CustomerName = "Moi1"},
                new Customer { CustomerId = 2, CustomerName = "Moi2" }
            };
        }

        // GET api/values/5
        public Customer Get(int id)
        {
            if (id == 1)
            {
                return new Customer{CustomerId = 1, CustomerName = "Moi1"};
            }
            if (id == 2)
            {
                return new Customer { CustomerId = 2, CustomerName = "Moi2" };
            }
            return null;
        }

      
    }
}
