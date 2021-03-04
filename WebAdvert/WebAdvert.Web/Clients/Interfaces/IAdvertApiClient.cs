using System.Threading.Tasks;
using Advert.Models;
using WebAdvert.Web.Clients.Requests;

namespace WebAdvert.Web.Clients.Interfaces
{
    public interface IAdvertApiClient
    {
         Task<CreateAdvertResponse> Create(CreateAdvertRequest model);
    }
}