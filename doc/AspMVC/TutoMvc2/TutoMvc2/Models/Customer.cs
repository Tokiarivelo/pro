﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TutoMvc2.Models
{
    public class Customer
    {
        [Key]
        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }
    }
}