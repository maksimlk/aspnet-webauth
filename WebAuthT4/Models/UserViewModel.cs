using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAuthT4.Models
{
	public class UserViewModel
	{
		[Display(Name = "User Index")]
		public string UserID { get; set; }
		[Display(Name = "ID")]
		public int UserIndex { get; set; }
		
		[Display(Name = "Full Name")]
		public string? UserFullName { get; set; }
		[Display(Name ="Email")]
		public string Email { get; set; }
		[Display(Name = "Last Login Time")]
		public DateTime LastLoginTime { get; set; }
		[Display(Name = "Registration Time")]
		public DateTime RegistrationTime { get; set; }
		[Display(Name = "Current Status")]
		public string? UserStatus { get; set; }

		public SelectListItem Selection { get; set; }
	}
}
