using System;
using Advert.Models;
using Amazon.DynamoDBv2.DataModel;

namespace Advert.Api.Entities
{
    [DynamoDBTable("Advert")]
    public class AdvertDbModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Title { get; set; }
        
        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public double Price { get; set; }

        [DynamoDBProperty]
        public DateTime Creation { get; set; }

        [DynamoDBProperty]
        public AdvertStatus Status { get; set; }
    }
}