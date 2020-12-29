﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IEIPaperSearch.Models;
using IEIPaperSearch.Persistence;

namespace IEIPaperSearch.Pages.Journals
{
    public class DetailsModel : PageModel
    {
        private readonly IEIPaperSearch.Persistence.PaperSearchContext _context;

        public DetailsModel(IEIPaperSearch.Persistence.PaperSearchContext context)
        {
            _context = context;
        }

        public Journal Journal { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Journal = await _context.Journals.FirstOrDefaultAsync(m => m.Id == id);

            if (Journal == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
