using Microsoft.AspNetCore.Mvc;
using Bao.Data;
using Bao.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Bao.Controllers.Admin
{
    public class CategoryController : Controller
    {
        private readonly BaoContext _context;

        public CategoryController(BaoContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("categoryId,categoryName,Status")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreateAt = DateTime.Now;
                category.ModifiedAt = DateTime.Now;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit
        public async Task<IActionResult> Edit(int? categoryId)
        {
            if (categoryId == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return BadRequest(categoryId + " is not found in the table!");
            }

            return View(category);
        }

        // POST: Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.categoryId == category.categoryId);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    // Keep the created date
                    category.CreateAt = existingCategory.CreateAt;

                    // Update the modified date
                    category.ModifiedAt = DateTime.Now;
                    _context.Categories.Update(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Category");
                }
                return View("Edit", category);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

        }

        // GET: Category/Delete
        public async Task<IActionResult> Delete(int? categoryId)
        {
            if (categoryId == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return BadRequest(categoryId + " is not found in the table!");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Category");
        }


    }
}
