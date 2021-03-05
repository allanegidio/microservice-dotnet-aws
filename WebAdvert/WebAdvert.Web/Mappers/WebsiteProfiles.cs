using AutoMapper;
using WebAdvert.Web.Clients.Requests;
using WebAdvert.Web.Models.AdvertManagement;

namespace WebAdvert.Web.Mappers
{
    public class WebsiteProfiles : Profile
    {
        public WebsiteProfiles()
        {
            CreateMap<CreateAdvertViewModel, CreateAdvertRequest>().ReverseMap();
        }
        
    }
}