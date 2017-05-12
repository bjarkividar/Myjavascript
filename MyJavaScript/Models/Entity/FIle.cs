using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyJavaScript.Models.Entity
{
	public class File
	{
		public int ID { get; set; }
		[Required]
		public string Title { get; set; }
		public string ContentType { get; set; }
		public int ProjectID { get; set; }
		public string Content { get; set; }
	}
}