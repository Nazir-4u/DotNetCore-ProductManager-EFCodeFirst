using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Management.Data;
using Product_Management.Models;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(p => p.Images).ToListAsync();
        return View(products);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Product product, List<IFormFile> images)
    {
        if (ModelState.IsValid)
        {
            product.CreatedAt = DateTime.Now; 
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (images != null && images.Count > 0)
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);

                foreach (var file in images)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string fullPath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = "/uploads/" + fileName
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product, List<IFormFile> images)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            if (images != null && images.Count > 0)
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);

                foreach (var file in images)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string fullPath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = "/uploads/" + fileName
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        foreach (var img in product.Images)
        {
            string path = Path.Combine(_env.WebRootPath, img.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await _context.ProductImages.FindAsync(id);
        if (image == null) return NotFound();

        string path = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();
        return RedirectToAction("Edit", new { id = image.ProductId });
    }
}