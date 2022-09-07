using Microsoft.AspNetCore.Mvc;

namespace Movies.Client.Controllers {
    public class AccountController : Controller {
        public AccountController() { }
        public IActionResult AccessDenied() {
            return View();
        }
    }
}
