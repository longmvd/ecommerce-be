using Common.Entities;
using ECommerce.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Entities
{
    [TableConfig("user", "")]
    public class User : BaseEntity
    {
        [KeyAttribute]
        public Guid UserID { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string? Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        [NotMapped]
        public List<Role>? Roles { get; set; }

        public string? RoleNames { get; set; }

        public int? Status { get; set; }

        public User()
        {
            this.EntityDetailConfigs = new List<EntityDetailConfig> { 
                new EntityDetailConfig() 
                { 
                    DetailTableName = "UserRole",
                    PropertyNameOnMaster = "Users",
                } };
        }
    }
}
