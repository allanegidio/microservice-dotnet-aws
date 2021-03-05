using Advert.Models;
using AutoMapper;
using WebAdvert.Web.Clients.Requests;

namespace WebAdvert.Web.Mappers
{
    public class AdvertApiProfile : Profile
    {
        public AdvertApiProfile()
        {
            CreateMap<AdvertModel, CreateAdvertRequest>().ReverseMap();
            CreateMap<Advert.Models.CreateAdvertResponse, WebAdvert.Web.Clients.Responses.CreateAdvertResponse>().ReverseMap();
            CreateMap<ConfirmAdvertRequest, ConfirmAdvertModel>().ReverseMap();;
        }
    }
}