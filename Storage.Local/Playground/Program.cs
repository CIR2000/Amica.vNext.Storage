using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amica.Models;
using Amica.Storage;

namespace Playground
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

        private static async Task Test()
        {
            var r = new LocalRepository()
            {
                ApplicationName = "TestApp"
            };

            var stamp = DateTime.Now;
            var c = new Company() { UniqueId = "k1", Name = "C1", ETag = "etag", Created = DateTime.MinValue, Updated = stamp};
            await r.Insert(c);
            var companies = await r.Get<Company>(stamp.AddDays(1));
            Console.WriteLine(companies.Count);
            companies = await r.Get<Company>(stamp);
            Console.WriteLine(companies.Count);
            companies = await r.Get<Company>(stamp.AddMilliseconds(-1));
            Console.WriteLine(companies.Count);

            //var c = new Company() { UniqueId = "k1" };
            //var copy = await r.Get(c);
            //c.Updated = DateTime.Now;

            //await r.Replace(c);
        }
    }
}
