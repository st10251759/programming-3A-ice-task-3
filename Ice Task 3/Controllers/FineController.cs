using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ice_Task_3.Models;
using Ice_Task_3.Interfaces;

namespace Ice_Task_3.Controllers
{
    public class FineController : Controller
    {
        private readonly IFineRepository _fineRepository;
        private readonly IUserRepository _userRepository;

        public FineController(IFineRepository fineRepository, IUserRepository userRepository)
        {
            _fineRepository = fineRepository;
            _userRepository = userRepository;
        }

        // GET: Fine
        public async Task<IActionResult> Index()
        {
            var fines = await _fineRepository.GetAllFinesAsync();
            return View(fines);
        }

        // GET: Fine/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var fine = await _fineRepository.GetFineByIdAsync(id);
            if (fine == null)
            {
                return NotFound();
            }

            return View(fine);
        }

        // GET: Fine/UserFines/5
        public async Task<IActionResult> UserFines(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var fines = await _fineRepository.GetFinesByUserIdAsync(id);
            ViewBag.UserName = $"{user.FirstName} {user.LastName}";

            return View(fines);
        }

        // GET: Fine/Unpaid
        public async Task<IActionResult> Unpaid()
        {
            var unpaidFines = await _fineRepository.GetUnpaidFinesAsync();
            return View(unpaidFines);
        }

        // GET: Fine/Pay/5
        public async Task<IActionResult> Pay(int id)
        {
            var fine = await _fineRepository.GetFineByIdAsync(id);
            if (fine == null)
            {
                return NotFound();
            }

            return View(fine);
        }

        // POST: Fine/Pay/5
        [HttpPost, ActionName("Pay")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayConfirmed(int id)
        {
            var fine = await _fineRepository.PayFineAsync(id);
            if (fine == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}