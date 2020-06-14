using System;

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
