using Bao.Data;
using Bao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdCreate([Bind("ProductId,ProductName,ProductDescription,Price,Quantity,CategoryId,Status,CreateAt,ModifiedAt")] Product product)
        {
            _logger.LogInformation("Received product: {@Product}", product);


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
                    return BadRequest("A product image is required.");
                }

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
            ViewBag.CategoryId = new SelectList(_context.Categories, "categoryId", "categoryName", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit
        public async Task<IActionResult> ProdEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "categoryId", "categoryName", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdEdit(int id, [Bind("ProductId,ProductName,ProductDescription,Price,Quantity,CategoryId,Status,CreateAt,ModifiedAt")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            _logger.LogInformation("Received product for edit: {@Product}", product);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    var file = Request.Form.Files.GetFile("CoverImageFile");
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
                    if (file == null || file.Length == 0)
                    {
                        // Retain the old image if no new image is uploaded
                        product.CoverImage = existingProduct.CoverImage;
                        product.FileName = existingProduct.FileName;
                        product.ContentType = existingProduct.ContentType;
                    }
                    else
                    {
                        // Delete the old image if a new image is uploaded
                        if (existingProduct.CoverImage != null)
                        {
                            _context.Entry(existingProduct).Property(p => p.CoverImage).CurrentValue = null;
                            _context.Entry(existingProduct).Property(p => p.FileName).CurrentValue = null;
                            _context.Entry(existingProduct).Property(p => p.ContentType).CurrentValue = null;
                        }
                    }

                    product.ModifiedAt = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Product updated successfully: {@Product}", product);

                    return RedirectToAction(nameof(ProdIndex));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency exception occurred while updating product: {@Product}", product);
                    return StatusCode(500, "An error occurred while updating the product: " + ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred while updating product: {@Product}", product);
                    return StatusCode(500, "An error occurred while updating the product: " + ex.Message);
                }
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

            ViewBag.CategoryId = new SelectList(_context.Categories, "categoryId", "categoryName", product.CategoryId);
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }


        // GET: Product/Delete
        public async Task<IActionResult> ProdDelete(int? id)
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

        // POST: Product/Delete
        [HttpPost, ActionName("ProdDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdDeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ProdIndex));
        }
    }
}
