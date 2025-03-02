namespace FoodDeliveryApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; } // Nullable
        public string? Name { get; set; }     // Nullable
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}