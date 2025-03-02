using FoodDeliveryApp.Data;
using FoodDeliveryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodDeliveryApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FoodDeliveryContext _context;

        public HomeController(FoodDeliveryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            var menuItems = _context.MenuItems.Include(m => m.Category).ToList();
            ViewBag.Categories = categories;
            if (!categories.Any())
            {
                ViewBag.Error = "No categories found in the database.";
            }
            return View(menuItems);
        }

        public IActionResult Category(int id)
        {
            var category = _context.Categories.Find(id);
            var items = _context.MenuItems.Where(m => m.CategoryId == id).ToList();
            ViewBag.Category = category;
            return View(items);
        }

        public IActionResult Details(int id)
        {
            var item = _context.MenuItems.Find(id);
            return View(item);
        }

        [HttpPost]
        public IActionResult AddToCart(int menuItemId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = _context.Orders.FirstOrDefault(o => o.CustomerId == userId && o.Status == "Pending");
            if (order == null)
            {
                order = new Order
                {
                    CustomerId = userId,
                    CustomerName = User.Identity!.Name,
                    OrderDate = DateTime.Now,
                    Status = "Pending"
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                MenuItemId = menuItemId,
                Quantity = quantity
            };
            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();

            return Json(new { success = true, cartCount = _context.OrderItems.Count(oi => oi.Order.CustomerId == userId && oi.Order.Status == "Pending") });
        }

        public IActionResult Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .FirstOrDefault(o => o.CustomerId == userId && o.Status == "Pending");
            return View(order ?? new Order());
        }

        [HttpPost]
        public IActionResult Checkout(string phone, string address, string paymentMethod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefault(o => o.CustomerId == userId && o.Status == "Pending");
            if (order != null && order.OrderItems.Any())
            {
                order.CustomerPhone = phone;
                order.CustomerAddress = address;
                order.PaymentMethod = paymentMethod;
                order.Status = "Ordered";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}