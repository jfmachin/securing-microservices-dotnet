using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Client.ApiServices;
using Movies.Client.Models;

namespace Movies.Client.Controllers {
    [Authorize]
    public class MoviesController : Controller {
        private readonly IMovieApiService movieApiService;

        public MoviesController(IMovieApiService movieApiService) {
            this.movieApiService = movieApiService;
        }

        [Authorize(Roles="admin")]
        public async Task<IActionResult> OnlyAdmin() {
            var userInfo = await movieApiService.GetUserInfo();
            return View(userInfo);
        }

        public async Task<IActionResult> Index() {
            foreach (var claim in User.Claims)
                Console.WriteLine($"type {claim.Type} - value {claim.Value}");
            
            return View(await movieApiService.GetMoviesByOwnerName(User.Identity.Name));
        }

        public async Task Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> Details(int id) {
            var movie = await movieApiService.GetMovie(id);
            return movie == null ? NotFound() : View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Rating,ReleaseDate,ImageUrl,Owner")] Movie movie) {
            return View(await movieApiService.CreateMovie(movie));
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return View();
            /*
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
            */
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Rating,ReleaseDate,ImageUrl,Owner")] Movie movie)
        {
            return View();
            /*
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
            */
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id) {
            var movie = await movieApiService.GetMovie(id);
            return movie == null ? NotFound() : View(await movieApiService.DeleteMovie(id));
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return View();
            /*
            if (_context.Movie == null)
            {
                return Problem("Entity set 'MoviesClientContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            */
        }

        private bool MovieExists(int id)
        {
            return true;
          //return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
