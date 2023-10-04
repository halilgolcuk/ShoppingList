using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Entities;
using ShoppingList.Models;

namespace ShoppingList.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
	{
		private readonly ListContext _context;
		public AdminController(ListContext context)
		{
			_context = context;
		}
		public IActionResult Desk()
		{
			int productCount = _context.Products.Count();
			int categoryCount = _context.Categories.Count();
			AdminViewModel viewModel = new AdminViewModel()
			{
				ProductCount = productCount,
				CategoryCount = categoryCount
			};
			return View(viewModel);
		}
		public IActionResult Products()
		{

			List<AdminProductViewModel> products = _context.Products
				.Select(p => new AdminProductViewModel() { ProductId = p.Id ,ProductName = p.Name, ProductImage = p.image ,CategoryName = p.Category.Name, CategoryId = p.Category.Id })
				.ToList();
			return View(products);
		}
		[HttpGet]
		public IActionResult AddProduct() 
		{
			List<Category> categories = _context.Categories.ToList();
			return View(categories);
		}
		[HttpPost]
		public async Task<IActionResult> AddProduct(string product_name, int category_id, IFormFile file)
		{
			if (string.IsNullOrEmpty(product_name) || category_id < 1 )
			{
				return RedirectToAction("desk", "admin");
			}
			var randomName = "";
			var path = "";
			if (file != null)
			{
				var extention = Path.GetExtension(file.FileName).ToLower();
				randomName = string.Format($"{Guid.NewGuid()}{extention}");
				path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            } else { randomName = "default.jpg"; }

			if (_context.Products.Any(p => p.Name == product_name))
			{
				TempData["Message"] = "Bu isimle başka bir ürün bulunmaktadır.";
				return View();
			};
			Category category = _context.Categories.Where(c => c.Id == category_id).FirstOrDefault();
			Product product = new Product() { Name = product_name, Category = category };
			product.image = randomName;
			_context.Products.Add(product);
			_context.SaveChanges();

			return RedirectToAction("products", "admin");
		}
		[HttpGet]
		public IActionResult EditProduct(int id)
		{
			AdminProductViewModel model = new AdminProductViewModel();
			List<Category> categories = _context.Categories.ToList();
			Product product = _context.Products.Where(c => c.Id == id).FirstOrDefault();

			model.ProductId = product.Id;
			model.ProductName = product.Name;
			model.ProductImage = product.image;
			model.CategoryId = product.Category.Id;
			model.CategoryName = product.Category.Name;

			ViewBag.Model = model;
			ViewBag.Categories = categories;
			return View();
		}
		[HttpPost]
		public IActionResult EditProduct(int id, string product_name, int category_id)
		{
			if (string.IsNullOrEmpty(product_name) || _context.Products.Any(p => p.Name == product_name))
			{
                return RedirectToAction("products", "admin");
            }
            Product product = _context.Products.Where(p => p.Id == id).FirstOrDefault();
			Category category = _context.Categories.Where(c => c.Id == category_id).FirstOrDefault();
			product.Name = product_name;
			product.Category = category;
			_context.SaveChanges();
			return RedirectToAction("products", "admin");
		}
		[HttpGet]
		public IActionResult RemoveProduct(int id) 
		{
			Product product = _context.Products.Where(p => p.Id == id).FirstOrDefault();
			return View(product);
		}
		[HttpPost]
		public IActionResult RemoveProductPost(int product_id)
		{
			Product product = _context.Products.Where(p => p.Id == product_id).FirstOrDefault();
			_context.Products.Remove(product);
			_context.SaveChanges();
			return RedirectToAction("products", "admin");
		}
		public IActionResult Categories() 
		{
			List<Category> categories = _context.Categories.ToList();
			return View(categories);
		}

		[HttpGet]
		public IActionResult AddCategories()
		{
			return View();
		}

		[HttpPost]
		public IActionResult AddCategories(string category_name) 
		{ 
			if (_context.Categories.Any(c => c.Name == category_name))
			{
				TempData["Message"] = "Bu isimle başka bir kategori bulunmaktadır.";
				return View();
			} else if(category_name == null)
			{
				return RedirectToAction("desk", "admin");
			}
			else
			{
				Category category = new Category() { Name = category_name };
				_context.Categories.Add(category);
				_context.SaveChanges();
			}

		return RedirectToAction("desk", "Admin");
		}
		[HttpGet]
		public IActionResult RemoveCategory(int id) 
		{
			Category category = _context.Categories.Where(category => category.Id == id).FirstOrDefault();
			return View(category);
		}
		[HttpPost]
		public IActionResult RemoveCategoryPost(int category_id)
		{
			Category category = _context.Categories.Where(c => c.Id == category_id).FirstOrDefault();
			if(category != null)
			{
				_context.Categories.Remove(category);
				_context.SaveChanges();
			}
			return RedirectToAction("categories","admin");
		}
		[HttpGet]
		public IActionResult EditCategory(int id)
		{
			Category category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
			return View(category);
		}
		[HttpPost]
		public IActionResult EditCategory(int id, string category_name)
		{
			if (string.IsNullOrEmpty(category_name))
			{
				return RedirectToAction("categories", "admin");
			}
			if (_context.Categories.Any(c => c.Name == category_name))
			{
				return RedirectToAction("categories", "admin");
            }
            Category category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
			if(category != null)
			{
				category.Name = category_name;
				_context.SaveChanges();
			}
			return RedirectToAction("categories","admin");
		}
	}
}
