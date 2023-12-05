using ECommerce.BL;
using ECommerce.DL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BL
{
    public class TestBL : BaseBL, ITestBL
    {
        public TestBL(ITestDL productDL, IConfiguration configuration) : base(productDL, configuration)
        {
            this._baseDL = productDL;
        }
    }
}
