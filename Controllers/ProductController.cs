using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Entities;
using ShoppingList.Models;
using System.Security.Claims;

namespace ShoppingList.Controllers
{
	[Authorize(Roles = "User")]
	public class ProductController : Controller
	{
		private readonly ListContext _context;
		public ProductController(ListContext context)
		{
			_context = context;
		}
		public IActionResult ProductList(int id, string category, string product)
		{
			List<Product> products;
			var addedProducts = _context.ProductLists.Where(p => p.ListId == id).Select(p => p.ProductId).ToList();
			List list = _context.Lists.Where(e => e.Id == id).FirstOrDefault();
			List<Category> categories = _context.Categories.ToList();
			if (!string.IsNullOrEmpty(product))
			{
				products = _context.Products.Where(p => !addedProducts.Contains(p.Id) && p.Name == product).ToList();
			} 
			else if (!string.IsNullOrEmpty(category))
			{
				products = _context.Products.Where(p => !addedProducts.Contains(p.Id) && p.Category.Name == category).ToList();

			} 
			else
			{
				products = _context.Products.Where(p => !addedProducts.Contains(p.Id)).ToList();
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			ViewBag.Products = products;
			ViewBag.Categories = categories;
			ViewBag.List = list;
			ViewBag.UserId = userId;
			return View();
		}
		[HttpPost]
		public IActionResult AddProduct(int list_id, int product_id, string description)
		{
			ProductList product = new ProductList() { ProductId = product_id, ListId = list_id, ProductDescription = description };
			_context.ProductLists.Add(product);
			_context.SaveChanges();
			string redirectUrl = Url.Action("ProductList", "Product", new { id = list_id });
			return Redirect(redirectUrl);
		}
		[HttpPost]
		public IActionResult RemoveProduct(int list_id)
		{
            var form = HttpContext.Request.Form;
            var selectedProducts = form.Keys
                .Where(key => key.StartsWith("product_"))
                .Select(key => form[key])
                .ToList();
            int[] selectedProductIds = selectedProducts.Select(value => int.Parse(value)).ToArray();
			List<ProductList> items = _context.ProductLists
				.Where(e => e.ListId == list_id && selectedProductIds.Contains(e.ProductId)).ToList();
			_context.ProductLists.RemoveRange(items);
			_context.SaveChanges();
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var returnUrl = "/list/getlists/" + userId;
			return Redirect(returnUrl);
		}
		[HttpPost]
		public IActionResult SearchProduct()
		{
			var form = HttpContext.Request.Form;
			var list_id = form.Keys.Where(key => key.StartsWith("list")).Select(key => form[key]).FirstOrDefault();
			var product_name = form.Keys.Where(key => key.StartsWith("product")).Select(key => form[key]).FirstOrDefault();
			var returnUrl = "/product/productlist/" + list_id + "?product=" + product_name;
			return Redirect(returnUrl);
		}
	}
}
