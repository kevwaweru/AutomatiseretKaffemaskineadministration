using CoffeeCrazy.Interfaces;
using CoffeeCrazy.Models;
using CoffeeCrazy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeCrazy.Pages.Machines
{
    public class EditModel : PageModel
    {
        private readonly ICRUDRepo<Machine> _machineRepo;
        private readonly IImageService _imageService;
        private readonly IAccessService _accessService;

        public EditModel(ICRUDRepo<Machine> machineRepo, IImageService imageService, IAccessService accessService)
        {
            _machineRepo = machineRepo;
            _imageService = imageService;
            _accessService = accessService;
        }

        [BindProperty]
        public Machine MachinetoUpdate { get; set; }
        public IFormFile PictureToUpload { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!_accessService.IsUserLoggedIn(HttpContext))
            {
                return RedirectToPage("/Login/Login");
            }
            if (!_accessService.IsAdmin(HttpContext))
            {
                return RedirectToPage("/Errors/AccessDenied");
            }


            // Henter maskinen baseret p� ID
            MachinetoUpdate = await _machineRepo.GetByIdAsync(id);
            if (MachinetoUpdate == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            MachinetoUpdate.MachineImage = _imageService.FormFileToByteArray(PictureToUpload);
            if (!ModelState.IsValid)
            {
                return Page();
            }
           
            MachinetoUpdate.MachineId = id;
            // Opdaterer maskinen i databasen
            await _machineRepo.UpdateAsync(MachinetoUpdate);

            // Redirecter til Index (oversigten)
            return RedirectToPage("./Index");
        }
    }
}
