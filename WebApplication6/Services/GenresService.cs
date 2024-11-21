using WebApplication6.Context;
using WebApplication6.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication6.Services
{
    public class GenresService : IGenresService
    {
        private readonly ApplicationDbContext _context;

        public GenresService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ajout d'un genre
        public async Task<Genre> Add(Genre genre)
        {
            await _context.AddAsync(genre);
            await _context.SaveChangesAsync(); // Utilisation de la version asynchrone
            return genre;
        }

        // Suppression d'un genre
        public async Task<Genre> Delete(Genre genre)
        {
            _context.Remove(genre);
            await _context.SaveChangesAsync(); // Utilisation de la version asynchrone
            return genre;
        }

        // Récupérer tous les genres
        public async Task<IEnumerable<Genre>> GetAll()
        {
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        // Récupérer un genre par son ID
        public async Task<Genre> GetById(byte id)
        {
            return await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
        }

        // Vérifier si le genre existe
        public async Task<bool> IsValidGenre(byte id)
        {
            return await _context.Genres.AnyAsync(g => g.Id == id);
        }

        // Mise à jour d'un genre
        public async Task<Genre> Update(Genre genre)
        {
            _context.Update(genre);
            await _context.SaveChangesAsync(); // Utilisation de la version asynchrone
            return genre;
        }
    }
}
