using System;

namespace CosmoGrapher.Classes
{
    public class Edge
    {
        public Edge(Object v)
        {
            id = Guid.NewGuid().ToString();
            inV = v;
        }
        // Edge ID
        public string id { get; set; }
        // Reference to Vertex ID
        public Object inV { get; set; }
    }
}
