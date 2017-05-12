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
		public ActionResult Index(string search)
        {
            ViewBag.Name = "All Projects";

			IEnumerable<Project> result = ProjectService.Instance.GetAllProjects(System.Web.HttpContext.Current.User.Identity.Name);
            if(String.IsNullOrEmpty(search))
            {
                return View(result.ToList());
            }
            result = result.Where(x => x.Title.Contains(search));
            return View(result);
        }

		public ActionResult MyProjects(string search)
		{
            ViewBag.Name = "My Projects";

			IEnumerable<Project> result = ProjectService.Instance.GetMyProjects(System.Web.HttpContext.Current.User.Identity.Name);
            if(String.IsNullOrEmpty(search))
            {
                return View("Index", result.ToList());
            }
            result = result.Where(x => x.Title.Contains(search));
            return View("Index", result.ToList());
        }

		public ActionResult SharedProjects(string search)
		{
            ViewBag.Name = "Shared with me";

			IEnumerable<Project> result = ProjectService.Instance.GetSharedProjects(System.Web.HttpContext.Current.User.Identity.Name);
            if(String.IsNullOrEmpty(search))
            {
                return View("Index", result.ToList());
            }
            result = result.Where(x => x.Title.Contains(search));
            return View("Index", result.ToList());
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
				if (!ProjectService.Instance.CheckIfExist(project))
				{
					ProjectService.Instance.AddProject(project);
				
					return RedirectToAction("Index");
				}
				else
				{
					ModelState.AddModelError("Title", "You already have a project with this name.");
				}				
			}
            return View(project);
        }

        /* Creates selected project with modal window.  */
        [HttpGet]
        public PartialViewResult GetCreatePartial(string title)
        {
            Project createProject = new Project()
            {
                Title = title
            };
            return PartialView("Create", createProject);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Project project = ProjectService.Instance.FindProject(id.Value);
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
				ProjectService.Instance.Edit(project);
                return RedirectToAction("Index");
            }
            return View(project);
        }

        /* User can edit name of project with modal window */
        [HttpGet]
        public PartialViewResult GetEditPartial(int id)
        {
            var editItem = ProjectService.Instance.FindProject(id);

            return PartialView("Edit", editItem);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Project project = ProjectService.Instance.FindProject(id.Value);
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
			Project project = ProjectService.Instance.FindProject(id);
			if (project.UserID == System.Web.HttpContext.Current.User.Identity.Name)
			{
				ProjectService.Instance.DeleteProject(project);
			}
			else
			{
				ProjectService.Instance.LeaveProject(project, System.Web.HttpContext.Current.User.Identity.Name);				
			}
			return RedirectToAction("Index");
        }

        /* Deletes selected projects with modal window. 
           When user clickes delete button the project will disappear from list  */
        [HttpGet]
        public PartialViewResult GetDeletePartial(int id)
        {
            var deleteItem = ProjectService.Instance.FindProject(id);

            return PartialView("Delete", deleteItem);
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
				if (ProjectService.Instance.InviteToProject(user))
				{
					return RedirectToAction("Index");
				}
				else
				{
					ViewBag.ErrorMessage = "User not in the system or already has access to the project";
				}
			}
			
			return View(user);
		}

        /* Shares project with another user, using email. When user clickes on share icon a modal window appears. 
           User will get an email that tells him that he's been invited to work on that particular project.    */
        [HttpGet]
        public PartialViewResult GetSharePartial(int id)
        {
            InvitedUser user = new InvitedUser
            {
                ProjectID = id
            };
            return PartialView("ShareProject", user);
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
