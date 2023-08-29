using ECommerce.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DL
{
    public interface IRoleDL : IBaseDL
    {
        IEnumerable<User> GetUserByRoleName(string roleName);
    }
}
