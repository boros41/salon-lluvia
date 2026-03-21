using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Mvc.Models;
using Mvc.Models.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Mvc.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;

    public AccountController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult LogIn(string returnUrl = "")
    {
        LoginViewModel model = new() { ReturnUrl = returnUrl };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LogIn(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid username/password.");
            return View(model);
        }

        if (!model.ReturnUrl.IsNullOrEmpty() && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public ViewResult AccessDenied()
    {
        return View();
    }
}
