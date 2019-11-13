using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CosmosGrapher.Classes;

namespace CosmosGrapher
{
    public class VertexTranslator
    {
        public static Vertex ConvertToLpgObject(Object model)
        {
            Vertex v = new Vertex();

            // 1. convert properties to GraphSON properties object
            v.Properties = ConvertToGraphsonProperties(model);

            // 2. convert l

            return v;
        }

        private static Dictionary<string, List<Property>> ConvertToGraphsonProperties(Object model)
        {
            List<PropertyInfo> properties = model.GetType().GetProperties().ToList();
            Dictionary<string, List<Property>> graphsonProperties = new Dictionary<string, List<Property>>();
            foreach (var prop in properties)
            {
                //if (prop.Name == "buzz_id") continue;
                string propName = prop.Name;
                List<Property> propVal = new List<Property>();
                propVal.Add(new Property(prop.GetValue(model, null)));
                graphsonProperties.Add(propName, propVal);
            }

            return graphsonProperties;
        }
    }
}
