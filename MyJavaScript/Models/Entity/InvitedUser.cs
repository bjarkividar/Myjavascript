using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyJavaScript.Models.Entity
{
	public class InvitedUser
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int ProjectID { get; set; }
		public virtual Project Project { get; set; }
	}
}