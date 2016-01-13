using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext;
using Amica.vNext.Storage;
using Amica.vNext.Models;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Test().Wait();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }


        static async Task Test()
        {
            var cache = new SqliteObjectCache {ApplicationName = "Playground"};

            using (var adam = new RemoteRepository
					{
						DiscoveryService = new Discovery()
                        {
							BaseAddress = new Uri("http://10.0.2.2:9000/"),
							Cache = cache   
						}, 

						Username = "nicola",
						Password = "nicola",

						ClientId = Environment.GetEnvironmentVariable("SentinelClientId"),
						Cache = cache
					})
            {
                // targeted bulk get 
                var ret = await adam.Get<Country>("5603b9ea38345bc2cd1c7ec3");

                Console.WriteLine(ret[0].CompanyId);

            } 

            //await hdp.UpateAsync(dp);
            //await hdp.UpateAsync(cdp);
            //await hdp.UpdateNazioniAsync(nr);
            //await hdp.UpdateAziendeAsync(nr);
            //await hdp.GetAziendeAsync(dp);
            //await hdp.GetNazioniAsync(dp);
            //}
            //catch (Exception e) 
            //{
            //throw e;
            //}

            //Console.WriteLine(hdp.HttpResponse.StatusCode);

        }
    }
}
