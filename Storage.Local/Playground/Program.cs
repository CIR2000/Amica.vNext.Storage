using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amica.vNext.Models;
using Amica.vNext.Storage;

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

            var c = new Company() { UniqueId = "k1", Name = "C1", ETag = "etag", Created = DateTime.MinValue, Updated = DateTime.MinValue };
            await r.Insert(c);

            //var c = new Company() { UniqueId = "k1" };
            //var copy = await r.Get(c);
            //c.Updated = DateTime.Now;

            await r.Replace(c);
            Console.WriteLine(c.Updated);
        }
    }
}
