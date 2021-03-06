﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using SiteMVC.Models;

namespace SiteMVC.Controllers {
    public class AccountController : Controller {
        private readonly SignInManager<WebsiteUser> _signInManager;
        private readonly UserManager<WebsiteUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<WebsiteUser> signInManager, UserManager<WebsiteUser> userManager, RoleManager<IdentityRole> roleManager) {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public ActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel, string redirectUrl) {
            if (ModelState.IsValid) {
                WebsiteUser user = new WebsiteUser {
                    UserName = registerModel.Username,
                    Email = registerModel.Email
                };
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded) {
                    bool roleExists = await _roleManager.RoleExistsAsync(registerModel.RoleName);
                    if (!roleExists) {
                        await _roleManager.CreateAsync(new IdentityRole(registerModel.RoleName));
                    }
                    var addedUser = await _userManager.FindByNameAsync(registerModel.Username);
                    await _userManager.AddToRoleAsync(addedUser, registerModel.RoleName);

                    return await Login(registerModel, redirectUrl);
                }
            }
            ViewBag.RedirectUrl = redirectUrl;
            return View();
        }

        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel, string redirectUrl) {
            // Que fait cette condition ? Pris dans le skillpipe
            if (ModelState.IsValid) {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, true, false);
                if (result.Succeeded) {
                    if (!redirectUrl.IsNullOrEmpty()) {
                        return Redirect(redirectUrl);
                    }
                    else {
                        return RedirectToAction("Index", "Tirelires");
                    }
                }
            }
            ViewBag.RedirectUrl = redirectUrl;
            return View();
        }
        [Authorize]
        public ActionResult Logout(string redirectUrl) {
            if (User.Identity.IsAuthenticated) {
                _signInManager.SignOutAsync();
            }
            if (!redirectUrl.IsNullOrEmpty()) {
                return Redirect(redirectUrl);
            }
            else {
                return RedirectToAction("Index", "Tirelires");
            }
        }
    }
}
