using Microsoft.AspNetCore.Identity;

namespace ShoppingList.Entities
{
    public class User : IdentityUser
    {
        //public int SecondaryId { get; set; }
        public string FullName { get; set; }
        public List<List> Lists { get; set; }
    }
}
