using CoffeeCrazy.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCrazy.Pages.Users
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IUserRepo _userRepo;

        public ChangePasswordModel(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [BindProperty]
        public string CurrentPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = HttpContext.Session.GetString("Email");

            // Check if the user is logged in
            if (email == null)
            {
                ErrorMessage = "Du skal v�re logget ind. du bliver rediregeret til Login Page";
                //M�ske implem noget tid p� 2+ sek s� man kan n� at l�se fejlbesked.
                return RedirectToPage("/Login/Login"); 
            }

            try
            {
                await _userRepo.ChangePasswordAsync(email, CurrentPassword, NewPassword);
                SuccessMessage = "Nyt password er lavet.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ErrorMessage = "An error occurred while changing the password. SQL related";
            }

            return Page();
        }
    }
}
