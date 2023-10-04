using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Entities;

var builder = WebApplication.CreateBuilder(args);

//Identity i�in olu�turulan Context'in database ile ba�lant�s� burada yap�l�yor.
builder.Services.AddDbContext<ListContext>(options => options.UseMySql("Server=localhost;Database=ShoppingListDB;Uid=root;Pwd=Halil123*;", new MySqlServerVersion(new Version(7, 0, 0))));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ListContext>().AddDefaultTokenProviders();

//Identity ayarlar�
builder.Services.Configure<IdentityOptions>(options =>
{
    //�ifrede say�sal de�er olmak zorunda
    options.Password.RequireDigit = true;
    //K���k-b�y�k harf olmak zorunda
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    //Minimum uzunluk
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "";
});

//Cookie kullan�c� taray�c�s�nda uyguluma taraf�ndan b�rak�lan bilgidir.
builder.Services.ConfigureApplicationCookie(options =>
{
    //E�er ki ba�lant� koptuysa, uygulama kullan�c�y� tan�m�yorsa login sayfas�na y�nlendirir.
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    //Kullan�c� yetkili olmad��� alana girmeye �al���nca
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
