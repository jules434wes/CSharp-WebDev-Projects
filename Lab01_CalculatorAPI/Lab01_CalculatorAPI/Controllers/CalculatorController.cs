using Microsoft.AspNetCore.Mvc;

namespace CalculatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculatorController : ControllerBase
    {
        [HttpPost("add")]
        public IActionResult Add([FromBody] CalculationRequest request)
        // [FromBody]：屬性標註，表示 request 參數從 HTTP 請求的 body 中取得
        {
            try
            {
                // 計算邏輯
                var result = request.FirstNumber + request.SecondNumber;

                return Ok(new CalculationResponse
                // Ok()：ASP.NET Core 的方法，產生 HTTP 200 OK 狀態碼回應
                {
                    Result = result,
                    Operation = "加法",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CalculationResponse
                // BadRequest()：ASP.NET Core 的方法，產生 HTTP 400 Bad Request 狀態碼回應
                {
                    Success = false,
                    ErrorMessage = "計算發生錯誤：" + ex.Message
                });
            }
        }

        [HttpPost("subtract")]
        public IActionResult Subtract([FromBody] CalculationRequest request)
        {
            try
            {
                var result = request.FirstNumber - request.SecondNumber;

                return Ok(new CalculationResponse
                {
                    Result = result,
                    Operation = "減法",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CalculationResponse
                {
                    Success = false,
                    ErrorMessage = "計算發生錯誤：" + ex.Message
                });
            }
        }

        [HttpPost("multiply")]
        public IActionResult Multiply([FromBody] CalculationRequest request)
        {
            try
            {
                var result = request.FirstNumber * request.SecondNumber;

                return Ok(new CalculationResponse
                {
                    Result = result,
                    Operation = "乘法",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CalculationResponse
                {
                    Success = false,
                    ErrorMessage = "計算發生錯誤：" + ex.Message
                });
            }
        }

        [HttpPost("divide")]
        public IActionResult Divide([FromBody] CalculationRequest request)
        {
            try
            {
                if (request.SecondNumber == 0)
                {
                    return BadRequest(new CalculationResponse
                    {
                        Success = false,
                        ErrorMessage = "除數不能為零！"
                    });
                }

                var result = request.FirstNumber / request.SecondNumber;
                
                return Ok(new CalculationResponse
                {
                    Result = result,
                    Operation = "除法",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new CalculationResponse
                {
                    Success = false,
                    ErrorMessage = "計算發生錯誤：" + ex.Message
                });
            }
        }
    }

    // 請求資料模型
    public class CalculationRequest
    {
        public decimal FirstNumber { get; set; }
        public decimal SecondNumber { get; set; }
    }

    // 回應資料模型
    public class CalculationResponse
    {
        public decimal Result { get; set; }
        public string Operation { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}