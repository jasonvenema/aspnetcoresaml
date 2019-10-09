using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Util;
using System;
using Sustainsys.Saml2.AspNetCore2;

namespace WebApplication1.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Token()
        {
            var token = TokenCache.GetToken(User.Identity.Name);

            if (!String.IsNullOrEmpty(token))
            {
                XElement x = XElement.Parse(token);
                ViewData["Token"] = x.ToString();
            }
            else
            {
                ViewData["Token"] = "Token not found. Try logging out and then logging back in again.";
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var redirectUrl = Url.Content("~/");

            // ++WS-Federation
            //return Challenge(
            //    new AuthenticationProperties { RedirectUri = redirectUrl },
            //    WsFederationDefaults.AuthenticationScheme);
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, Saml2Defaults.Scheme);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var redirectUrl = Url.Content("~/");
            return SignOut(new AuthenticationProperties { RedirectUri = redirectUrl }, Saml2Defaults.Scheme);

            // ++WS-Federation
            //return SignOut(
            //    new AuthenticationProperties { RedirectUri = redirectUrl },
            //    WsFederationDefaults.AuthenticationScheme);
        }
    }
}