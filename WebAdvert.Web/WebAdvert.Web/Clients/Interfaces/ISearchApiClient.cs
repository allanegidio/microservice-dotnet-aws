  
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAdvert.Web.Models;

namespace WebAdvert.Web.Clients.Interfaces
{
  public interface ISearchApiClient
  {
    Task<List<AdvertType>> Search(string keyword);
  }
}