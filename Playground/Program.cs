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
            using (var adam = new AdamStorage
            {
                Username = Environment.GetEnvironmentVariable("SentinelUsername"),
                Password = Environment.GetEnvironmentVariable("SentinelPassword")
            }
         )
            {
                var c = new Company { Name = "A new company" };
                //var c = new Country() {Name = "A new country"};
                var updated = await adam.Insert(c);
                updated.Name = "A changed company";
                var replaced = await adam.Replace(updated);
                var cnew = await adam.Get<Company>(replaced.UniqueId);
                await adam.Delete(replaced);
                Console.WriteLine(c.Name);
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
