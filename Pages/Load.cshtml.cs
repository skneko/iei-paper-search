using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IEIPaperSearch.Services.DataLoaders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IEIPaperSearch.Pages
{
    public class LoadModel : PageModel
    {
        readonly IDataLoaderService loaderService;

        public LoadModel(IDataLoaderService loaderService)
        {
            this.loaderService = loaderService;
        }
    
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public bool LoadFromDblp { get; set; }
        [BindProperty]
        public bool LoadFromIeeeXplore { get; set; }
        [BindProperty]
        public bool LoadFromGoogleScholar { get; set; }

        [BindProperty]
        [Range(1000,3000)]
        public uint StartingYear { get; set; }
        [BindProperty]
        [Range(1000,3000)]
        [GreaterThanOrEqualTo("StartingYear", ErrorMessage = "El a�o de final debe ser mayor o igual que el a�o de inicio.")]
        public uint EndYear { get; set; }

        public async Task<IActionResult> OnPostSubmit()
        {
            Console.WriteLine("Form sent");

            if (!LoadFromDblp && !LoadFromIeeeXplore && !LoadFromGoogleScholar)
            {
                ModelState.AddModelError("", "Selecciona al menos una fuente.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            return Page();

            loaderService.LoadFromIeeeXplore();

            TempData["test"] = 3;

            return RedirectToPage("/LoadResults");
        }
    }
}