﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using IEIPaperSearch.Models;
using IEIPaperSearch.Persistence;

namespace IEIPaperSearch.Pages.Journals
{
    public class CreateModel : PageModel
    {
        private readonly IEIPaperSearch.Persistence.PaperSearchContext _context;

        public CreateModel(IEIPaperSearch.Persistence.PaperSearchContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Journal Journal { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Journals.Add(Journal);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
