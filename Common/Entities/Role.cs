using Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Entities
{
    public class Role: BaseEntity
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        public List <User>? Users { get; set; }



    }
}
