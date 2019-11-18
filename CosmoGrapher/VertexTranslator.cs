using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CosmoGrapher.Classes;

namespace CosmoGrapher
{
    public class VertexTranslator
    {
        public static Vertex ConvertToLpgObject(Object model)
        {
            Vertex v = new Vertex();

            // 1. convert properties to GraphSON properties object
            v.Properties = ConvertToGraphsonProperties(model);

            // 2. convert l
            v.Edges = ConvertToGraphsonEdges(model);

            return v;
        }

        private static Dictionary<string, List<Property>> ConvertToGraphsonProperties(Object model)
        {
            List<PropertyInfo> properties = model.GetType().GetProperties().ToList();
            Dictionary<string, List<Property>> graphsonProperties = new Dictionary<string, List<Property>>();
            foreach (var prop in properties)
            {
                Object propValue = prop.GetValue(model, null);
                if (prop.PropertyType.BaseType.Name != "Object") continue;
                string propName = prop.Name;
                List<Property> propVal = new List<Property>();
                propVal.Add(new Property(propValue));
                graphsonProperties.Add(propName, propVal);
            }

            return graphsonProperties;
        }

        private static Dictionary<string, List<Edge>> ConvertToGraphsonEdges(Object model)
        {
            List<PropertyInfo> edges = model.GetType().GetProperties().ToList();
            Dictionary<string, List<Edge>> graphsonEdges = new Dictionary<string, List<Edge>>();
            foreach (var edge in edges)
            {
                Object edgeValue = edge.GetValue(model, null);
                if (edge.PropertyType.BaseType.Name == "Object" || edgeValue == null) continue;
                string edgeName = edge.Name;
                List<Edge> propVal = new List<Edge>();
                propVal.Add(new Edge(edge.GetValue(model, null)));
                graphsonEdges.Add(edgeName, propVal);
            }

            return graphsonEdges;
        }
    }
}
