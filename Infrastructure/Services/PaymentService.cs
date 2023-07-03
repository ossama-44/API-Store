using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = Core.Entities.Product;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepository;
        private readonly IConfiguration config;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IBasketRepository basketRepository,
            IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            this.basketRepository = basketRepository;
            this.config = config;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = this.config["StripeSettings:Secretkey"];

            var basket = await this.basketRepository.GetBasketAsync(basketId);

            if (basket == null)
                return null;

            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await this.unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in basket.BasketItems)
            {
                var productItem = await this.unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                if(item.Price != productItem.Price)
                    item.Price = productItem.Price;
            }

            var service = new PaymentIntentService();

            PaymentIntent intent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice *100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100)
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }
            await this.basketRepository.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await this.unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);

            if (order is null)
                return null;

            order.OrderStatus = OrderStatus.PaymentFailed;

            this.unitOfWork.Repository<Order>().Update(order);

            await this.unitOfWork.Complete();

            return order;
        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

            var order = await this.unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);

            if (order is null)
                return null;

            order.OrderStatus = OrderStatus.PaymentReceived;

            this.unitOfWork.Repository<Order>().Update(order);

            await this.unitOfWork.Complete();

            return order;
        }
    }
}
