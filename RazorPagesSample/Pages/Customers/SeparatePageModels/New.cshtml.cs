using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RazorPagesSample.Data;

namespace RazorPagesSample.Pages.Customers.SeparatePageModels
{
    public class NewModel : PageModel
    {
        private readonly AppDbContext _db;
        private IHostingEnvironment _environment;

        public NewModel(AppDbContext db, IHostingEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }
        
        [BindProperty]
        public Customer Customer { get; set; }

        [TempData]
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Customer.PhotePath = Customer.Phote.FileName;
            _db.Customers.Add(Customer);
            await UploadPhoto();
            await _db.SaveChangesAsync();

            Message = "New customer created successfully!";

            return RedirectToPage("./Index");
        }

        private async Task UploadPhoto()
        {
            var uploadsDirectoryPath = Path.Combine(_environment.WebRootPath, "Uploads");
            var uploadedfilePath = Path.Combine(uploadsDirectoryPath, Customer.Phote.FileName);

            using (var fileStream = new FileStream(uploadedfilePath, FileMode.Create))
            {
                await Customer.Phote.CopyToAsync(fileStream);
            }
        }
    }
}
