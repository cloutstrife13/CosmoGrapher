using System.Collections.Generic;
using CosmoGrapher.Interfaces;

namespace CosmoGrapher.Classes
{
    public class Vertex : IVertex
    {
        public string Id { get; set; }

        public Dictionary<string, List<Property>> Properties { get; set; }

        public Dictionary<string, List<Edge>> Edges { get; set; }
    }
}
