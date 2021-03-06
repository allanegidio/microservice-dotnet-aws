using System.Collections.Generic;
using System.Threading.Tasks;
using WebAdvert.SearchApi.Models;

namespace WebAdvert.SearchApi.Services.Interfaces
{
    public interface ISearchService
    {
        Task<List<AdvertType>> Search(string keyword);
        Task<bool> CheckHealthAsync();
    }
}