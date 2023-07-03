namespace Core.Entities.OrderAggregate
{
    public class ShippingAddress
    {
        public ShippingAddress(string fistName, string lastName, string street, string city, string state, string zipCode)
        {
            FistName = fistName;
            LastName = lastName;
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        public string FistName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}