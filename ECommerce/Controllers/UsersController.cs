using ECommerce.BL;
using ECommerce.Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        protected IUserBL _userBL;

        public UsersController(IUserBL userBL) : base(userBL)
        {
            _userBL = userBL;
            this.CurrentType = typeof(User);
        }

    }
}
