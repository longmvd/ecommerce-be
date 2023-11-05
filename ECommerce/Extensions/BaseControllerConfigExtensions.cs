using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Extension
{
    public static class BaseControllerConfigExtensions
    {
        public static IMvcBuilder AddBaseControllerConfig(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            builder.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
            return builder;
        }
    }
}
