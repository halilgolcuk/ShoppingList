using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Entities
{
    public class List
	{
		[Key]
        public int Id { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDate { get; set; }
		public List<ProductList> ProductLists { get; set; }
		public string UserId { get; set; }
		public User User { get; set; }
	}
}
