using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EssaiWeb.Models;

namespace EssaiWeb.ViewModels
{
    public class RandomMovieViewModel
    {
        public Movie Movie { get; set; }
        public List<Customers>  Customerses { get; set; }
    }
}