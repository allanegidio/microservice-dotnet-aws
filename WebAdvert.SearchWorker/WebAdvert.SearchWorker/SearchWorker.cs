using System.Text.Json;
using System.Threading.Tasks;
using Advert.Models.Messages;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Nest;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace WebAdvert.SearchWorker
{
  public class SearchWorker
  {
    private readonly IElasticClient _client;

    public SearchWorker() : this (ElasticSearchHelper.GetInstance(ConfigurationHelper.Instance))
    {
        
    }

    public SearchWorker(IElasticClient client)
    {
      _client = client;
    }

    public async Task Function(SNSEvent snsEvent, ILambdaContext context)
    {
        
      foreach (var record in snsEvent.Records)
      {
        //  Test for see the log on AWS Cloud Watch
        context.Logger.LogLine(record.Sns.Message);
        
        var message = JsonSerializer.Deserialize<AdvertConfirmedMessage>(record.Sns.Message);

        var advertDocument = MappingHelper.Map(message);

        await _client.IndexDocumentAsync(advertDocument);

      }
    }


    //
    
    //
    // public void Function(SNSEvent snsEvent, ILambdaContext context)
    // {
    //     foreach(var record in snsEvent.Records)
    //     {
    //         context.Logger.LogLine(record.Sns.Message);
    //     }
    // }
  }
}
