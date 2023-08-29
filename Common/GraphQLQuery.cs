using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common
{
    public class GraphQLQuery
    {
        public  string OperationName { get; set; }
        public string Query { get; set; }

        public JObject Variables { get; set; }
    }
}
