﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.API.Data;
using Movies.API.Model;

namespace Movies.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class MoviesController : ControllerBase {
        private readonly MoviesContext _context;

        public MoviesController(MoviesContext context) {
            _context = context;
        }
        
        [HttpGet("{name}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovieByOwnerName(string name) {
            return await _context.Movie
                .Where(x => x.Owner.ToLower() == name.ToLower())
                .ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovie() {
            return await _context.Movie.ToListAsync();
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Movie>> GetMovie(int id) {
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
                return NotFound();
            return movie;
        }
        

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie) {
            if (id != movie.Id)
                return BadRequest();
            _context.Entry(movie).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!MovieExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie) {
            _context.Movie.Add(movie);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id) {
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
                return NotFound();
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        private bool MovieExists(int id) {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
