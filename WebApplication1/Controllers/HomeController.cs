using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.WsFederation;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                //System.IdentityModel.Tokens. bootstrapContext = User.Identities.First().BootstrapContext;
                //var token = bootstrapContext.SecurityToken;

                try
                {
                    System.Security.Claims.ClaimsIdentity identity = User.Identity as System.Security.Claims.ClaimsIdentity;
                    string userAccessToken = identity.BootstrapContext as string;
                    string userName = (User.FindFirst(ClaimTypes.Upn))?.Value;

                    var token = AuthenticationHttpContextExtensions.GetTokenAsync(
                        Request.HttpContext, WsFederationDefaults.AuthenticationScheme, "access_token").Result;
                    //Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties
                }
                catch
                {
                    // Swallow
                }
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
