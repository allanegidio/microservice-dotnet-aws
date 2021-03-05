using System;
using Advert.Models.Messages;

namespace WebAdvert.SearchWorker
{
    public static class MappingHelper
    {
        public static AdvertType Map(AdvertConfirmedMessage message)
        {
            var doc = new AdvertType
            {
                Id = message.Id,
                Title = message.Title,
                Creation = DateTime.Now
            };

            return doc;
        }
    }
}