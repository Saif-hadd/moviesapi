using WebApplication6.Models;

namespace WebApplication6.Services
{
    public interface IGenresService
    {
        Task<IEnumerable<Genre>> GetAll();
        Task<Genre> GetById(byte id);
        Task<Genre> Add(Genre genre);
        Task<Genre> Update(Genre genre); // Méthode asynchrone
        Task<Genre> Delete(Genre genre); // Méthode asynchrone
        Task<bool> IsValidGenre(byte id); // Utilisation de la version asynchrone
    }
}
