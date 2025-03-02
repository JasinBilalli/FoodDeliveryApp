using FoodDeliveryApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryApp.Controllers
{
    [Authorize(Roles = "Worker")]
    public class WorkerController : Controller
    {
        private readonly FoodDeliveryContext _context;

        public WorkerController(FoodDeliveryContext context)
        {
            _context = context;
        }

        public IActionResult CurrentOrders()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == "Pending" || o.Status == "Ordered")
                .ToList();
            return View(orders);
        }

        public IActionResult DoneOrders()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == "Ready to Send" || o.Status == "Done")
                .ToList();
            return View(orders);
        }

        [HttpPost]
        public IActionResult MarkReady(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Status = "Ready to Send";
                _context.SaveChanges();
            }
            return RedirectToAction("CurrentOrders");
        }
    }
}