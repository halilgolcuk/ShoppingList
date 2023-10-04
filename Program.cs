using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Entities;

var builder = WebApplication.CreateBuilder(args);

//Identity için oluþturulan Context'in database ile baðlantýsý burada yapýlýyor.
builder.Services.AddDbContext<ListContext>(options => options.UseMySql("Server=localhost;Database=ShoppingListDB;Uid=root;Pwd=Halil123*;", new MySqlServerVersion(new Version(7, 0, 0))));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ListContext>().AddDefaultTokenProviders();

//Identity ayarlarý
builder.Services.Configure<IdentityOptions>(options =>
{
    //þifrede sayýsal deðer olmak zorunda
    options.Password.RequireDigit = true;
    //Küçük-büyük harf olmak zorunda
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    //Minimum uzunluk
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "";
});

//Cookie kullanýcý tarayýcýsýnda uyguluma tarafýndan býrakýlan bilgidir.
builder.Services.ConfigureApplicationCookie(options =>
{
    //Eðer ki baðlantý koptuysa, uygulama kullanýcýyý tanýmýyorsa login sayfasýna yönlendirir.
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    //Kullanýcý yetkili olmadýðý alana girmeye çalýþýnca
    options.AccessDeniedPath = "/auth/accessdenied";

    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name = ".ShopApp.Security.Cookie"
    };
});

// Add services to the container.
builder.Services.AddControllersWithViews();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
