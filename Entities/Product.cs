namespace ShoppingList.Entities
{
	public class Product
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string image { get; set; }
        public Category Category { get; set; }
		public List<ProductList> ProductLists { get; set; }

	}
}
