using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Transactions;
using WebAuthT4.Areas.Identity.Data;
using WebAuthT4.Data;
using WebAuthT4.Models;

namespace WebAuthT4.Pages
{
    [Authorize(Roles="ActiveUser")]
    public class UsersModel : PageModel
    {
		private readonly SignInManager<WebAuthUser> _signInManager;
		private readonly UserManager<WebAuthUser> _userManager;
		private readonly WebAuthT4Context _context;
		private readonly ILogger<UsersModel> _logger;

		public UsersModel(WebAuthT4Context context, SignInManager<WebAuthUser> signInManager, UserManager<WebAuthUser> userManager, ILogger<UsersModel> logger)
		{
			_context = context;
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
		}

		[BindProperty]
		public IList<UserViewModel> WebUser { get; set; } = default!;

        public async Task OnGetAsync()
        {
            WebUser = new List<UserViewModel>();
            if (_context.Users != null)
            {
                var users = await _context.Users.ToListAsync();
				foreach (var user in users)
				{
                    WebUser.Add(new UserViewModel
                    {
                        UserID = user.Id,
                        UserIndex = user.UserIndex,
						UserFullName = user.UserFullName,
						Email = user.Email,
                        LastLoginTime= user.LastLoginTime,
                        RegistrationTime= user.RegistrationTime,
                        UserStatus = user.UserStatus
                    });
				}
			}
			WebUser = WebUser.OrderBy(s => s.UserIndex).ToList();
		}

		public async Task<IActionResult> OnPostDeleteUsers()
		{
			var selectedUsers = GetSelectedUsers();
			foreach (var user in selectedUsers)
			{
				await DeleteUser(user);
				_logger.LogInformation("Deleted " + user.Email + " ID: " + user.Id);
			}
			return RedirectToPage("./Users");
		}

		private async Task<IActionResult> DeleteUser(WebAuthUser user)
		{
			if (_context.Users == null || user == null)
			{
				return RedirectToAction(nameof(Index));
			}
			var currentUser = await _userManager.GetUserAsync(User);
			var strategy = _context.Database.CreateExecutionStrategy();
			await strategy.ExecuteAsync(
				async () =>
				{
					await _userManager.UpdateSecurityStampAsync(user);
					await _userManager.DeleteAsync(user);
					await _context.SaveChangesAsync();
				});
			if (currentUser == user)
				await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> OnPostBlockUsers()
		{
			var selectedUsers = GetSelectedUsers();
			foreach (var user in selectedUsers)
			{
				await BlockUser(user);
				_logger.LogInformation("Blocked " + user.Email + " ID: " + user.Id);
			}
			return RedirectToPage("./Users");
		}

		private async Task<IActionResult> BlockUser(WebAuthUser user)
		{
			if (_context.Users == null || user == null)
			{
				return RedirectToAction(nameof(Index));
			}
			user.UserStatus = "Blocked";
			var currentRoles = await _userManager.GetRolesAsync(user);
			if(currentRoles.Count == 0)
				return RedirectToAction(nameof(Index));
			await _userManager.RemoveFromRoleAsync(user, "ActiveUser");
			user.LockoutEnd = DateTime.MaxValue;
			await _userManager.UpdateSecurityStampAsync(user);
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == user)
				await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> OnPostUnblockUsers()
		{
			var selectedUsers = GetSelectedUsers();
			foreach (var user in selectedUsers)
			{
				await UnblockUser(user);
				_logger.LogInformation("Unblocked " + user.Email + " ID: " + user.Id);
			}
			return RedirectToPage("./Users");
		}

		private async Task<IActionResult> UnblockUser(WebAuthUser user)
		{
			if (_context.Users == null || user == null)
			{
				return RedirectToAction(nameof(Index));
			}
			user.UserStatus = "Active";
			var currentRoles = await _userManager.GetRolesAsync(user);
			if(currentRoles.Contains("ActiveUser"))
				return RedirectToAction(nameof(Index));
			await _userManager.AddToRoleAsync(user, "ActiveUser");
			user.LockoutEnd = DateTime.Now;

			return RedirectToAction(nameof(Index));
		}

		private List<WebAuthUser> GetSelectedUsers()
		{
			var selectedUsers = new List<WebAuthUser>();
			foreach (var user in WebUser)
			{
				if (user.Selection.Selected)
				{
					var selectedUser = _context.Users.Find(user.UserID);
					if (selectedUser != null)
						selectedUsers.Add(selectedUser);
				}
			}
			return selectedUsers;
		}
	}
}
