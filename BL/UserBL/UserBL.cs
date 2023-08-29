using ECommerce.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BL
{
    public class UserBL : BaseBL, IUserBL
    {
        public UserBL(IBaseDL baseDL) : base(baseDL)
        {

        }
    }
}
