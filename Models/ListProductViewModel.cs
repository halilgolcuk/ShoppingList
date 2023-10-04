using ShoppingList.Entities;

namespace ShoppingList.Models
{
	public class ListProductViewModel
	{ 
		public int ListId { get; set; }
		public string ListName { get; set; }
		public List<Product> ListProducts { get; set; }
		public List<ProductList> productListsWithDesc { get; set; }
	}
}
