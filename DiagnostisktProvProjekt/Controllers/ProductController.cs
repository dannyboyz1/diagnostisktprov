﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiagnostisktProvProjekt.Data;
using DiagnostisktProvProjekt.Models;
using DiagnostisktProvProjekt.Models.ProductViewModels;
using Microsoft.Extensions.Logging;
using DiagnostisktProvProjekt.Services;

namespace DiagnostisktProvProjekt.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly ProductCategoryService _productCategoryService;

        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger, ProductCategoryService productCategoryService)
        {
            _context = context;
            _logger = logger;
            _productCategoryService = productCategoryService;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            _logger.LogWarning("With great powers comes great responsibilities");

            var products = await _context.Products.Include(p => p.ProductCategory).ToListAsync();

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .SingleOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            var viewModel = new CreateEditProductViewModel()
            {
                Product = new Product(),
                AllProductCategories = _productCategoryService.GetProductCategories().ToList()
            };

            return View(viewModel);
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEditProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var productCategory = _context.ProductCategories.Single(p => p.ProductCategoryId == viewModel.SelectedProductCategoryId);

                viewModel.Product.ProductCategory = productCategory;

                _context.Add(viewModel.Product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = new CreateEditProductViewModel()
            {
                Product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id),
                AllProductCategories = _productCategoryService.GetProductCategories().ToList()
            };
            
            if (viewModel.Product == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateEditProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var productCategory = _context.ProductCategories.Single(p => p.ProductCategoryId == viewModel.SelectedProductCategoryId);
                    viewModel.Product.ProductCategory = productCategory;

                    _context.Update(viewModel.Product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(viewModel.Product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .SingleOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
