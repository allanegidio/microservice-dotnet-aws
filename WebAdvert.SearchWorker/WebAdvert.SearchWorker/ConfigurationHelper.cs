namespace WebAdvert.SearchWorker
{
    public static class ConfigurationHelper
    {
      private static IConfiguration _configuration = null;
      public static IConfiguration Instance {
      get 
      {
        if(_configuration == null)
        {
          _configuration = new ConfigurationHelper()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsetting.json")
                                .Build();
        }
      }
    }
  }
}