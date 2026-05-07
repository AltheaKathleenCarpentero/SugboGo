// program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using SugboGo.Services.Admin;
using SugboGo.Services.Auth;
using SugboGo.Services.Dashboard;
using SugboGo.Services.Travel;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys")))
    .SetApplicationName("SogboGo");
builder.Services.Configure<SupabaseOptions>(builder.Configuration.GetSection("Supabase"));
builder.Services.Configure<AccountRoleOptions>(builder.Configuration.GetSection("Authentication"));
builder.Services.AddSingleton<IPasswordService, Pbkdf2PasswordService>();
builder.Services.AddSingleton<IAccountRoleService, AccountRoleService>();
builder.Services.AddScoped<LocalJsonUserAccountStore>();
builder.Services.AddScoped<PostgresUserAccountStore>();
builder.Services.AddHttpClient<SupabaseUserAccountStore>();
builder.Services.AddScoped<UserAccountStoreFactory>();
builder.Services.AddScoped<IUserAccountStore>(provider => provider.GetRequiredService<UserAccountStoreFactory>().Create());
builder.Services.AddScoped<LocalJsonTravelPreferenceStore>();
builder.Services.AddScoped<PostgresTravelPreferenceStore>();
builder.Services.AddHttpClient<SupabaseTravelPreferenceStore>();
builder.Services.AddScoped<TravelPreferenceStoreFactory>();
builder.Services.AddScoped<ITravelPreferenceStore>(provider => provider.GetRequiredService<TravelPreferenceStoreFactory>().Create());
builder.Services.AddSingleton<ICebuRecommendationService, CebuRecommendationService>();
builder.Services.AddScoped<IAdminOperationsService, AdminOperationsService>();
builder.Services.AddScoped<IDashboardExperienceService, DashboardExperienceService>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SogboGo.Auth";
        options.LoginPath = "/Account";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Default route points to the SogboGo landing page.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
