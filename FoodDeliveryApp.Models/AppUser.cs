using Microsoft.AspNetCore.Identity;
namespace FoodDeliveryApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? Role { get; set; } // Nullable
    }
}