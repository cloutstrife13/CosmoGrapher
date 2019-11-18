using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gremlin.Net;
using Newtonsoft.Json;
using CosmoGrapher.Interfaces;

namespace CosmoGrapher.Classes
{
    public class Vertex : IVertex
    {
        public string Id { get; set; }

        public Dictionary<string, List<Property>> Properties { get; set; }
        // Vertex ID
        // Label of Class
        public Dictionary<string, List<Edge>> Edges { get; set; }
    }
}
