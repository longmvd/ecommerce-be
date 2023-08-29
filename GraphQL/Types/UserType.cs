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
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Field(x => x.Roles).ResolveWith<Resolver>(x => x.GetRoles(default!, default!));
        }

        private class Resolver
        {
            public IEnumerable<Role> GetRoles(User user, IUserDL userDL)
            {
                return userDL.GetRoleByUserCode(user.UserCode);
            }
        }
    }
}
