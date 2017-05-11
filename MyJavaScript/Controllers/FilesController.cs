using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;

using System.Web.Mvc;
using MyJavaScript.Models;
using MyJavaScript.Models.Entity;


namespace MyJavaScript.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ContentType;
        // GET: Files
        public ActionResult Index(int? id, string search)
        {
            var project = (from p in db.Projects
                           where id.Value == p.ID
                           select p.Title).FirstOrDefault();

            ViewBag.Name = project;

            var file = from f in db.Files
                       where id.Value == f.ProjectID
                       select f;

            if (!String.IsNullOrEmpty(search))
            {
                file = file.Where(x => x.Title.Contains(search));
                return View(file);
            }
            return View(db.Files.Where(x => x.ProjectID.Equals(id.Value)).ToList());
        }

        // GET: Files/Create
        public ActionResult Create(int? id)
        {
            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ContentType,Data,ProjectID")] File file)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<File> result = from files in db.Files
                                           where (files.Title == file.Title) && (files.ProjectID == file.ProjectID)
                                           select files;
                if (result.FirstOrDefault() == null)
                {
                    db.Files.Add(file);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = file.ProjectID });
                }
                else
                {
                    ModelState.AddModelError("Title", "There is already a file with this name in this project.");
                }
            }
            return View(file);
        }
        // GET: Files/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.Files.Find(id.Value);
            if (file == null)
            {
                return HttpNotFound();
            }

            //ViewBag.Code = "alert('Hello World');";
            string code = file.Content;
            ContentType = file.ContentType;

            ViewBag.ContentType = ContentType;
            ViewBag.Code = code;
            ViewBag.DocumentID = id;
            //Sækja kóðann úr gagnagrunni og senda hérna inn í breytuna, í staðin fyrir Hello World

            return View(file);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveCode(File model, int? id)
        {
            File file = db.Files.Find(id.Value);
            if (file == null)
            {
                return HttpNotFound();
            }
            file.Content = model.Content;
            db.Entry(file).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = model.ID });
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ContentType,Data,ProjectID")] File file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = file.ProjectID });
            }
            return View(file);
        }

        // GET: Files/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            File file = db.Files.Find(id);

            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            File file = db.Files.Find(id);
            db.Files.Remove(file);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = file.ProjectID });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public PartialViewResult GetDeletePartial(int id)
        {
            var deleteItem = db.Files.Find(id);

            return PartialView("Delete", deleteItem);
        }

        public ActionResult EditInfo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.Files.Find(id.Value);
            if (file == null)
            {
                return HttpNotFound();
            }

            return View(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInfo([Bind(Include = "ID,Title,ContentType,Data,ProjectID")] File file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = file.ProjectID });
            }
            return View(file);
        }

		}
		/*
		public ActionResult UploadFile (string myUploader)
		{
			using (CuteWebUI.MvcUploader uploader = new CuteWebUI.MvcUploader(System.Web.HttpContext.Current))
			{
				uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
				uploader.Name = "myuploader";
				uploader.AllowedFileExtensions = "*.jpg,*.gif,*.png,*.bmp,*.zip,*.rar";
				uploader.InsertText = "Select a file to upload";
				ViewData["uploaderhtml"] = uploader.Render();
			}
			return View();
		}
		*/
		
    }

