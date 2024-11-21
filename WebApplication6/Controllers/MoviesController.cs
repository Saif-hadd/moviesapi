using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Dtos;
using WebApplication6.Models;
using WebApplication6.Services;
using System.IO;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMoviesService _moviesService;
        private readonly IGenresService _genresService;

        private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private readonly long MaxAllowedPosterSize = 1048576; // 1 MB

        public MoviesController(IMoviesService moviesService, IGenresService genresService, IMapper mapper)
        {
            _moviesService = moviesService;
            _genresService = genresService;
            _mapper = mapper;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _moviesService.GetAll();
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }

        // GET: api/Movies/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
                return NotFound($"No movie found with ID: {id}");

            var dto = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(dto);
        }

        // GET: api/Movies/GetByGenreId?genreId={genreId}
        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _moviesService.GetAll(genreId);
            if (movies == null || !movies.Any())
                return NotFound($"No movies found for genre ID: {genreId}");

            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster is required!");

            var fileExtension = Path.GetExtension(dto.Poster.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExtension))
                return BadRequest("Only .png and .jpg images are allowed!");

            if (dto.Poster.Length > MaxAllowedPosterSize)
                return BadRequest("Max allowed size for poster is 1 MB!");

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var isValidGenre = await _genresService.IsValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid genre ID!");

            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();

            try
            {
                await _moviesService.Add(movie);

                // Vérifiez que l'ID est bien généré
                if (movie.Id <= 0)
                {
                    return StatusCode(500, "Failed to generate a valid movie ID.");
                }

                return CreatedAtAction(nameof(GetByIdAsync), new { id = movie.Id }, movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // PUT: api/Movies/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
                return NotFound($"No movie was found with ID {id}");

            var isValidGenre = await _genresService.IsValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid genre ID!");

            if (dto.Poster != null)
            {
                var fileExtension = Path.GetExtension(dto.Poster.FileName).ToLower();
                if (!_allowedExtensions.Contains(fileExtension))
                    return BadRequest("Only .png and .jpg images are allowed!");

                if (dto.Poster.Length > MaxAllowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1 MB!");

                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.Storeline = dto.Storeline;
            movie.Rate = dto.Rate;

            try
            {
                _moviesService.Update(movie);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Movies/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
                return NotFound($"No movie was found with ID {id}");

            try
            {
                _moviesService.Delete(movie);
                return Ok(movie);  // Retourne le film supprimé
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
