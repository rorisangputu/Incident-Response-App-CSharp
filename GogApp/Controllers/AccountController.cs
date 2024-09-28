using System;
using GogApp.Data;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GogApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
    ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public ActionResult Register()
    {
        var response = new RegisterViewModel();
        return View(response);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerVM)
    {
        if (!ModelState.IsValid) return View(registerVM);

        //Checking if the email already exists
        var userEmail = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
        if (userEmail != null)
        {
            TempData["Error"] = "This email address is already in use";
            return View(registerVM);
        }

        //Checking if the username already exists
        var userName = await _userManager.FindByNameAsync(registerVM.Name);
        if (userName != null)
        {
            TempData["Error"] = "This Username is already in use";
            return View(registerVM);
        }

        //If checks are good, a new App User is created
        var newUser = new AppUser()
        {
            Email = registerVM.EmailAddress,
            UserName = registerVM.Name
        };
        var newUserRes = await _userManager.CreateAsync(newUser, registerVM.Password);

        if (newUserRes.Succeeded)
        {
            return RedirectToAction("Index", "Home");

        }
        else
        {
            // Log the errors for debugging
            foreach (var error in newUserRes.Errors)
            {
                // Log the error or use a debug tool to view it
                Console.WriteLine($"Error: {error.Description}");
                TempData["Error"] = "Registration failed: " + error.Description;
            }

        }

        return View(registerVM);
    }


    [HttpGet]
    public ActionResult Login()
    {
        var response = new LoginViewModel();
        return View(response);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginVM)
    {
        if (!ModelState.IsValid) return View(loginVM);
        var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);

        if (user != null)
        {
            //User is found, check password
            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            if (passwordCheck)
            { //Password is correct, sign in
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            //Password is incorrect
            TempData["Error"] = "Wrong credentials. Please Try again";
            return View(loginVM);
        }
        //User not found
        TempData["Error"] = "Wrong credentials. Please Try again";
        return View(loginVM);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

