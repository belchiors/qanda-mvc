using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Qanda.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

if (builder.Environment.IsDevelopment())
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options => {
        options.UseLazyLoadingProxies();
        options.UseNpgsql(connectionString);
    });
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => {
        var databaseUri = new Uri(Environment.GetEnvironmentVariable("DATABASE_URL")!);
        string database = databaseUri.LocalPath.TrimStart('/');
        string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
        string connectionString = $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={database};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";            
        options.UseLazyLoadingProxies();
        options.UseNpgsql(connectionString);
    });
}

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        options.Cookie.Name = "sessionid";
        options.LoginPath = "/Account/SignIn";
        options.LogoutPath = "/Account/SignOut";
    });

builder.Services.AddRouting(options => {
    options.LowercaseUrls = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "profile",
    pattern: "profile/{username}",
    defaults: new { controller = "Account", action = "Profile" }
);

app.MapControllerRoute(
    name: "search",
    pattern: "search",
    defaults: new { controller = "Question", action = "Search" }
);

app.MapControllerRoute(
    name: "questions",
    pattern: "question/{questionId}/{questionUrl}",
    defaults: new { controller = "Question", action = "ViewQuestion" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();