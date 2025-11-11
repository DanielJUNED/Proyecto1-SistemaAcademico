using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;

namespace SistemaAcademico.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5275/");
                var response =  client.GetAsync("weatherforecast");
                 
                return View(new List<string>());
            } 
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}