﻿using BilgiYarismasi.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilgiYarismasi.Web.Models
{
    public class QuizIndexViewModel
    {
        public LinkedList<Konu> konular { get; set; }


        public QuizIndexViewModel()
        {
            konular = new LinkedList<Konu>();
        }
    }
}