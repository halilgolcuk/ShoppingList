using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ShoppingList.Entities
{
    public class ListContext : IdentityDbContext<User>
	{
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<List> Lists { get; set; }
		public DbSet<ProductList> ProductLists { get; set; }

		public ListContext(DbContextOptions<ListContext> options)
		: base(options)
		{
		}

		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	//base.OnConfiguring(optionsBuilder);
		//	optionsBuilder.UseMySql("Server=localhost;Database=ShoppingListDB;Uid=root;Pwd=Halil123*;", new MySqlServerVersion(new Version(7, 0, 0)));
		//}

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	modelBuilder.Entity<ProductList>()
		//				.HasKey(t => new { t.ProductId, t.ListId });

		//	modelBuilder.Entity<ProductList>()
		//				.HasOne(pl => pl.Product)
		//				.WithMany(p => p.ProductLists)
		//				.HasForeignKey(pl => pl.ProductId);

		//	modelBuilder.Entity<ProductList>()
		//				.HasOne(pl => pl.List)
		//				.WithMany(l => l.ProductLists)
		//				.HasForeignKey(pl => pl.ListId);

		//	//modelBuilder.Entity<List>()
		//	//			.HasOne(p => p.User)
		//	//			.WithMany(c => c.Lists)
		//	//			.HasForeignKey(p => p.UserId)
		//	//			.OnDelete(DeleteBehavior.NoAction);
		//}
	}
}
