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
		public void AddFile(File f)
		{
			Instance._files.Add(f);
			db.Files.Add(f);
			db.SaveChanges();

		}
		public void DeleteFile(int id)
		{
			IEnumerable<File> files = from file in _files
									  where file.ProjectID == id
									  select file;

			db.Files.RemoveRange(files);
			_files.RemoveAll(file => file.ProjectID == id);
		}
	}
}