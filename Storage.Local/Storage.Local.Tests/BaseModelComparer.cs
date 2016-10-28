using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amica.Models;

namespace Storage.Local.Tests
{
    internal class BaseModelComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var m1 = x as BaseModel;
            if (m1 == null) throw new ArgumentException();

            var m2 = y as BaseModel;
            if (m2 == null) throw new ArgumentException();

			if (
				m1.Created == m2.Created && 
				m1.Updated == m2.Updated && 
				m1.ETag == m2.ETag && 
				m1.UniqueId == m2.UniqueId ) return 0;
            return 1;
        }
    }
}
