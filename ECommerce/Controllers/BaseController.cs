﻿using Common.Entities;
using ECommerce.BL;
using ECommerce.Common.DTO;
using ECommerce.Common.Resources;
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
        public async Task<IActionResult> GetAll()
        {
            var response = new ServiceResponse();
            var res = await _baseBL.GetAll<BaseEntity>(this.CurrentType);
            return Ok(response.OnSuccess(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(res), typeof(List<>).MakeGenericType(this.CurrentType))));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID([FromRoute] string id)
        {
            var response = new ServiceResponse();
            var res = await _baseBL.GetByID<BaseEntity>(this.CurrentType, id);
            return Ok(response.OnSuccess(res));
        }

        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] PagingRequest pagingRequest)
        {
            if(pagingRequest == null)
            {
                return BadRequest();
            }
            var response = new ServiceResponse();
            var result = await _baseBL.GetPagingAsync(CurrentType, pagingRequest);
            return Ok(response.OnSuccess(result));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Save([FromBody] object stringEntity)
        {
            var entity = (BaseEntity)JsonConvert.DeserializeObject(stringEntity.ToString(), this.CurrentType);
            return Ok(await _baseBL.SaveAsync(entity));
        }

        [HttpPost("list-save")]
        public virtual async Task<IActionResult> SaveList([FromBody] object stringEntity)
        {
            try
            {
                
                var entities = (IEnumerable<BaseEntity>)JsonConvert.DeserializeObject(stringEntity.ToString(), typeof(List<>).MakeGenericType(this.CurrentType));
            
                return Ok(await _baseBL.SaveListAsync(entities));

            }catch (Exception ex)
            {
                return BadRequest(
                    new ServiceResponse().OnException(new ExceptionResponse(){ ExceptionMessage = ex.Message })

                ) ;
            }
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromBody] object stringEntity, [FromRoute] string id)
        {
            try
            {
                var response = new ServiceResponse();
                if(stringEntity == null)
                {
                    return BadRequest(response.OnError(new ErrorResponse(){ ErrorMessage = Resource.DEV_NullRequestObject }));
                }
                var entity = (BaseEntity)JsonConvert.DeserializeObject(stringEntity.ToString(), this.CurrentType);

                entity.SetPrimaryKey(id);
                return Ok(await _baseBL.UpdateOneAsync(entity));

            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new ServiceResponse().OnException(new ExceptionResponse() { ExceptionMessage = ex.Message })

                );
            }
        }

        [HttpPatch("{id}")]
        public virtual async Task<IActionResult> UpdateFields([FromBody] object stringEntity, [FromRoute] string id)
        {
            try
            {
                var response = new ServiceResponse();
                if (stringEntity == null)
                {
                    return BadRequest(response.OnError(new ErrorResponse() { ErrorMessage = Resource.DEV_NullRequestObject }));
                }
                var entity = (BaseEntity)Activator.CreateInstance(CurrentType);
                var fieldUpdates = JsonConvert.DeserializeObject<List<EntityFieldUpdate>>(stringEntity.ToString());

                entity.SetPrimaryKey(id);
                return Ok(await _baseBL.SaveChangesAsync(entity, fieldUpdates));

            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new ServiceResponse().OnException(new ExceptionResponse() { ExceptionMessage = ex.Message })

                );
            }
        }
    }
}
