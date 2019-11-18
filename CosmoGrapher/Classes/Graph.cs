using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public IEnumerable<Object> GetVertex<Object>()
        {
            //{typeof(T)}
            string queryBuilder = $"g.V().hasLabel(\"Customer\")";
            var result = GetQuery(queryBuilder).Result.ToList();

            IEnumerable<dynamic> l = GetFormattedBaseModel(result);


            return l as IEnumerable<Object>;
        }

        private IEnumerable<Object> GetFormattedBaseModel(List<dynamic> l)
        {
            IList<Object> formattedList = new List<Object>();

            foreach (var entry in l)
            {
                List<Object> parsedObject = new List<Object>(); 
                foreach (var variable in entry)
                {
                    if(variable.Key == "label" || variable.Key == "type")
                        continue;

                    if (variable.Key == "properties")
                        parsedObject = GetInVertexProperties(variable.Value, parsedObject);
                    else
                    {
                        dynamic o = RuntimeBinder.GetDynamicObject(new Dictionary<string, object>()
                        {
                            {variable.Key, variable.Value}
                        });
                        parsedObject.Add(o);
                    }
                }
                formattedList.Add(parsedObject);
            }
            return formattedList;
        }

        private List<Object> GetInVertexProperties(Dictionary<string, object> vtx_var, List<Object> parsedObject)
        {
            foreach (var variable in vtx_var)
            {
                if(variable.Key == PartitionKey) continue;
                dynamic o = RuntimeBinder.GetDynamicObject(new Dictionary<string, object>()
                {
                    {variable.Key, variable.Value}
                });
                parsedObject.Add(o);
            }

            return parsedObject;
        }

        public async void AddVertex(Object model)
        {
            IVertex vertex = VertexTranslator.ConvertToLpgObject(model);
            string queryBuilder = $"g.addV(\"{model.GetType().Name}\")";

            if (vertex.Properties != null)
                queryBuilder += GetQueryFragmentAddProperties(vertex.Properties);

            if (vertex.Edges != null)
                queryBuilder += GetQueryFragmentAddEdges(vertex.Edges);

            await PostQuery(queryBuilder);
        }

        public async void UpdateVertex(Object model, string vtx_id)
        {
            IVertex vertex = VertexTranslator.ConvertToLpgObject(model);
            string queryBuilder = $"g.V(\"{vtx_id}\")";

            if (vertex.Properties != null)
                queryBuilder += GetQueryFragmentChangeProperties(vertex.Properties);

            if (vertex.Edges != null)
                queryBuilder += GetQueryFragmentAddEdges(vertex.Edges);

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
            var listSize = edgs.Count;

            foreach (KeyValuePair<string, List<Edge>> e in edgs)
            {
                dynamic pair = GetParsedKeyValuePair(e.Key, e.Value.Cast<Object>().ToList());
                queryBuilder += $".addE(\"{pair.key}\").to(g.V(\"{pair.value}\"))";
                if (i != listSize - 1)
                {
                    i++;
                    queryBuilder += ".select(\"vtx\")";
                }
            }

            return queryBuilder;
        }

        private dynamic GetParsedKeyValuePair(string k, List<Object> v)
        {
            KeyValuePair<string, List<Object>> o = new KeyValuePair<string, List<object>>(k, v);
            return new { key = o.Key, value = GetValue(o) };
        }

        private string GetValue(KeyValuePair<string, List<Object>> v)
        {
            string propType = v.Value[0].GetType().Name;
            string propVar = (propType == "Property") ? "value" : "inV";
            System.Reflection.PropertyInfo pi = v.Value[0].GetType().GetProperty(propVar);
            string propVal = (propType == "Property") ?
                (string)(pi.GetValue(v.Value[0], null)) :
                GetEdgeToVertexId((object[]) pi.GetValue(v.Value[0], null));

            return propVal;
        }

        private string GetEdgeToVertexId(object[] relatedObj)
        {
            System.Reflection.PropertyInfo pi = relatedObj[0].GetType().GetProperty("id");
            return (string) pi.GetValue(relatedObj[0], null);
        }
    }
}
