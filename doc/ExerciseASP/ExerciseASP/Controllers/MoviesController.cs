using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExerciseASP.Models;
using ExerciseASP.ViewModel;

namespace ExerciseASP.Controllers
{
    public class MoviesController : Controller
    {
        // GET: Movies
        public ActionResult MovieActionResult()
        {
            var movies = new List<Movies>
            {
                new Movies {Id = 1, MovieName = "Shrek"},
                new Movies {Id = 1, MovieName = "Wall-e"}
            };

            var moviesList = new MoviesViewModel
            {
                MoviesList = movies
            };

            return View(moviesList);
        }
    }
}