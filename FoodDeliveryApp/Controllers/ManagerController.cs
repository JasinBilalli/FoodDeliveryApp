using FoodDeliveryApp.Data;
using FoodDeliveryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly FoodDeliveryContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ManagerController(FoodDeliveryContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .ToList();
            decimal revenue = orders
                .Where(o => o.Status == "Done")
                .SelectMany(o => o.OrderItems)
                .Sum(oi => oi.Quantity * oi.MenuItem.Price);
            ViewBag.Revenue = revenue;
            return View(orders);
        }

        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(string email, string password, string role)
        {
            var user = new AppUser { UserName = email, Email = email, Role = role };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return RedirectToAction("Users");
        }

        public IActionResult Orders()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }

        [HttpPost]
        public IActionResult MarkOrdered(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Status = "Ordered";
                _context.SaveChanges();
            }
            return RedirectToAction("Orders");
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
            return RedirectToAction("Orders");
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
            return RedirectToAction("Orders");
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
            return RedirectToAction("Orders");
        }

        public IActionResult Menu()
        {
            var menuItems = _context.MenuItems.Include(m => m.Category).ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(menuItems);
        }

        [HttpPost]
        public IActionResult AddMenuItem(string title, string description, string imagePath, int categoryId, decimal price)
        {
            var item = new MenuItem { Title = title, Description = description, ImagePath = imagePath, CategoryId = categoryId, Price = price };
            _context.MenuItems.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Menu");
        }

        [HttpPost]
        public IActionResult UploadMenu(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    _context.MenuItems.Add(new MenuItem
                    {
                        Title = worksheet.Cells[row, 1].Text,
                        Description = worksheet.Cells[row, 2].Text,
                        ImagePath = worksheet.Cells[row, 3].Text,
                        CategoryId = int.Parse(worksheet.Cells[row, 4].Text),
                        Price = decimal.Parse(worksheet.Cells[row, 5].Text)
                    });
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Menu");
        }
    }
}