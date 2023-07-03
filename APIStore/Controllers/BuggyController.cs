using APIStore.ResponseModule;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly StoreDbContext context;

        public BuggyController(StoreDbContext context)
        {
            this.context = context;
        }

        [HttpGet("TestText")]
        [Authorize]
        public ActionResult<string> GetText()
        {
            return "Some Text";
        }

        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest() 
        {
            var anything = this.context.Products.Find(1000);

            if(anything == null)
                return NotFound(new ApiResponse(404));
            
            return Ok();
        }

        [HttpGet("BadRequest")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }
    }
}
