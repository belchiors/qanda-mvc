using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

using Qanda.Data;
using Qanda.Models;
using Qanda.ViewModels;

namespace Qanda.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public AccountController(ILogger<AccountController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult SignIn() => View();

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel model)
    {
        if (!ModelState.IsValid)
            return View();

        var user = _dbContext.Users?.SingleOrDefault(u => u.Email!.Equals(model.Email));

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError("", "The email or password is incorrect.");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()!),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var claimsIdendity = new ClaimsIdentity(claims, "Credentials");
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdendity),
            authProperties
        );

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult SignUp() => View();

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        if (!ModelState.IsValid)
            return View();

        if (!model.Password!.Equals(model.ConfirmPassword))
            ModelState.AddModelError("", "Password and repeated password did not match.");
        if (_dbContext.Users!.Any(u => u.Username!.Equals(model.Username)))
            ModelState.AddModelError("", "Sorry, but that username has already been taken.");
        if (_dbContext.Users!.Any(u => u.Email!.Equals(model.Email)))
            ModelState.AddModelError("", "This email address is already in use.");
        if (ModelState.ErrorCount > 0)
            return View();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var newUser = new User
        {
            Email = model.Email,
            Username = model.Username,
            PasswordHash = passwordHash
        };

        await _dbContext.AddAsync<User>(newUser);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("SignIn", "Account");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEmail(SettingsViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Settings");
        
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var currentUser = _dbContext.Users!.SingleOrDefault(u => u.Email!.Equals(userEmail));

        if (!BCrypt.Net.BCrypt.Verify(model.EmailSettingsViewModel!.Password, currentUser!.PasswordHash))
            return View("Settings");

        currentUser.Email = model.EmailSettingsViewModel.NewEmail;
        _dbContext.Users!.Update(currentUser);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Logout");
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePassword(SettingsViewModel model)
    {
        if (!ModelState.IsValid || model.PasswordSettingsViewModel!.NewPassword 
            != model.PasswordSettingsViewModel.ConfirmPassword)
            return RedirectToAction("Settings");

        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var currentUser = _dbContext.Users!.SingleOrDefault(u => u.Email!.Equals(userEmail));

        if (!BCrypt.Net.BCrypt.Verify(model.PasswordSettingsViewModel!.CurrentPassword, currentUser!.PasswordHash))
            return View("Settings");
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordSettingsViewModel.NewPassword);
        currentUser.PasswordHash = passwordHash;
        _dbContext.Users!.Update(currentUser);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Logout");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(string password)
    {
        if (!ModelState.IsValid)
            return View("Settings");

        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var currentUser = _dbContext.Users!.SingleOrDefault(u => u.Email!.Equals(userEmail));

        if (!BCrypt.Net.BCrypt.Verify(password, currentUser!.PasswordHash))
            return View("Settings");

        _dbContext.Users!.Remove(currentUser);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Logout");
    }

    public IActionResult Profile(string username) => View();

    public IActionResult Settings() => View();
}
