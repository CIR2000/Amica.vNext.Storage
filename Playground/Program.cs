using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                //var c = new Company { Name = "A new company" };
                var countries = new List<Country>
                {
                    new Country {Name = "country1", CompanyId="5603b9ea38345bc2cd1c7ec3"},
                    new Country {Name = "country2", CompanyId="1234"},
                    new Country {Name = "country3", CompanyId="5603b9ea38345bc2cd1c7ec3"},
                };
                //var c = new Country() {Name = "A new country"};
                //var updated = await adam.Insert(c);
                //updated.Name = "A changed company";
                //var replaced = await adam.Replace(updated);
                //var cnew = await adam.Get<Company>(replaced.UniqueId);
                //await adam.Delete(replaced);

                //var wannabes = new List<string> {"notreally", "5603b9ea38345bc2cd1c7ec3"};

                var inserted = await adam.Insert<Country>(countries);
                var deleted = await adam.Delete<Country>(inserted);
                Console.WriteLine(deleted);
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
