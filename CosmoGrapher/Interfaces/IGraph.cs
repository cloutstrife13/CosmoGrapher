using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosGrapher.Classes;
using Gremlin.Net.Driver;

namespace CosmosGrapher.Interfaces
{
    public interface IGraph
    {
        /// <summary>
        /// Retrieve a set of vertices based on the label.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetVertex<T>() where T : Vertex;

        /// <summary>
        /// Adds a new vertex on the backend side.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        void AddVertex(Object model);

        /// <summary>
        /// Updates a vertex on the backend side.
        /// </summary>
        /// <param name="vertex"></param>
        void UpdateVertex(Object model);

        /// <summary>
        /// Deletes a vertex on the backend side.
        /// </summary>
        /// <param name="id"></param>
        void DeleteVertex(string id);
    }
}
