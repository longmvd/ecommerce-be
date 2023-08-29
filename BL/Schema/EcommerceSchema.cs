using GraphQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BL
{
    public class EcommerceSchema : GraphQL.Types.Schema
    {
        public EcommerceSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<BaseEntityQuery>();


        }
    }
}
