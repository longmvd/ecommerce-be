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
    [TableConfig(tableName: "test")]
    public class Test: BaseEntity
    {
        [Key]
         public int ID { get; set; }

        public string Name { get; set; }

        [NotMapped]
        [FilterColumn]
        public int Status { get; set; }

        public DateTime? StartDate{ get; set; }
    }
}
