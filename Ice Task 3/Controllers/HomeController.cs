using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ice_Task_3.Models;
using Ice_Task_3.Interfaces;

namespace Ice_Task_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFineRepository _fineRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IBookRepository bookRepository,
            ITransactionRepository transactionRepository,
            IFineRepository fineRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;
            _transactionRepository = transactionRepository;
            _fineRepository = fineRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Get some stats for the dashboard
            ViewBag.TotalBooks = (await _bookRepository.GetAllBooksAsync()).Count();
            ViewBag.OverdueBooks = (await _transactionRepository.GetOverdueTransactionsAsync()).Count();
            ViewBag.UnpaidFines = (await _fineRepository.GetUnpaidFinesAsync()).Count();

            // Check if user is logged in (simplified approach)
            if (TempData.ContainsKey("UserId"))
            {
                ViewBag.IsLoggedIn = true;
                ViewBag.UserName = TempData["UserName"]?.ToString();
                ViewBag.UserRole = TempData["UserRole"]?.ToString();
                ViewBag.UserId = TempData["UserId"];

                // Keep the TempData alive for next request
                TempData.Keep("UserId");
                TempData.Keep("UserName");
                TempData.Keep("UserRole");
            }
            else
            {
                ViewBag.IsLoggedIn = false;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}