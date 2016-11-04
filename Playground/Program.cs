using System;
using System.Threading.Tasks;
using Amica.Storage;
using Amica.Models.Documents;
using SimpleObjectCache;
using Amica.Models;

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
            var adam = new RemoteRepository()
            {
                ClientId = Environment.GetEnvironmentVariable("SentinelClientId"),
                DiscoveryUri = new Uri("http://10.0.2.2:9000/"),
                LocalCache = new SqliteObjectCache {ApplicationName = "Playground"},
				UserAccount = new UserAccount() { Username="nicola", Password="nicola" }
            };
            // targeted bulk get 
            //var ret = await adam.Get<Country>("5603b9ea38345bc2cd1c7ec3");
    //        var doc = new Invoice()
    //        {
				//UniqueId="56b0be8a38345b7d881b8974"
    //            //CompanyId = "56b0705f38345b7d881b896f",
    //            //Contact = { Name = "Nicola", Address = "Addr", Vat = "Vat" },
    //            //Items = new List<DocumentItem>()
    //            //        {
    //            //            new DocumentItem() {Sku = "Sku1", Description = "D1"},
    //            //        },
    //            //Total = 10,
    //        };
            //var ret = await adam.Insert(doc);
			//Console.WriteLine(ret.CompanyId);
            var test = await adam.Get<Company>();
            //var test = await adam.Get(doc);
			//Console.WriteLine(test.GetType());

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
