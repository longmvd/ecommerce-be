using Common.Entities;
using Ecommerce.Types;
using ECommerce.DL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BL
{
    public class BaseEntityQuery: ObjectGraphType
    {
        public BaseEntityQuery(IBaseDL baseDL) {
            Field<ListGraphType<BaseEntityType>>("BaseEntity", resolve: context => baseDL.GetAllBase());
            //Field<ListGraphType<BaseEntityType>>("User", resolve: contex => )
        }
    }
}
