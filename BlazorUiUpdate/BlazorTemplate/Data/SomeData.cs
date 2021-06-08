using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTemplate.Data
{
    public class SomeData
    {
        public int Id { get; set; }
        public int SomeInteger { get; set; }

        public SomeData(int id, int someInteger)
        {
            Id = id;
            SomeInteger = someInteger;
        }
    }
}
