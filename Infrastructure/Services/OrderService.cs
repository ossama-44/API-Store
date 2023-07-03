using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService paymentService;

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IPaymentService paymentService )
        {
            this.basketRepository = basketRepository;
            this.unitOfWork = unitOfWork;
            this.paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, ShippingAddress address)
        {
            var basket = await this.basketRepository.GetBasketAsync(basketId);

            var items = new List<OrderItem>();

            foreach (var item in basket.BasketItems)
            {
                var productItem = await this.unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                var itemOrdered = new ProductItemOrder(productItem.Id, productItem.Name, productItem.PictureUrl);

                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);

                items.Add(orderItem);
            }
            // Get DeliveryMethod
            var deliveryMethod = await this.unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // Calculate subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            // TODO Payment stuff
            // Check if Order Exist

            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var existOrder = await this.unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);

            if (existOrder != null)
            {
                this.unitOfWork.Repository<Order>().Delete(existOrder);
                await this.paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            // Create Order
            var order = new Order (buyerEmail, address, deliveryMethod, items, subtotal, basket.PaymentIntentId);

            this.unitOfWork.Repository<Order>().Add(order);

            var result = await this.unitOfWork.Complete();

            if (result <= 0)
                return null;

            //Delete Basket
            await this.basketRepository.DeleteBasketAsync(basketId);

            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetODeliveryMethodsAsync()
            => await this.unitOfWork.Repository<DeliveryMethod>().ListAllAsync();

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var orderSpec = new OrderWithItemsSpecifications(id, buyerEmail);

            return await this.unitOfWork.Repository<Order>().GetEntityWithSpecifications(orderSpec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var orderSpec = new OrderWithItemsSpecifications(buyerEmail);

            return await this.unitOfWork.Repository<Order>().ListAsync(orderSpec);
        }
    }
}
