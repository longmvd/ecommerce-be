using ECommerce.Common.Entities;
using ECommerce.DL;
using ECommerce.DL.RoleDL;
using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.GraphQL.Queries
{
    public class Query
    {
        public IQueryable<User> GetUser([Service]IUserDL userDL)
        {
            return userDL.GetAll<User>().AsQueryable();
        }

        //public User GetUser()
        //{
        //    return new User()
        //    {
        //        Email = "long@gmail.com",
        //        FirstName = "Long",
        //        LastName = "Mike",
        //    };
        //}
    }
}
