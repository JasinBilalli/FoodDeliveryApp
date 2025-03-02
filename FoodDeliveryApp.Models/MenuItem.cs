namespace FoodDeliveryApp.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }       // Nullable
        public string? Description { get; set; } // Nullable
        public string? ImagePath { get; set; }   // Nullable
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public Category? Category { get; set; }  // Nullable
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}