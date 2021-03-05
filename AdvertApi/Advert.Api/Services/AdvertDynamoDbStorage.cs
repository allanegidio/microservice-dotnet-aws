using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Advert.Api.Entities;
using Advert.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;

namespace Advert.Api.Services
{
  public class AdvertDynamoDbStorage : IAdvertStorageService
  {
    private readonly IMapper _mapper;

    public AdvertDynamoDbStorage(IMapper mapper)
    {
      _mapper = mapper;
    }

    public async Task<string> Add(AdvertModel model)
    {
      var dbModel = _mapper.Map<AdvertDbModel>(model);
      dbModel.Id = new Guid().ToString();
      dbModel.Creation = DateTime.UtcNow;
      dbModel.Status = AdvertStatus.Peding;

      using (var client = new AmazonDynamoDBClient())
      {
        using (var context = new DynamoDBContext(client))
        {
          await context.SaveAsync(dbModel);
        }
      }

      return dbModel.Id;
    }

    public async Task Confirm(ConfirmAdvertModel model)
    {
      using (var client = new AmazonDynamoDBClient())
      {
        using (var context = new DynamoDBContext(client))
        {
          var record = await context.LoadAsync<AdvertDbModel>(model.Id);

          if (record == null) throw new KeyNotFoundException($"A record with ID={model.Id} was not found.");
          if (model.Status == AdvertStatus.Active)
          {
            record.Status = AdvertStatus.Active;
            await context.SaveAsync(record);
          }
          else
          {
            await context.DeleteAsync(record);
          }
        }
      }
    }

    public async Task<bool> CheckHealthAsync()
    {
      using (var client = new AmazonDynamoDBClient())
      {
        var table = await client.DescribeTableAsync("Advert");
        return table.Table.TableStatus == "Active";
      }
    }

    public async Task<AdvertModel> GetByIdAsync(string id)
    {
      using (var client = new AmazonDynamoDBClient())
      {
        using (var context = new DynamoDBContext(client))
        {
          var dbModel = await context.LoadAsync<AdvertDbModel>(id);

          if (dbModel == null)
            throw new KeyNotFoundException();

          return _mapper.Map<AdvertModel>(dbModel);
        }
      }
    }
  }
}