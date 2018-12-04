using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using piler_inc.Models;

namespace piler_inc.Controllers
{
    public class AccountsController : Controller
    {
        private capstone_mwdEntities db = new capstone_mwdEntities();

        public ActionResult user_main()
        {
            return View();
        }
        // GET: tbl_accounts
        public ActionResult Index()
        {
            var tbl_accounts = db.tbl_accounts.Include(t => t.tbl_companies);
            return View(tbl_accounts.ToList());
        }

        // GET: tbl_ccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(id);
            if (tbl_accounts == null)
            {
                return HttpNotFound();
            }
            return View(tbl_accounts);
        }

        // GET: tbl_accounts/Create
        public ActionResult Create()
        {
            ViewBag.company_id = new SelectList(db.tbl_companies, "company_id", "company_name");
            return View();
        }

        // POST: tbl_accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "account_id,account_email,account_status,account_password,account_type,company_id")] tbl_accounts tbl_accounts)
        {
            if (ModelState.IsValid)
            {
                db.tbl_accounts.Add(tbl_accounts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.company_id = new SelectList(db.tbl_companies, "company_id", "company_name", tbl_accounts.company_id);
            return View(tbl_accounts);
        }

        // GET: tbl_accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(id);
            if (tbl_accounts == null)
            {
                return HttpNotFound();
            }
            ViewBag.company_id = new SelectList(db.tbl_companies, "company_id", "company_name", tbl_accounts.company_id);
            return View(tbl_accounts);
        }

        // POST: tbl_accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "account_id,account_email,account_status,account_password,acoununt_type,company_id")] tbl_accounts tbl_accounts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_accounts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.company_id = new SelectList(db.tbl_companies, "company_id", "company_name", tbl_accounts.company_id);
            return View(tbl_accounts);
        }

        // GET: tbl_accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(id);
            if (tbl_accounts == null)
            {
                return HttpNotFound();
            }
            return View(tbl_accounts);
        }

        // POST: tbl_accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(id);
            db.tbl_accounts.Remove(tbl_accounts);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Login()
        {
            //var tbl_accounts = db.tbl_accounts.Include(t => t.tbl_companies);
            return View();
        }

        [Route("login")]
        [HttpPost]
        public string Login(string username, string password)
        {

            //tbl_accounts accounts = new tbl_accounts();
            var account = db.tbl_accounts
                .Where(table => table.account_email == username)
                .Where(table => table.account_password == password)
                .Where(table => table.account_status == 1)
                .ToList();
            try
            {
                Session["Account_id"] = account[0].account_id;
                Session["Account_type"] = account[0].account_type;
                return account[0].account_type.ToString();
            }
            catch
            {
                return "9";
            }
        }


        // GET: tbl_accounts/Create
        public ActionResult Registration()
        {
            ViewBag.company_id = new SelectList(db.tbl_companies, "company_id", "company_name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Include = "account_id,account_email,account_status,account_password,account_type,company_id")] tbl_accounts tbl_accounts)
        {
            tbl_accounts.account_type = int.Parse(Request.Form["account_type"]);
            tbl_accounts.account_status = 0;

            var a = new tbl_companies();
            a.company_name = Request.Form["tbl_companies.company_name"];
            a.company_address = Request.Form["tbl_companies.company_address"];

            if (ModelState.IsValid)
            {

                db.tbl_companies.Add(a);
                db.SaveChanges();
                tbl_accounts.company_id = db.tbl_companies.Max(u => u.company_id);
                db.tbl_accounts.Add(tbl_accounts);
                db.SaveChanges();
                return RedirectToAction("./../Home/");
            }

            ViewBag.company_id = new SelectList(db.tbl_companies, "company_id", "company_name", tbl_accounts.company_id);
            return View(tbl_accounts);
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return Redirect("~/");
        }

        [Route("create_account")]
        [HttpPost]
        public void create_account(string fName, string lName, string userame, string password, int account_type, string img)
        {
            string image = img == "" ? @"\\upload.png" : img;

            string[] words = image.Split('\\');
            tbl_accounts account = new tbl_accounts();
            account.account_email = userame;
            account.account_status = 1;
            account.account_password = password;
            account.account_type = account_type;
            account.account_img = words[2];
            account.company_id = 1;

            db.tbl_accounts.Add(account);
            db.SaveChanges();

            var y = db.tbl_accounts.OrderByDescending(u => u.account_id).FirstOrDefault().account_id;

            tbl_personalInformations personal = new tbl_personalInformations();
            personal.account_id = y;
            personal.personal_firstName = fName;
            personal.personal_lastName = lName;

            db.tbl_personalInformations.Add(personal);
            db.SaveChanges();
        }

        public void create_account_nonUser(string fName, string lName, string userame, string password, int account_type, string img, string compName, string bankName, string accountNumber, string files, string address)
        {
            string image = img == "" ? @"\\upload.png" : img;
            string file = files == "" ? @"\\upload.png" : files;
            string[] words = image.Split('\\');
            string[] docu = file.Split('\\');

            tbl_companies comp = new tbl_companies();
            comp.company_name = compName;
            comp.company_address = address;
            db.tbl_companies.Add(comp);
            db.SaveChanges();

            var x = db.tbl_companies.OrderByDescending(u => u.company_id).FirstOrDefault();

            tbl_accounts account = new tbl_accounts();
            account.account_email = userame;
            account.account_status = 0;
            account.account_password = password;
            account.account_type = account_type;
            account.account_img = words[2];
            account.company_id = x.company_id;
            account.account_bankName = bankName;
            account.account_bankNum = accountNumber;
            db.tbl_accounts.Add(account);
            db.SaveChanges();

            var y = db.tbl_accounts.OrderByDescending(u => u.account_id).FirstOrDefault().account_id;

            tbl_personalInformations personal = new tbl_personalInformations();
            personal.account_id = y;
            personal.personal_firstName = fName;
            personal.personal_lastName = lName;

            db.tbl_personalInformations.Add(personal);
            db.SaveChanges();

            tbl_requirements req = new tbl_requirements();
            req.account_id = y;
            req.requirement_name = "supporting document";
            req.requirement_dir = docu[2];
            db.tbl_requirements.Add(req);
            db.SaveChanges();
        }

        public JsonResult EditImage()
        {
            int user = int.Parse(Session["Account_id"].ToString());
            var ctr = db.tbl_accounts.Find(user).account_id;


            string targetPathProfilePic = Server.MapPath(@"\images\accounts\" + ctr + @"\profile\");

            System.IO.DirectoryInfo di = new DirectoryInfo(targetPathProfilePic);

            if (System.IO.Directory.Exists(targetPathProfilePic))
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(targetPathProfilePic);
            }

            for (int i = 0; i < 1; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file

                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(targetPathProfilePic + fileName); //File will be saved in application root
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }

        public JsonResult UploadImg()
        {
            var ctr = db.tbl_accounts.OrderByDescending(u => u.account_id).FirstOrDefault().account_id + 1;
            string targetPathProfilePic = Server.MapPath(@"\images\accounts\" + ctr + @"\profile\");

            System.IO.DirectoryInfo di = new DirectoryInfo(targetPathProfilePic);

            if (System.IO.Directory.Exists(targetPathProfilePic))
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(targetPathProfilePic);
            }

            for (int i = 0; i < 1; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file

                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(targetPathProfilePic + fileName); //File will be saved in application root
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }

        public JsonResult UploadDocu()
        {
            var ctr = db.tbl_accounts.OrderByDescending(u => u.account_id).FirstOrDefault().account_id + 1;
            string targetPathDoc = Server.MapPath(@"\images\accounts\" + ctr + @"\document\");
            System.IO.DirectoryInfo di = new DirectoryInfo(targetPathDoc);

            if (System.IO.Directory.Exists(targetPathDoc))
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(targetPathDoc);
            }

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(targetPathDoc + fileName); //File will be saved in application root
            }

            return Json("Uploaded " + Request.Files.Count + " files");
        }

        public ActionResult registration_success()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult account_request()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [Route("update_user")]
        [HttpPost]
        public void update_user(string fName, string lName, string userame, string password, string img, string bankName, string accountNumber)
        {
            int user = int.Parse(Session["Account_id"].ToString());
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(user);

            var personal = db.tbl_personalInformations.Where(x => x.account_id == user).FirstOrDefault();
            personal.personal_firstName = fName;
            personal.personal_lastName = lName;

            tbl_accounts.account_email = userame;
            tbl_accounts.account_password = password;
            tbl_accounts.account_bankName = bankName;
            tbl_accounts.account_bankNum = accountNumber;
            tbl_accounts.account_password = password;

            if (img != "")
            {
                string[] words = img.Split('\\');
                tbl_accounts.account_img = words[2];
            }

            db.SaveChanges();
        }

        [Route("update_nonuser")]
        [HttpPost]
        public void update_nonuser(string fName, string lName, string userame, string password, string img, string bankName, string accountNumber, string compName, string compAddress)
        {
            //compName: compName, bankName: bankName 
            int user = int.Parse(Session["Account_id"].ToString());
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(user);

            var personal = db.tbl_personalInformations.Where(x => x.account_id == user).FirstOrDefault();
            personal.personal_firstName = fName;
            personal.personal_lastName = lName;

            tbl_accounts.account_email = userame;
            tbl_accounts.account_password = password;
            tbl_accounts.account_bankName = bankName;
            tbl_accounts.account_bankNum = accountNumber;
            tbl_accounts.account_password = password;

            var comp = db.tbl_companies.Where(x => x.company_id == tbl_accounts.company_id).FirstOrDefault();
            comp.company_name = compName;
            comp.company_address = compAddress;

            if (img != "")
            {
                string[] words = img.Split('\\');
                tbl_accounts.account_img = words[2];
            }

            db.SaveChanges();
        }

        [Route("update_admin")]
        [HttpPost]
        public void update_admin(string userame, string password)
        {
            //compName: compName, bankName: bankName 
            int user = int.Parse(Session["Account_id"].ToString());
            tbl_accounts tbl_accounts = db.tbl_accounts.Find(user);

            var personal = db.tbl_personalInformations.Where(x => x.account_id == user).FirstOrDefault();


            tbl_accounts.account_email = userame;
            tbl_accounts.account_password = password;
            tbl_accounts.account_password = password;

            db.SaveChanges();
        }

        [Route("update_nonuser")]
        [HttpPost]
        public void update_viaAdmin(string fName, string lName, string userame, string password, string img, string bankName, string accountNumber, string compName, string compAddress, string id, string account_status)
        {
            //compName: compName, bankName: bankName 

            tbl_accounts tbl_accounts = db.tbl_accounts.Find(int.Parse(id));
            int user = int.Parse(id);
            var personal = db.tbl_personalInformations.Where(x => x.account_id == user).FirstOrDefault();
            personal.personal_firstName = fName;
            personal.personal_lastName = lName;

            tbl_accounts.account_email = userame;
            tbl_accounts.account_password = password;
            tbl_accounts.account_bankName = bankName;
            tbl_accounts.account_bankNum = accountNumber;
            tbl_accounts.account_password = password;
            tbl_accounts.account_status = int.Parse(account_status);

            var comp = db.tbl_companies.Where(x => x.company_id == tbl_accounts.company_id).FirstOrDefault();
            comp.company_name = compName;
            comp.company_address = compAddress;

            if (img != "")
            {
                string[] words = img.Split('\\');
                tbl_accounts.account_img = words[2];
            }

            db.SaveChanges();
        }


        public JsonResult EditImageAdmin(string id)
        {
            int user = int.Parse(id);
            var ctr = db.tbl_accounts.Find(user).account_id;


            string targetPathProfilePic = Server.MapPath(@"\images\accounts\" + ctr + @"\profile\");

            System.IO.DirectoryInfo di = new DirectoryInfo(targetPathProfilePic);

            if (System.IO.Directory.Exists(targetPathProfilePic))
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(targetPathProfilePic);
            }

            for (int i = 0; i < 1; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file

                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(targetPathProfilePic + fileName); //File will be saved in application root
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }
    }
}
