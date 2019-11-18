using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_Smart_Semantic_Framework.MasterModels
{
    public class Customer
    {
        public Customer() {}
        public string buzz_id { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string title { get; set; }
        public string job_title { get; set; }
        public string sex { get; set; }
        public string nationality { get; set; }
        public string company { get; set; }
        public Exhibitor[] employed_by { get; set; }
        public Badge[] owns { get; set; }
    }
}
