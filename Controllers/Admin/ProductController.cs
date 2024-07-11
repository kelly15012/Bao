using Bao.Data;
using Bao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bao.Controllers.Admin
{
    public class ProductController : Controller
    {
        private readonly BaoContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(BaoContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Product
        public async Task<IActionResult> ProdIndex()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // GET: Product/Details
        public async Task<IActionResult> ProdDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult ProdCreate()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "categoryId", "categoryName");
            return View();
        }

        // POST: Product/Create
        //public async Task<IActionResult> ProdCreate([Bind("ProductId,ProductName,ProductDescription,Price,Quantity,CoverImage,CategoryId,Status,CreateAt,ModifiedAt")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        product.CreateAt = DateTime.Now;
        //        product.ModifiedAt = DateTime.Now;
        //        _context.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(ProdIndex));
        //    }
        //    ViewData["categoryId"] = new SelectList(_context.Categories, "categoryId", "categoryName", product.CategoryId);
        //    return View(product);
        //}

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdCreate([Bind("ProductId,ProductName,ProductDescription,Price,Quantity,CategoryId,Status")] Product product)
        {
            _logger.LogInformation("Received product: {@Product}", product);
            //if (coverImageFile != null)
            //{
            //    _logger.LogInformation("Received cover image file: {FileName}, Length: {Length}", coverImageFile.FileName, coverImageFile.Length);
            //}

            if (ModelState.IsValid)
            {
                _logger.LogInformation("ModelState is valid.");

                var file = Request.Form.Files.GetFile("productImage");
                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        product.CoverImage = memoryStream.ToArray();
                        product.FileName = file.FileName;
                        product.ContentType = file.ContentType;
                    }
                }
                else
                {
                    return BadRequest("Proof of delivery is required for 'Delivered' status.");
                }

                //if (coverImageFile != null && coverImageFile.Length > 0)
                //{
                //    using (var memoryStream = new MemoryStream())
                //    {
                //        await coverImageFile.CopyToAsync(memoryStream);
                //        product.CoverImage = memoryStream.ToArray();
                //    }
                //}

                product.CreateAt = DateTime.Now;
                product.ModifiedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProdIndex));
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError("ModelState Error: {ErrorMessage}", error.ErrorMessage);
                    }
                }
            }
            PopulateCategoriesDropDownList(product.CategoryId);
            return View(product);
        }

        private void PopulateCategoriesDropDownList(object? selectedCategory = null)
        {
            var categoriesQuery = from c in _context.Categories
                                  orderby c.categoryName
                                  select c;
            ViewBag.CategoryId = new SelectList(categoriesQuery.AsNoTracking(), "categoryId", "categoryName", selectedCategory);
        }

    }
}
