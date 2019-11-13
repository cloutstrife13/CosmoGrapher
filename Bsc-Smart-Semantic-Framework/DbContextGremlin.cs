using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Linq;
using Bsc_Smart_Semantic_Framework.LpgModels;
using CosmoGrapher.Classes;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json;

namespace Bsc_Smart_Semantic_Framework
{
    public class DbContextGremlin
    {
        public Graph DefaultModel = new Graph(
            "livebuzz-transition-test",
            "sample-database",
            "cosmographer-test",
            "hxpERNXwXaWlEHL2lXKQ3YsNNJHw5TnHk59oX9RT7X7ROstxpsrcw1OkYp0Vjaqq5slsw9pLEq9p1FvuLzqPHg=="
        );
    }
}