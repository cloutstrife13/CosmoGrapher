using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using CosmoGrapher.Interfaces;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;

namespace CosmoGrapher.Classes
{
    public class Graph : IGraph
    {
        private GremlinServer Server;
        private GremlinClient Client;
        private string PartitionKey;

        public Graph(string uri_name, string db_name, string graph_name, string primary_key, string partition_key)
        {
            Server = new GremlinServer(
                $"{uri_name}.gremlin.cosmos.azure.com",
                443,
                true,
                $"/dbs/{db_name}/colls/{graph_name}",
                primary_key
            );
            Client = new GremlinClient(Server, new GraphSON2Reader(), new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType);
            PartitionKey = partition_key;
        }

        private async Task<ResultSet<dynamic>> GetQuery(string gremlinQuery)
        {
            return await Client.SubmitAsync<dynamic>(gremlinQuery);
        }

        private async Task PostQuery(string gremlinQuery)
        {
            await Client.SubmitAsync<dynamic>(gremlinQuery);
        }

        public IEnumerable<T> GetVertex<T>()
        {
            string queryBuilder = $"g.V().hasLabel(\"{typeof(T).Name}\")";
            var result = GetQuery(queryBuilder).Result.ToList();

            IEnumerable<T> models_unfiltered = GetFormattedBaseModel<T>(result);

            return models_unfiltered;
        }

        private IEnumerable<T> GetFormattedBaseModel<T>(List<dynamic> l)
        {
            IList<T> formattedList = new List<T>();

            foreach (var entry in l)
            {
                Object obj = Activator.CreateInstance(typeof(T));
                foreach (var variable in entry)
                {
                    if(variable.Key == "label" || variable.Key == "type")
                        continue;

                    if (variable.Key == "properties")
                        SetInVertexProperties(variable.Value, obj, typeof(T));

                    else
                        SetGenericObjectProperty(obj, typeof(T), variable.Key, variable.Value);
                }
                formattedList.Add((T)obj);
            }

            return formattedList;
        }

        private void SetInVertexProperties(Dictionary<string, object> vtx_var, Object obj, Type type)
        {
            foreach (var variable in vtx_var)
            {
                bool cond1 = (variable.Key == PartitionKey);
                bool cond2 = (obj.GetType().GetProperty(variable.Key).PropertyType.BaseType.Name != "Object");
                if(cond1 || cond2) continue;

                List<object> vars = (variable.Value as IEnumerable<object>).ToList();
                var val = (vars[0] as Dictionary<string, object>)["value"];
                if(val == String.Empty || val == null) continue;

                SetGenericObjectProperty(obj, type, variable.Key, val);
            }
        }

        private void SetGenericObjectProperty(Object obj, Type type, string key, dynamic value)
        {
            PropertyInfo prop = type.GetProperty($"{key}");
            prop.SetValue(obj, value, null);
        }

        public async void AddVertex(Object model)
        {
            // Vertex Translation important for making the distinction between Properties and Edges
            IVertex vertex = VertexTranslator.ConvertToLpgObject(model);
            string queryBuilder = $"g.addV(\"{model.GetType().Name}\")";

            if (vertex.Properties != null)
                queryBuilder += GetQueryFragmentAddProperties(vertex.Properties);

            if (vertex.Edges != null)
                queryBuilder += GetQueryFragmentAddEdges(vertex.Edges);

            await PostQuery(queryBuilder);
        }

        // To be changed so that vtx_id is retrieved from model
        public async void UpdateVertex(Object model, string vtx_id)
        {
            // Vertex Translation important for making the distinction between Properties and Edges
            IVertex vertex = VertexTranslator.ConvertToLpgObject(model);
            string queryBuilder = $"g.V(\"{vtx_id}\")";

            if (vertex.Properties != null)
                queryBuilder += GetQueryFragmentChangeProperties(vertex.Properties
                    .Where(x => !x.Value
                        .Select(y => y.value)
                        .Contains(null))
                    .ToDictionary(z => z.Key, z => z.Value));

            if (vertex.Edges != null)
                queryBuilder += GetQueryFragmentAddEdges(vertex.Edges
                    .Where(x => !x.Value
                        .Select(y => y.inV)
                        .Contains(null))
                    .ToDictionary(z => z.Key, z => z.Value));

            await PostQuery(queryBuilder);
        }

        public async void DeleteVertex(string vtx_id)
        {
            string queryBuilder = $"g.V(\"{vtx_id}\").drop()";
            await PostQuery(queryBuilder);
        }

        private string GetQueryFragmentAddProperties(Dictionary<string, List<Property>> prps)
        {
            string queryBuilder = "";
            foreach (KeyValuePair<string, List<Property>> p in prps)
            {
                dynamic pair = GetParsedKeyValuePair(p.Key, p.Value.Cast<Object>().ToList());
                queryBuilder += $".property(\"{pair.key}\", \"{pair.value}\")";
            }

            return queryBuilder;
        }

        private string GetQueryFragmentChangeProperties(Dictionary<string, List<Property>> prps)
        {
            string queryBuilder = "";
            foreach (KeyValuePair<string, List<Property>> p in prps)
            {
                if(p.Key == PartitionKey || p.Key == "id") continue;
                dynamic pair = GetParsedKeyValuePair(p.Key, p.Value.Cast<Object>().ToList());
                queryBuilder += $".property(\"{pair.key}\", \"{pair.value}\")";
            }

            return queryBuilder;
        }

        private string GetQueryFragmentAddEdges(Dictionary<string, List<Edge>> edgs)
        {
            string queryBuilder = ".as(\"vtx\")";

            var i = 0;
            var edgesSize = edgs.Count;

            foreach (KeyValuePair<string, List<Edge>> e in edgs)
            {
                List<object> objs = GetEdgeRelationships(e.Value.Cast<Object>().ToList()[0]);
                var j = 0;
                var listSize = objs.Count;
                foreach (dynamic o in objs)
                {
                    queryBuilder += $".addE(\"{e.Key}\").to(g.V(\"{o.id}\"))";
                    if (j < listSize) {
                        j++;
                        if (j <= listSize - 1) queryBuilder += ".select(\"vtx\")";
                    }
                }

                if (i < edgesSize)
                {
                    i++;
                    if (i <= edgesSize - 1) queryBuilder += ".select(\"vtx\")";
                }
            }

            return queryBuilder;
        }

        private List<object> GetEdgeRelationships(Object obj)
        {
            PropertyInfo prop = obj.GetType().GetProperty("inV");
            return (prop.GetValue(obj, null) as IEnumerable<object>).ToList();
        }

        private dynamic GetParsedKeyValuePair(string k, List<Object> v)
        {
            KeyValuePair<string, List<Object>> o = new KeyValuePair<string, List<object>>(k, v);
            return new { key = o.Key, value = GetValue(o) };
        }

        private dynamic GetValue(KeyValuePair<string, List<Object>> v)
        {
            string propType = v.Value[0].GetType().Name;
            string propVar = (propType == "Property") ? "value" : "inV";
            PropertyInfo pi = v.Value[0].GetType().GetProperty(propVar);
            dynamic propVal = (propType == "Property") ?
                (pi.GetValue(v.Value[0], null)) :
                GetEdgeToVertexId((object[]) pi.GetValue(v.Value[0], null));

            return propVal;
        }

        private string GetEdgeToVertexId(object[] relatedObj)
        {
            PropertyInfo pi = relatedObj[0].GetType().GetProperty("id");
            return (string) pi.GetValue(relatedObj[0], null);
        }
    }
}
