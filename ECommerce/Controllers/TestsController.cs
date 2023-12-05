using ECommerce.BL;
using ECommerce.Common.DTO;
using ECommerce.Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    //public class TestsController : BaseController
    //{
    //    public TestsController(ITestBL productBL) : base(productBL)
    //    {
    //        CurrentType = typeof(Test);
    //        this._baseBL = productBL;
    //    }

    //    [HttpGet("test")]
    //    public async Task<IActionResult> test()
    //    {
    //        var response = new ServiceResponse();
    //        var validate = new List<ValidateResult>() { };
    //        validate.Add(new ValidateResult() { AdditionInfo = new { QuantityAvailable = 3 }, ErrorMessage = "Số lượng mua vượt quá số lượng hàng trong kho" });
    //        response.OnError(new { Data = validate });
    //        return Ok(response);
    //    }
    //}
}
