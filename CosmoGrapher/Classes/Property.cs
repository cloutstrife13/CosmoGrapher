using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoGrapher.Classes
{
    public class Property
    {
        public Property(Object v)
        {
            id = Guid.NewGuid().ToString();
            value = v;
        }
        public string id { get; set; }
        public Object value { get; set; }
    }
}
