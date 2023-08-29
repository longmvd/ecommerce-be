using ECommerce.Common;
using ECommerce.Common.DTO;
using ECommerce.Common.Entities;
using ECommerce.Common.Enums;
using ECommerce.Common.Extension;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphQLController : ControllerBase
    {
        //private readonly ISchema _schema;
        //private IDocumentExecuter _documentExecuter;
        //public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter)
        //{
        //    _schema = schema;
        //    _documentExecuter = documentExecuter;
        //}

        [HttpPost]
        public IActionResult Post([FromBody] PagingRequest pagingRequest)
        {
            var filter = JsonConvert.DeserializeObject<JArray>(pagingRequest.CustomFilter);
            var sort = JsonConvert.DeserializeObject<List<PagingSort>>(pagingRequest.Sort);
            string sortOrder = BuildSortOrder(sort);
            string whereCondition = "";
            var user = new User();
            user.Set("UserID", Guid.NewGuid());
            var dic = new Dictionary<string, object>();
            foreach (var element in filter)
            {
                if(element == null)
                {
                    //return "";
                }
                
                
                Console.WriteLine(element.GetType() == typeof(JArray));
            }
            BuildCondition(filter);
            return Ok(new { Condition = BuildCondition(filter),  SortOrder = sortOrder });
        }

        public string BuildCondition(JToken filters)
        {
            string whereCondition = "";
            int index = 0;
            foreach (var element in filters)
            {
                if (element == null)
                {
                    return whereCondition;
                }
                if(element.GetType() == typeof(JValue))
                {
                    if(index == 2)
                    {
                        whereCondition = String.Format(whereCondition, element.ToString());
                    }
                    else
                    {
                        whereCondition += " " + GetCondition(element.ToString()) + " ";
                    }
                }
                if(element.GetType() == typeof(JArray))
                {
                    whereCondition += this.BuildCondition(element);
                }
                index++;
                Console.WriteLine(element.GetType() == typeof(JArray));
            }
            return "("+ whereCondition+")";
        }
        public string GetCondition(string value)
        {
            switch (value)
            {
                case Operator.Equal:
                    return "= '{0}'";
                case Operator.Contain:
                    return "LIKE '%{0}%'";
                    //return "LIKE";
                case Operator.NotEqual:
                    return "<> '{0}'";
                case Operator.LessOrEqual:
                    return "<= '{0}'";
                case Operator.GreaterOrEqual:
                    return ">= '{0}'";
                case Operator.LessThan:
                    return "< '{0}'";
                case Operator.GreaterThan:
                    return "> '{0}'";
                case Operator.StartWith:
                    //return "LIKE";
                    return "LIKE '%{0}'";
                case Operator.EndWith:
                    //return "LIKE";
                    return "LIKE '{0}%'";

            }
            return value;
        }

        public string BuildSortOrder(IEnumerable<PagingSort> sorts)
        {
            string sortOrder = "";
            foreach (var sort in sorts)
            {
                sortOrder += " " + sort.Selector + " " + (sort.Desc ? "DESC" : "ASC") + ",";
            }
            return sortOrder.Remove(sortOrder.Length - 1);
        }

        
    }
}
