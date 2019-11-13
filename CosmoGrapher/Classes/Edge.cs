using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosGrapher.Classes
{
    public class Edge
    {
        // Edge ID
        public string id { get; set; }
        // Reference to Vertex ID
        public string inV { get; set; }
    }
}
