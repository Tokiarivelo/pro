using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EssaiWeb.Models;
using EssaiWeb.ViewModels;
using Microsoft.Ajax.Utilities;

namespace EssaiWeb.Controllers
{
    public class MoviesController : Controller
    {
        // GET: Movies/random
        public ActionResult Random()
        {
            var movie = new Movie {Name = "Shrek!!"};
            var customers = new List<Customers>
            {
                new Customers {Name = "Customer 1"},
                new Customers {Name = "Customer 2"}
            };

            var viewModel = new RandomMovieViewModel
            {
                Movie = movie,
                Customerses = customers
            };
            return View(viewModel);
            //return Content("Hello MVC");
            //return HttpNotFound();
            //return new EmptyResult();
            //return RedirectToAction("Index", "Home", new {page = 1, sortBy = "name"});
        }

        public ActionResult Edit(int Id)
        {
            return Content(@"id = " + Id);
        }

        public ActionResult Index(int? pageIndex, string sorteBy)
        {
            if (!pageIndex.HasValue)
                pageIndex = 1;
            if (sorteBy.IsNullOrWhiteSpace())
                sorteBy = "Name";
            return Content($@"Page Index = {pageIndex}, sort by = {sorteBy}");
        }


        [Route("moi/{year}/{month:regex(\\d{2}):range(10,11)}")]
        public ActionResult ByDate(int year, int month)
        {
            return Content(year +"/"+month);
        }
    }
}