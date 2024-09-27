using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.APIs.Errors;

namespace OrderManagementSystem.APIs.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)] //to un document this controller
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            if (code == 400)
                return BadRequest(new ApiResponse(400));
            else if (code == 401)
                return NotFound(new ApiResponse(401));
            else if (code == 404)
                return NotFound(new ApiResponse(404));
            else
                return StatusCode(code);

        }
    }
}
