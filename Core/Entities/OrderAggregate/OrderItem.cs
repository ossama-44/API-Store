namespace Core.Entities.OrderAggregate
{
    public class OrderItem : BaseEntity
    {
        public OrderItem()
        {
            
        }
        public OrderItem(ProductItemOrder itemOrdered, decimal price, int quantity)
        {
            ItemOrderd = itemOrdered;
            Price = price;
            Quantity = quantity;
        }

        public int Id { get; set; }
        public ProductItemOrder ItemOrderd { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}