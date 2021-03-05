using Advert.Models;

namespace WebAdvert.Web.Clients.Requests
{
  public class ConfirmAdvertRequest
  {
    public string Id { get; set; }
    public AdvertStatus Status { get; set; }
  }
}