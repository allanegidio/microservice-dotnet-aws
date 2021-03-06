using Advert.Models;
using AutoMapper;
using WebAdvert.Web.Clients.Requests;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.Models.Home;

namespace WebAdvert.Web.Mappers
{
    public class WebsiteProfiles : Profile
    {
        public WebsiteProfiles()
        {
            CreateMap<CreateAdvertViewModel, CreateAdvertRequest>().ReverseMap();
            CreateMap<AdvertModel, Advertisement>().ReverseMap();

            CreateMap<Advertisement, IndexViewModel>()
                .ForMember(dest => dest.Title, src => src.MapFrom(prop => prop.Title))
                .ForMember(dest => dest.Image, src=> src.MapFrom(prop => prop.FilePath));

            CreateMap<AdvertType, SearchViewModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(prop => prop.Id))
                .ForMember(dest => dest.Title, src => src.MapFrom(prop => prop.Title));
        }
        
    }
}