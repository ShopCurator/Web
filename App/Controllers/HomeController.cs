using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Data;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string search)
        {
            if(!string.IsNullOrEmpty(search) &&  search.Length > 3)
            {
                var data = DataManager.GetAsync(search);
                if (data != null)
                    ViewData["Product"] = data;
            }

           
            //DataManager.harvynormanDataTemplate();
            //DataManager.jbhifiDataTemplate();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            var dt = new DataManager();
            return View();
        }
        
        public IActionResult Contact()
        {           
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
