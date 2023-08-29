using Common.Entities;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Types
{
    public class BaseEntityType : ObjectGraphType<BaseEntity>
    {
        public BaseEntityType() 
        {
            
            Field(x => x.ModifiedBy);
            Field(x => x.ModifiedDate, type: typeof(DateTimeGraphType));
            Field(x => x.CreatedBy);
            Field(x => x.CreatedDate, type: typeof(DateTimeGraphType));
        }
    }
}
