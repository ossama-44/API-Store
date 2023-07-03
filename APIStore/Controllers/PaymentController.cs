using APIStore.ResponseModule;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace APIStore.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentController> logger;
        private const string WhSecret = "whsec_556fee7e9666eb790f487c4081527c49e7af62778cc55ba5b77f441efba6ff4f";

        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger)
        {
            this.paymentService = paymentService;
            this.logger = logger;
        }

        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await this.paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null)
            {
                return BadRequest(new ApiResponse(400, "Error with your basket"));
            }
            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json,Request.Headers["Stripe-Signature"], WhSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentPaymentFailed:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    this.logger.LogInformation("Payment Failed: ", intent.Id);
                    order = await this.paymentService.UpdateOrderPaymentFailed(intent.Id);
                    this.logger.LogInformation("Payment Failed: ", order.Id);
                    break;


                case Events.PaymentIntentSucceeded:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    this.logger.LogInformation("Payment Succeeded: ", intent.Id);
                    order = await this.paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    this.logger.LogInformation("Order Updated to Payment Succeeded: ", order.Id);
                    break;
            }

            return new EmptyResult();
        }
    }
}
