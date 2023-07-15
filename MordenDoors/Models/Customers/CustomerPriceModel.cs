namespace MordenDoors.Models.Customers
{
    public class CustomerPriceModel
    {
        public int ID { get; set; }
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }
}