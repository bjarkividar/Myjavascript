using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyJavaScript.Models.Entity
{
	public class File
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string ContentType { get; set; }
		public byte[] Data { get; set; }
		public int ProjectID { get; set; }
	}
}