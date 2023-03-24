using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebAuthT4.Areas.Identity.Data;

public class WebAuthUser : IdentityUser
{
    [Column("UserIndex")]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "User Index")]
    public int UserIndex { get; set; }

    [Column("UserFullName")]
    [StringLength(50, ErrorMessage = "Name should be shorter than 50 symbols. Consider shortening it.")]
    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Full Name")]
    public string? UserFullName { get; set; }

    [Column("LastLoginTime")]
    [Display(Name = "Last Login Time")]
    public DateTime LastLoginTime { get; set; }

    [Column("RegistrationTime")]
    [Display(Name = "Registration Time")]
    public DateTime RegistrationTime { get; set; }

    [Column("UserStatus")]
    [Display(Name = "Current Status")]
    public string? UserStatus { get; set; }

}

