using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_Smart_Semantic_Framework.MasterModels
{
    public class Badge
    {
        public Badge() {}
        public string buzz_id { get; set; }
        public string id { get; set; }
        public string role { get; set; }
        public string name { get; set; }
        public string colour { get; set; }
        public Customer[] owned_by { get; set; }
    }
}
