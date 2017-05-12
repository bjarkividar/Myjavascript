using MyJavaScript.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace MyJavaScript.Models
{
    public class FileService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static FileService _instance;

        public static FileService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FileService();
                return _instance;
            }
        }

        private List<File> _files = null;

        private FileService()
        {
            _files = db.Files.ToList();
        }
		// Gets all Files connected to a particular Project.
        public IEnumerable<File> Files(int id)
        {
            var files = from f in _files
                        where id == f.ProjectID
                        select f;
            return files;
        }
		// Adds a File to a project.
        public void AddFile(File f)
        {
            _files.Add(f);
            db.Files.Add(f);
            db.SaveChanges();

        }

        public File FindFile(int id)
        {
            File file = (from f in _files
                         where f.ID == id
                         select f).FirstOrDefault();
            return file;
        }

        public void Edit(File file)
        {
			File f = Instance.FindFile(file.ID);
            f.Content = file.Content;
            f.Title = file.Title;
        }
		// Check if the file is in database.
        public bool FileExists(File file)
        {
            var result = (from files in _files
                          where (files.Title == file.Title) && (files.ProjectID == file.ProjectID)
                          select files).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DeleteFilesFromProject(int id)
        {
            IEnumerable<File> files = from file in _files
                                      where file.ProjectID == id
                                      select file;

            db.Files.RemoveRange(files);
            db.SaveChanges();
            _files.RemoveAll(file => file.ProjectID == id);
        }

        public void DeleteFile(int id)
        {
            var file = (from f in _files
                        where f.ID == id
                        select f).FirstOrDefault();

            db.Files.Remove(file);
            db.SaveChanges();
            _files.Remove(file);
        }

        public File AddExtension(File f)
        {
            if (f.Title.EndsWith(".js") || f.Title.EndsWith(".cs") || f.Title.EndsWith(".html"))
            {
                return f;
            }
            else
            {
                string extension = null;
                switch (f.ContentType)
                {
                    case "JavaScript":
                        extension = ".js";
                        break;
                    case "Css":
                        extension = ".css";
                        break;
                    case "HTML":
                        extension = ".html";
                        break;
                }
                f.Title += extension;
                return f;
            }
        }
    }
}