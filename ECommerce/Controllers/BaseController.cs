using Common.Entities;
using ECommerce.BL;
using ECommerce.Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IBaseBL _baseBL;

        private Type _currentType;

        protected Type CurrentType
        {
            get { 
                if(_currentType == null)
                {
                    throw new Exception("Current type is null");
                }
                return _currentType; 
            }
            set
            {
                _currentType = value;
            }
        }


        public BaseController(IBaseBL baseBL)
        {
            _baseBL = baseBL;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_baseBL.GetAll());
        }

        [HttpPost]
        public virtual IActionResult Save([FromBody] object stringEntity)
        {
            var entity =(BaseEntity)JsonConvert.DeserializeObject(stringEntity.ToString(), this.CurrentType);
            return Ok(_baseBL.Save(entity));
        }
    }
}
