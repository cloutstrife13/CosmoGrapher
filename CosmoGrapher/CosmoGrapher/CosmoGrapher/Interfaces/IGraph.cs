using System;
using System.Collections.Generic;

namespace CosmoGrapher.Interfaces
{
    public interface IGraph
    {
        /// <summary>
        /// Retrieve a set of vertices based on the label.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetVertex<T>();

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
        void UpdateVertex(Object model, string vtx_id);

        /// <summary>
        /// Deletes a vertex on the backend side.
        /// </summary>
        /// <param name="id"></param>
        void DeleteVertex(string id);
    }
}
