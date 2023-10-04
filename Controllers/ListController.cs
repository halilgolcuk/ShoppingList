using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Entities;
using ShoppingList.Models;
using System.Security.Claims;

namespace ShoppingList.Controllers
{
    [Authorize(Roles = "User")]

    public class ListController : Controller
	{
		private readonly ListContext _context;
        public ListController(ListContext context)
        {
            _context = context;
        }
        public IActionResult GetLists(string id)
		{
			List<List> lists = _context.Lists
				.Where(e => e.UserId == id)
				.Select(e => new List() { Id = e.Id, Name = e.Name, CreatedDate = e.CreatedDate, ProductLists = null, IsActive = e.IsActive})
				.ToList();
			return View(lists);
		}
		public IActionResult ListEdit(int id)
		{	
		List<ProductList> productListsWithDescription = new List<ProductList>();
		ListProductViewModel model = new ListProductViewModel();
			var list = _context.Lists.FirstOrDefault(e => e.Id == id);
			if (list.IsActive == false)
			{
                productListsWithDescription = _context.ProductLists.Where(e => e.ListId == id).ToList();
				var productIds = _context.ProductLists.Where(e => e.ListId == id).Select(e => e.ProductId).ToList();
				var products = _context.Products.Where(p => productIds.Contains(p.Id)).ToList();
				model.ListProducts = products;
				model.ListName = list.Name;
				model.ListId = list.Id;
				model.productListsWithDesc = productListsWithDescription;
				return View(model);

			} else
			{
				return Redirect("/list/getlists");
			}
		}
		[HttpGet]
		public IActionResult Shopping(int id )
		{
			ListProductViewModel model = new ListProductViewModel();
			var productIds = _context.ProductLists.Where(e => e.ListId == id).Select(e => e.ProductId).ToList();
			var descriptions = _context.ProductLists.Where(e => e.ListId == id).Select(e => e.ProductDescription).ToList();
			var list = _context.Lists.FirstOrDefault(e => e.Id == id);
			list.IsActive = true;
			var products = _context.Products.Where(p => productIds.Contains(p.Id)).ToList();
			model.ListProducts = products;
			model.ListName = list.Name;
			model.ListId = list.Id;
			_context.SaveChanges();
			return View(model);
		}
		[HttpPost]
		public IActionResult ShoppingEnd(int list_id)
		{
            var form = HttpContext.Request.Form;
            var selectedProducts = form.Keys
                .Where(key => key.StartsWith("product_"))
                .Select(key => form[key])
                .ToList();
            int[] selectedProductIds = selectedProducts.Select(value => int.Parse(value)).ToArray();
            List<ProductList> items = _context.ProductLists
                .Where(e => e.ListId == list_id && selectedProductIds.Contains(e.ProductId)).ToList();
			List list = _context.Lists.FirstOrDefault(e => e.Id == list_id);
			list.IsActive = false;
            _context.ProductLists.RemoveRange(items);
            _context.SaveChanges();
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var returnUrl = "/list/getlists/" + userId;
			return Redirect(returnUrl);
		}
		[HttpGet]
		public IActionResult AddList()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			return View();
		}
		[HttpPost]
		public IActionResult AddList(string name) 
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var returnUrl = "/list/getlists/" + userId;

            if (!string.IsNullOrEmpty(name))
			{
                List list = new List() { Name = name, CreatedDate = DateTime.Now, IsActive = false, UserId = userId };
                _context.Lists.Add(list);
                _context.SaveChanges();
			}
			return Redirect(returnUrl);

        }
		[HttpGet]
		public IActionResult RemoveList(int id)
		{
			ListViewModel list = _context.Lists
				.Where(e => e.Id == id)
                .Select(e => new ListViewModel() { Id = e.Id, Name = e.Name, CreatedDate = e.CreatedDate }).FirstOrDefault();
			return View(list);
		}
		[HttpPost]
		public IActionResult RemoveListPost(int list_id)
		{
			List list = _context.Lists.Where(e => e.Id ==  list_id).FirstOrDefault();
			List<ProductList> listProducts = _context.ProductLists.Where(e => e.ListId == list_id).ToList();
			_context.Lists.Remove(list);
			_context.ProductLists.RemoveRange(listProducts);
			_context.SaveChanges();
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var returnUrl = "/list/getlists/" + userId;
			return Redirect(returnUrl);
		}
	}
}
