using CoffeeCrazy.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCrazy.Pages.Login.Password
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IPasswordRepo _passwordRepo;
        private readonly IAccessService _accessService;

        [BindProperty]
        public string CurrentPassword { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string RepeatNewPassword { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public ChangePasswordModel(IPasswordRepo passwordRepo, IAccessService accessService)
        {
            _passwordRepo = passwordRepo;
            _accessService = accessService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!_accessService.IsUserLoggedIn(HttpContext))
                return RedirectToPage("/Login/Login");

            string email = _accessService.GetLoggedUserEmail(HttpContext);

            if (NewPassword == RepeatNewPassword)
            {
                try
                {
                    await _passwordRepo.ChangePasswordAsync(email, CurrentPassword, NewPassword);
                    SuccessMessage = "Nyt password er lavet.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ErrorMessage = "An error occurred while changing the password. SQL related";
                }
            }
            return RedirectToPage("/Machines/Index");

        }
    }
}
