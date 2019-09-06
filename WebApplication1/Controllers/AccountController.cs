using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.Extensions.Logging;
using WebApplication1.Util;

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
            XElement x = XElement.Parse(token);
            ViewData["Token"] = x.ToString();

            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var redirectUrl = Url.Content("~/");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                WsFederationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var redirectUrl = Url.Content("~/");
            return SignOut(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                WsFederationDefaults.AuthenticationScheme);
        }
    }
}