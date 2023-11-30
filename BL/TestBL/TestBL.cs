using ECommerce.BL;
using ECommerce.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BL
{
    public class TestBL : BaseBL, ITestBL
    {
        public TestBL(ITestDL productDL) : base(productDL)
        {
            this._baseDL = productDL;
        }
    }
}
