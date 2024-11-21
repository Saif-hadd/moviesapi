using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Dtos
{
    public class GenreDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
