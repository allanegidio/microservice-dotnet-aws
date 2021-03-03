using Advert.Api.Entities;
using Advert.Models;
using AutoMapper;

namespace Advert.Api.Mapper
{
    public class AdvertProfile : Profile
    {
        public AdvertProfile()
        {
            CreateMap<AdvertModel, AdvertDbModel>();
        }
        
    }
}