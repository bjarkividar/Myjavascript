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
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			else
			{
				Project project = ProjectService.Instance.FindProject(id.Value);
				ViewBag.Name = project.Title;

				IEnumerable<File> files = FileService.Instance.Files(id.Value);
				if (!String.IsNullOrEmpty(search))
				{
					files = files.Where(x => x.Title.Contains(search));
					return View(files);
				}
				return View(db.Files.Where(x => x.ProjectID.Equals(id.Value)).ToList());
			}
        }

		// GET: Files/Create
		public ActionResult Create(int? id)
		{

			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			return View();
		}

        // POST: Files/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ContentType,Data,ProjectID")] File file)
        {
            if (ModelState.IsValid)
            {
				file = FileService.Instance.AddExtension(file);
				if (!FileService.Instance.FileExists(file))
				{
					FileService.Instance.AddFile(file);
					
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
			File file = FileService.Instance.FindFile(id.Value);
            if (file == null)
            {
                return HttpNotFound();
            }

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
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			File file = FileService.Instance.FindFile(id.Value);
            if (file == null)
            {
                return HttpNotFound();
            }
            file.Content = model.Content;
            db.Entry(file).State = EntityState.Modified;
            db.SaveChanges();
			FileService.Instance.Edit(file);
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
			File file = FileService.Instance.FindFile(id.Value);

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
			File file = FileService.Instance.FindFile(id);
			FileService.Instance.DeleteFile(id);
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

        //Deletes selected files with a modal window
        [HttpGet]
        public PartialViewResult GetDeletePartial(int id)
        {
			var deleteItem = FileService.Instance.FindFile(id);

            return PartialView("Delete", deleteItem);
        }

        public ActionResult EditInfo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = FileService.Instance.FindFile(id.Value);
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
                file = FileService.Instance.AddExtension(file);
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
				FileService.Instance.Edit(file);
                return RedirectToAction("Index", new { id = file.ProjectID });
            }
            return View(file);
        }
    }
}

