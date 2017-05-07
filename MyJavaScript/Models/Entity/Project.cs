using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyJavaScript.Models.Entity
{
	public class Project
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string UserID { get; set; }
		public virtual ICollection<InvitedUser> InvitedUsers { get; set; }

		public string Content { get; set; }


	}
}
