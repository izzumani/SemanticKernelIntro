using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace semantic_kernel
{
    public  class Setting
    {
        public static (string apiKey, string? orgId) LoadFromFile()
        {
            IConfiguration config = new ConfigurationBuilder()
                        .AddUserSecrets<Setting>()
                        .Build();


            try
            {
             
                string? apiKey = string.IsNullOrEmpty(config.GetValue<string>("apiKey")) ? null : config.GetValue<string>("apiKey");
                string? orgId = config.GetValue<string>("orgId") ?? null;



                return (apiKey, orgId);
                
            }
            catch (Exception ex)
            {

                Console.WriteLine("Something went wrong: " + ex.Message);
                return ("", "");
            }
        }
    }
}
