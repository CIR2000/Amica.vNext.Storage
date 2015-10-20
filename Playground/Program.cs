using System;
using System.Data;
using System.Threading.Tasks;
using Amica.vNext;
using Amica.vNext.Data;
using Amica.vNext.Models;
using Nito.AsyncEx;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AsyncContext.Run(() => Test());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }


        static async Task Test()
        {

            var adam = new AdamStorage()
            {
                DiscoveryServiceAddress = new Uri("http://10.0.2.2:9000"),
                Username = Environment.GetEnvironmentVariable("SentinelUsername"),
                Password = Environment.GetEnvironmentVariable("SentinelPassword")
            };
            var c = await adam.Get<Company>("55fa6ad138345bbcc0bdf45b");
            Console.WriteLine(c.Name);
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
