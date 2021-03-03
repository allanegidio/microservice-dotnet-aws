using System.Threading.Tasks;
using Advert.Models;

namespace Advert.Api.Services
{
    public interface IAdvertStorageService
    {
        Task<string> Add(AdvertModel model);
        Task Confirm(ConfirmAdvertModel model);

    }
}