using APIStore.Dtos;
using APIStore.Extensions;
using APIStore.ResponseModule;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIStore.Controllers
{

    public class OrdersController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public OrdersController(IOrderService orderService, IMapper mapper) 
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var address = this.mapper.Map<ShippingAddress>(orderDto.Address);

            var order = await this.orderService.CreateOrderAsync(email, orderDto.DeliveryMethod,orderDto.BasketId, address);

            if (order == null)
                return BadRequest(new ApiResponse(400, "Some Error Occurred when Creation Order!!"));

            return Ok(order);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderByIdForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var order = await this.orderService.GetOrderByIdAsync(id, email);

            if (order == null)
                return NotFound(new ApiResponse(404, "There Is No Order Out Of Given Id"));

            return Ok(this.mapper.Map<OrderDetailsDto>(order));

        }

        [HttpGet("GetAllOrdersForUser")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var order = await this.orderService.GetOrdersForUserAsync( email);

            return Ok(this.mapper.Map<IReadOnlyList<OrderDetailsDto>>(order));

        }

        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<OrderDetailsDto>>> GetDeliveryMethod()
            => Ok(await this.orderService.GetODeliveryMethodsAsync());

    }
}
