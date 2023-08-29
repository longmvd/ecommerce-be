using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Entities
{
    public class EntityDetailConfig
    {
        public string DetailTableName { get; set; }

        public string ForeignKeyName { get; set; }

        public string PropertyNameOnMaster { get; set; }

        public bool OnDeleteCascade { get; set; }

        public EntityDetailConfig(string detailTableName, string foreignKeyName, string propertyNameOnMaster, bool onDeleteCascade)
        {
            DetailTableName = detailTableName;
            ForeignKeyName = foreignKeyName;
            PropertyNameOnMaster = propertyNameOnMaster;
            OnDeleteCascade = onDeleteCascade;
        }

        public EntityDetailConfig()
        {
        }
    }
}
