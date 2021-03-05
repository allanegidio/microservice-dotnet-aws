using System.Threading.Tasks;
using WebAdvert.Web.Clients.Requests;
using WebAdvert.Web.Clients.Responses;

namespace WebAdvert.Web.Clients.Interfaces
{
    public interface IAdvertApiClient
    {
         Task<CreateAdvertResponse> CreateAsync(CreateAdvertRequest request);
         Task<bool> ConfirmAsync(ConfirmAdvertRequest request);
    }
}