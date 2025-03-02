using FoodDeliveryApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryApp.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DriverController : Controller
    {
        private readonly FoodDeliveryContext _context;

        public DriverController(FoodDeliveryContext context)
        {
            _context = context;
        }

        public IActionResult ReadyOrders()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == "Ready to Send")
                .ToList();
            return View(orders);
        }

        [HttpPost]
        public IActionResult MarkDone(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Status = "Done";
                _context.SaveChanges();
            }
            return RedirectToAction("ReadyOrders");
        }

        [HttpPost]
        public IActionResult MarkFailed(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Status = "Failed";
                _context.SaveChanges();
            }
            return RedirectToAction("ReadyOrders");
        }
    }
}