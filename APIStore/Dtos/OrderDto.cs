using Core.Entities.OrderAggregate;

namespace APIStore.Dtos
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public int DeliveryMethod { get; set;}
        public ShippingAddressDto Address { get; set; }
    }
}
