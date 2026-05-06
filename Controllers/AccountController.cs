using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Models;
using SugboGo.Services.Auth;

namespace SugboGo.Controllers;

public sealed class AccountController : Controller
{
    private readonly IUserAccountStore _userStore;
    private readonly IPasswordService _passwordService;

    public AccountController(IUserAccountStore userStore, IPasswordService passwordService)
    {
        _userStore = userStore;
        _passwordService = passwordService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Sign in or create an account";
        return View(new EmailEntryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckEmail(EmailEntryViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Sign in or create an account";

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var email = NormalizeEmail(model.Email);
        var existingUser = await _userStore.FindByEmailAsync(email, cancellationToken);

        return existingUser is null
            ? RedirectToAction(nameof(Register), new { email })
            : RedirectToAction(nameof(SignIn), new { email });
    }

    [HttpGet]
    public IActionResult SignIn(string email)
    {
        ViewData["Title"] = "Enter your password";
        return View(new SignInViewModel { Email = NormalizeEmail(email) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(SignInViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Enter your password";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userStore.FindByEmailAsync(model.Email, cancellationToken);

        if (user is null || !_passwordService.VerifyPassword(model.Password, user.PasswordHash))
        {
            model.ErrorMessage = "The password does not match this SogboGo account.";
            return View(model);
        }

        await SignUserInAsync(user);
        return RedirectToAction("Index", "Booking");
    }

    [HttpGet]
    public IActionResult Register(string email)
    {
        ViewData["Title"] = "Create your account";
        return View(new RegisterViewModel { Email = NormalizeEmail(email) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Create your account";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = NormalizeEmail(model.Email);
        var existingUser = await _userStore.FindByEmailAsync(email, cancellationToken);

        if (existingUser is not null)
        {
            return RedirectToAction(nameof(SignIn), new { email });
        }

        var user = new UserAccount
        {
            Email = email,
            FullName = model.FullName.Trim(),
            PasswordHash = _passwordService.HashPassword(model.Password)
        };

        try
        {
            await _userStore.CreateAsync(user, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            model.ErrorMessage = ex.Message;
            return View(model);
        }

        await SignUserInAsync(user);
        return RedirectToAction("Index", "Booking");
    }

    [HttpGet]
    public IActionResult Recover()
    {
        ViewData["Title"] = "Recover your account";
        return View(new RecoverAccountViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Recover(RecoverAccountViewModel model)
    {
        ViewData["Title"] = "Recover your account";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Submitted = true;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    private async Task SignUserInAsync(UserAccount user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });
    }

    private static string NormalizeEmail(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}
