﻿using WebApplication6.Context;
using WebApplication6.Models;
using Microsoft.EntityFrameworkCore;
namespace WebApplication6.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly ApplicationDbContext _context;

        public MoviesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> Add(Movie movie)
        {
            await _context.AddAsync(movie);
            await _context.SaveChangesAsync(); // Use async version
            return movie;
        }

        public async Task<Movie> Delete(Movie movie)
        {
            _context.Remove(movie);
            await _context.SaveChangesAsync(); // Use async version
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
            return await _context.Movies
                .Where(m => m.GenreId == genreId || genreId == 0)
                .OrderByDescending(m => m.Rate)
                .Include(m => m.Genre)
                .ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
            return await _context.Movies
                .Include(m => m.Genre)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Movie> Update(Movie movie)
        {
            _context.Update(movie);
            await _context.SaveChangesAsync(); // Use async version
            return movie;
        }

        Movie IMoviesService.Delete(Movie movie)
        {
            throw new NotImplementedException();
        }

        Movie IMoviesService.Update(Movie movie)
        {
            throw new NotImplementedException();
        }
    }
}
