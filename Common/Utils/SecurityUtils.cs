using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common
{
    public class SecurityUtils
    {
        public static string GetColumns(string? columnNames, BaseEntity entity)
        {
            var columns = columnNames.Split(",").Select(col => col.Trim());
            foreach(var column in columns)
            {
                if (!entity.IsValidColumn(column))
                {
                    Console.WriteLine(column + " is invalid.");
                    throw new Exception(column + " is invalid.");
                }
            }
            return string.Join(",", columns);
            

        }

        public static string SafetyCharsForLIKEOperator(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Replace("\\", "\\\\\\\\").Replace("%", "\\%");
        }

        public static bool IsValidColumn(Type type, string column)
        {
            var result =  type.GetProperty(column);
            return result != null;
        }
    }
}
