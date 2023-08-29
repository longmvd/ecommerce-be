using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.DTO
{
    public class ServiceResponse
    {
        public List<ValidateResult> validateResults { get; set; }

        public bool IsSuccess { get; set; } = true;

        public int ErrorCode { get; set; }

        /// <summary>
        /// Displayed message for user
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Message from system
        /// </summary>
        public string SystemMessage { get; set; }
        
        /// <summary>
        /// Response data
        /// </summary>
        public object Data { get; set; }

        public bool GetLastData { get; set; }

        public DateTime ServerTime { get; set; }



        public ServiceResponse OnSuccess(object data)
        {
            this.Data = data;
            return this;
        }

        public ServiceResponse OnException(object data)
        {
            this.IsSuccess = false;
            //todo
            return this;
        }

        public ServiceResponse OnError(object data)
        {
            this.IsSuccess = false;
            //todo
            return this;
        }
    }
}
