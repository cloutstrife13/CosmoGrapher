using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_Smart_Semantic_Framework.MasterModels
{
    public class Exhibitor
    {
        public Exhibitor() {}
        public string buzz_id { get; set; }
        public string id { get; set; }
        public string biography { get; set; }
        public string name { get; set; }
        public bool is_published { get; set; }
        public string stand { get; set; }
        public string email { get; set; }
        public Customer[] employs { get; set; }
    }
}
