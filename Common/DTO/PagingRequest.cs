using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.DTO
{
    public class PagingRequest
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public bool Desc { get; set; }

        public string CustomFilter { get; set; }

        public string? Sort { get; set; }
    }
}
