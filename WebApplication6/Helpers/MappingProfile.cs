using AutoMapper;
using WebApplication6.Dtos;
using WebApplication6.Models;
namespace WebApplication6.Helpers

{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieDetailsDto>();

            CreateMap<MovieDto, Movie>()
                .ForMember(dest => dest.Poster, opt => opt.Ignore());
        }
    }

}
