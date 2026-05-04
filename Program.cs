// program.cs (Updated to ensure default route points to Explore)
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Default route points to ExploreController (Index action)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Explore}/{action=Index}/{id?}");

app.Run();