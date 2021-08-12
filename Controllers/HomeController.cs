using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //landing page
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {

            ///////
            return View();
        }

        public IActionResult Authen()
        {
            return View();
        }

        [HttpGet("denied")] //whenever authorization issue is created ,this page will be called
        public IActionResult Denied()
        {
            return View();
        }


        //only authenticated user and authorized user(whose role is admin) can enter 
        //this action
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Secured()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");
            return View();
        }


        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        //using async signin
        [HttpPost("login")]
        public async Task<IActionResult> Vaidate(string username,string password,string ReturnUrl)
        {
            if(username=="abc" && password == "123") //this can be replaced with datbase for lookup (of registerd users.)
            {
                /*
                 * so whats going on?
                 * there is list of claims created.
                 * these claim are populated with key:username and value:user's name,
                 * u can also add other things such as user birthday,email ,etc
                 * then with this claim we create user identity
                 * this user identity we create Principal-->it just like  a ticket that
                 * we discussed it stores inside cookie as encryption,it created once
                 * after signin it made inside browser ,and next time it will send with req
                 * server extract info and use it to authenticate and authorized user.
                 *
                 * */

                var claims = new List<Claim>();
                claims.Add( new Claim("username", username) );
                claims.Add( new Claim(ClaimTypes.NameIdentifier, username) );
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                return Redirect(ReturnUrl);
                //return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = ReturnUrl;
            ViewData["ErrorMsg"] = "Error. Invliad Email or Password....";
            return View("Login");
            

        }

        [Authorize] //to make sure this route is availbale to authenticated user.
        public  async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); //this will delete user from claims and
            //remove its cookies.

            //use this if you want to logout user from its google account also
            //return Redirect(@"https://www.google.com/accounts/Logout?continue=https://appengine.google.com/ ah/logout?continue=https://localhost:44398");

            return Redirect("/");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
