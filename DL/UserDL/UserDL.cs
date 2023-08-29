using Dapper;
using ECommerce.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DL
{
    public class UserDL: BaseDL, IUserDL
    {
        public IEnumerable<Role> GetRoleByUserCode(string userCode)
        {
            string storedProcedure = String.Format("Proc_role_SelectByUserCode", typeof(Role).Name);
            var parameters = new DynamicParameters();
            parameters.Add($"@UserCode", userCode);
            OpenDB();
            var result = mySqlConnection.Query<Role>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            CloseDB();
            return result;
        }
    }
}
