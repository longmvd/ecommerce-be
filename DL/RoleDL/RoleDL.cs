﻿using Dapper;
using Ecommerce.Common.Constants;
using ECommerce.Common.Entities;
using ECommerce.Common.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DL.RoleDL
{
    public class RoleDL : BaseDL, IRoleDL
    {
        public IEnumerable<User> GetUserByRoleName(string userCode)
        {
            string storedProcedure = String.Format("Proc_role_SelectByUserCode", typeof(User).Name);
            var parameters = new DynamicParameters();
            parameters.Add($"@UserName", userCode);
            OpenDB();
            var result = mySqlConnection.Query<User>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            CloseDB();
            
            return result;
        }
    }
}
