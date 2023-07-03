using Core.Entities.OrderAggregate;

namespace APIStore.Dtos
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public ShippingAddressDto ShippedToAddress { get; set; }
        public decimal ShippingPrice { get; set; }
        public string DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItemDto> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public string OrderStatus { get; set; }
        public decimal Total { get; set; }
        public string PaymentIntentId { get; set; }

    }
}
