using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using piler_inc.Models;

namespace piler_inc.Controllers
{
    public class HomeController : Controller
    {
        private capstone_mwdEntities db = new capstone_mwdEntities();
        public ActionResult Index()
        {
            string input = Request.Form["search-input"];
            var x = input == null || input == "" ? db.tbl_products.ToList() : db.tbl_products.Where(i => i.product_name.Contains(input) || i.product_desc.Contains(input)).ToList();
            return View(x);
        }
        public ActionResult Login()
        {
           return View();
        }

            public ActionResult Market()
        {
            string input = Request.Form["search-input"];
            var x = input == null || input == "" ? db.tbl_products.ToList() : db.tbl_products.Where(i => i.product_name.Contains(input) || i.product_desc.Contains(input)).ToList();
            return View(x);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult faq()
        {
            return View();
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_products tbl_products = db.tbl_products.Find(id);
            ViewBag.products = db.tbl_products.ToList();

            if (tbl_products == null)
            {
                return HttpNotFound();
            }
            return View(tbl_products);
        }
    }
}