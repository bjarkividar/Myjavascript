using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyJavaScript.Models;
using MyJavaScript.Models.Entity;
using Microsoft.AspNet.Identity;

namespace MyJavaScript.Controllers
{
	[Authorize]
	public class ProjectsController : Controller
    {
		private ApplicationDbContext db = new ApplicationDbContext();
		

		// GET: Projects
		public ActionResult Index()
        {
			IEnumerable<int> ids = from users in db.InvitedUsers
								   where (users.Name == System.Web.HttpContext.Current.User.Identity.Name)
								   select users.ProjectID;

			IEnumerable<Project> result = db.Projects.Where(t => ids.Contains(t.ID));
			return View(result.ToList());
        }

		public ActionResult MyProjects()
		{
			IEnumerable<Project> result = from project in db.Projects
										  where project.UserID == System.Web.HttpContext.Current.User.Identity.Name
										  orderby project.Title
										  select project;
			return View("Index", result.ToList());
		}
		public ActionResult SharedProjects()
		{
			IEnumerable<int> ids = from users in db.InvitedUsers
								   where (users.Name == System.Web.HttpContext.Current.User.Identity.Name)
								   select users.ProjectID;

			IEnumerable<Project> result = db.Projects.Where(t => ids.Contains(t.ID));
			result = from project in result
					 where (project.UserID != System.Web.HttpContext.Current.User.Identity.Name)
					 select project;
			return View("Index", result.ToList());
		}
		// GET: Projects/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,UserID")] Project project)
        {
            if (ModelState.IsValid)
            {
				project.UserID = System.Web.HttpContext.Current.User.Identity.Name;
				db.Projects.Add(project);
				InvitedUser user = new InvitedUser();
				user.Name = project.UserID;
				user.ProjectID = project.ID;
				db.InvitedUsers.Add(user);
				db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

			return View(project);

			
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,UserID")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
		
        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        /* POST: Projects/Delete/5 
		 If the user created the project he deletes the project and all files associated with the project,
		 but if another user created it he only deletes his connection to the project so it doesn't appear
		 on his page. */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
			if (project.UserID == System.Web.HttpContext.Current.User.Identity.Name)
			{
				IEnumerable<InvitedUser> invitations = from users in db.InvitedUsers
													   where users.ProjectID == id
													   select users;
				IEnumerable<File> files = from file in db.Files
													   where file.ProjectID == id
													   select file;
				db.Files.RemoveRange(files);
				db.InvitedUsers.RemoveRange(invitations);
				db.Projects.Remove(project);
			}
			else
			{
				IEnumerable<InvitedUser> invitations = from user in db.InvitedUsers
													   where (user.ProjectID == id) && (user.Name == System.Web.HttpContext.Current.User.Identity.Name)
													   select user;
				db.InvitedUsers.RemoveRange(invitations);
				
			}
			db.SaveChanges();
			return RedirectToAction("Index");
        }
		//Get
		public ActionResult ShareProject(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			return View();
		}

		/* POST: Projects/ShareProject
		Gives another user access to the project. */ 
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ShareProject(InvitedUser user)
		{
			if (ModelState.IsValid)
			{
				if(db.Users.Any(x => x.UserName == user.Name))
				{
					db.InvitedUsers.Add(user);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				else
				{
					ViewBag.ErrorMessage = "User not in the system";
				}
			}

			return View(user);
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

	}
}
