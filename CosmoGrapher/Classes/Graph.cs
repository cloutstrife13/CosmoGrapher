using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CosmosGrapher;
using CosmosGrapher.Classes;
using CosmosGrapher.Interfaces;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;

namespace CosmoGrapher.Classes
{
    public class Graph : IGraph
    {
        private GremlinServer Server;
        private GremlinClient Client;

        public Graph(string uri_name, string db_name, string graph_name, string primary_key)
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
        }

        private async Task<ResultSet<dynamic>> GetQuery(string gremlinQuery)
        {
            return await Client.SubmitAsync<dynamic>(gremlinQuery);
        }

        private async Task PostQuery(string gremlinQuery)
        {
            await Client.SubmitAsync<dynamic>(gremlinQuery);
        }

        public IEnumerable<T> GetVertex<T>() where T : Vertex
        {
            //{typeof(T)}
            string queryBuilder = $"g.V().hasLabel(\"City\")";
            var result = GetQuery(queryBuilder).Result.ToList();
            // If response is not null
            return null;
        }

        public async void AddVertex(Object model)
        {
            IVertex vertex = VertexTranslator.ConvertToLpgObject(model);
            string queryBuilder = $"g.addV(\"{model.GetType().Name}\")";
            Dictionary<string, List<Property>> properties = vertex.Properties;
            //Dictionary<string, List<Object>> edges = vertex.Edges;

            queryBuilder += GetQueryFragmentAddProperties(properties);
            //queryBuilder += GetQueryFragmentAddEdges(edges, vertex.Id);

            await PostQuery(queryBuilder);
        }

        public async void UpdateVertex(Object model)
        {
            //string queryBuilder = $"g.addV(\"{model.GetType().Name}\")";
            await PostQuery("");
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

        private string GetQueryFragmentAddEdges(Dictionary<string, List<Edge>> edgs, string vrt_id)
        {
            string queryBuilder = "";

            var i = 0;
            var listSize = edgs.Count;

            foreach (KeyValuePair<string, List<Edge>> e in edgs)
            {
                dynamic pair = GetParsedKeyValuePair(e.Key, e.Value.Cast<Object>().ToList());
                queryBuilder += $".addE(\"{pair.key}\").to(g.V(\"{pair.value}\"))";
                if (i != listSize - 1)
                {
                    i++;
                    queryBuilder += $".V(\"{vrt_id}\")";
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
            System.Reflection.PropertyInfo pi = v.Value[0].GetType().GetProperty("value");
            return (string)(pi.GetValue(v.Value[0], null));
        }
    }
}
