using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CosmosGrapher.Classes;

namespace CosmosGrapher.Interfaces
{
    public interface IVertex
    {
        string Id { get; set; }

        Dictionary<string, List<Property>> Properties { get; set; }

        Dictionary<string, List<Edge>> Edges { get; set; }

        //IGraph Graph { get; }
    }
}
