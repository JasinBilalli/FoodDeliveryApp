namespace FoodDeliveryApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }      // Nullable
        public string? CustomerName { get; set; }    // Nullable
        public string? CustomerPhone { get; set; }   // Nullable
        public string? CustomerAddress { get; set; } // Nullable
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; }          // Nullable
        public string? PaymentMethod { get; set; }   // Nullable
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}