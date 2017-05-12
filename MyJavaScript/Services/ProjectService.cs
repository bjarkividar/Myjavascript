using MyJavaScript.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyJavaScript.Models;
using System.Data.Entity;
using System.Net.Mail;

namespace MyJavaScript.Models
{
    public class ProjectService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static ProjectService _instance;

        public static ProjectService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProjectService();
                return _instance;
            }
        }

        private List<Project> _projects = null;

        private List<InvitedUser> _invitedUsers = null;

        private ProjectService()
        {
            _projects = db.Projects.ToList();
            _invitedUsers = db.InvitedUsers.ToList();
        }

        public IEnumerable<Project> GetAllProjects(string username)
        {
            IEnumerable<int> ids = GetIds(username);

            return _projects.Where(t => ids.Contains(t.ID));
        }

        public IEnumerable<Project> GetMyProjects(string username)
        {
            IEnumerable<Project> result = from project in _projects
                                          where project.UserID == username
                                          orderby project.Title
                                          select project;
            return result.ToList();
        }

        public IEnumerable<Project> GetSharedProjects(string username)
        {
            IEnumerable<int> ids = GetIds(username);

            IEnumerable<Project> result = _projects.Where(t => ids.Contains(t.ID));
            result = from project in result
                     where (project.UserID != username)
                     select project;
            return result.ToList();
        }

        public bool CheckIfExist(Project p)
        {

            IEnumerable<Project> result = from proj in _projects
                                          where (proj.Title == p.Title) && (proj.UserID == p.UserID)
                                          select proj;
            if (result.FirstOrDefault() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void AddProject(Project p)
        {
            Instance._projects.Add(p);
            db.Projects.Add(p);
            db.SaveChanges();
            InvitedUser user = new InvitedUser() { Name = p.UserID, ProjectID = p.ID };
            Instance._invitedUsers.Add(user);
            FileService.Instance.AddFile(new File() { Title = "index", ProjectID = p.ID, ContentType = "JavaScript" });
            db.InvitedUsers.Add(user);
            db.SaveChanges();
        }

        public Project FindProject(int id)
        {
            Project project = (from p in _projects
                               where p.ID == id
                               select p).FirstOrDefault();
            return project;
        }

        public void Edit(Project project)
        {

            Project p = Instance.FindProject(project.ID);
            p.Title = project.Title;

        }

        public void DeleteProject(Project p)
        {
            IEnumerable<InvitedUser> invitations = from users in _invitedUsers
                                                   where users.ProjectID == p.ID
                                                   select users;

            _invitedUsers.RemoveAll(user => user.ProjectID == p.ID);
            _projects.Remove(p);
            FileService.Instance.DeleteFilesFromProject(p.ID);
            db.InvitedUsers.RemoveRange(invitations);
            db.Projects.Remove(p);
            db.SaveChanges();

        }

        public void LeaveProject(Project p, string name)
        {
            IEnumerable<InvitedUser> invitations = from user in db.InvitedUsers
                                                   where (user.ProjectID == p.ID) && (user.Name == name)
                                                   select user;
            db.InvitedUsers.RemoveRange(invitations);
            _invitedUsers.RemoveAll(user => (user.ProjectID == p.ID && user.Name == name));
            db.SaveChanges();
        }

        public bool InviteToProject(InvitedUser user)
        {
            if ((db.Users.Any(x => x.UserName == user.Name)) && (!_invitedUsers.Contains(user)))
            {
                db.InvitedUsers.Add(user);
                db.SaveChanges();
                _invitedUsers.Add(user);
                try
                {
                    using (MailMessage message = new MailMessage())
                    {
                        message.To.Add(user.Name);
                        message.Subject = "Invitation to a project.";
                        message.Body = "You have been invited to edit a project in MyJavascript";
                        using (SmtpClient client = new SmtpClient())
                        {
                            client.EnableSsl = true;
                            client.Send(message);
                        }
                    }
                }
                catch (Exception ex) { }
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<int> GetIds(string name)
        {
            IEnumerable<int> result = from users in _invitedUsers
                                      where (users.Name == name)
                                      select users.ProjectID;
            return result;
        }
    }
}