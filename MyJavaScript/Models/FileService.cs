using MyJavaScript.Models.Entity;
using System;
using System.Collections.Generic;
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
		public IEnumerable<File> Files(int id)
		{
			var files = from f in _files
					   where id == f.ProjectID
					   select f;
			return files;
		}
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
		public void Edit (File file)
		{
			File f = Instance.FindFile(file.ID);
			f.Content = file.Content;
			f.Title = file.Title;
		}
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
	}
}