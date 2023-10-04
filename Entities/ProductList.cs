using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Entities
{
	public class ProductList
	{
        public int Id { get; set; }
        public int ProductId { get; set; }
		public Product Product { get; set;}
		public int ListId { get; set; }
		public List List {  get; set; }
		public string? ProductDescription { get; set; }

	}
}
