/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Spotify.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("~/")]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            ViewBag.UserName = await HttpContext.GetTokenAsync("Spotify", "refresh_token");
            return View();
        }
        [Authorize]
        
        public async System.Threading.Tasks.Task<ActionResult> Test()
        {
            ViewBag.Token = await HttpContext.GetTokenAsync("Spotify", "access_token");
            return View("Test");
        }
    }
}
