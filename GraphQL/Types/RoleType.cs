using ECommerce.Common.Entities;
using ECommerce.DL;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.GraphQL.Types
{
    public class RoleType : ObjectType<Role>
    {
        protected override void Configure(IObjectTypeDescriptor<Role> descriptor)
        {
            descriptor.Field(x => x.Users).ResolveWith<Resolvers>(x => x.GetUsers(default!, default!));
        }

        private class Resolvers
        {
            public IEnumerable<User> GetUsers(Role role, IRoleDL roleDL)
            {
                return roleDL.GetUserByRoleName(role.RoleName);
            }
        }
    }
}
