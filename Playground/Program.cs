using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amica.vNext.Storage;
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
            using (var adam = new RemoteRepository { Username = "nicola", Password = "nicola" } )
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
