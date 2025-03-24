using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ice_Task_3.Models;
using Ice_Task_3.Interfaces;

namespace Ice_Task_3.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFineRepository _fineRepository;

        public TransactionController(
            ITransactionRepository transactionRepository,
            IBookRepository bookRepository,
            IUserRepository userRepository,
            IFineRepository fineRepository)
        {
            _transactionRepository = transactionRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _fineRepository = fineRepository;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var transactions = await _transactionRepository.GetAllTransactionsAsync();
            return View(transactions);
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Borrow
        public async Task<IActionResult> Borrow()
        {
            // Get available books and users for dropdown lists
            var books = await _bookRepository.GetAllBooksAsync();
            var users = await _userRepository.GetAllUsersAsync();

            ViewBag.Books = new SelectList(books, "Id", "Title");
            ViewBag.Users = new SelectList(users, "Id", "Email");

            return View();
        }

        // POST: Transaction/Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow([Bind("BookId,UserId,DueDate,Notes")] Transaction transaction)
        {
            // Remove navigation property validation errors
            ModelState.Remove("Book");
            ModelState.Remove("User");

            // Immediately add debug information to ViewBag
            ViewBag.Debug = new List<string>
            {
                $"Transaction received: BookId={transaction.BookId}, UserId={transaction.UserId}, DueDate={transaction.DueDate}"
            };

            try
            {
                // Check if model is valid
                if (!ModelState.IsValid)
                {
                    ViewBag.Debug.Add("ModelState is invalid after removing navigation property validations:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        ViewBag.Debug.Add("- " + error.ErrorMessage);
                    }
                }
                else
                {
                    ViewBag.Debug.Add("ModelState is valid, proceeding...");

                    // Check if BookId is valid
                    var book = await _bookRepository.GetBookByIdAsync(transaction.BookId);
                    if (book == null)
                    {
                        ViewBag.Debug.Add($"Book with ID {transaction.BookId} not found");
                        ModelState.AddModelError("BookId", "Book not found");
                    }
                    else if (!book.IsAvailable || book.Quantity <= 0)
                    {
                        ViewBag.Debug.Add($"Book '{book.Title}' is not available. IsAvailable={book.IsAvailable}, Quantity={book.Quantity}");
                        ModelState.AddModelError("BookId", "Book is not available for borrowing");
                    }
                    else
                    {
                        ViewBag.Debug.Add($"Book '{book.Title}' is available");

                        // Check if UserId is valid
                        var user = await _userRepository.GetUserByIdAsync(transaction.UserId);
                        if (user == null)
                        {
                            ViewBag.Debug.Add($"User with ID {transaction.UserId} not found");
                            ModelState.AddModelError("UserId", "User not found");
                        }
                        else
                        {
                            ViewBag.Debug.Add($"User '{user.FirstName} {user.LastName}' is valid");

                            // Set default values
                            transaction.BorrowDate = DateTime.Now;
                            transaction.IsReturned = false;
                            transaction.ReturnDate = null;

                            ViewBag.Debug.Add("Attempting to call BorrowBookAsync...");

                            try
                            {
                                // Borrow the book
                                var result = await _transactionRepository.BorrowBookAsync(transaction);
                                ViewBag.Debug.Add("BorrowBookAsync completed successfully");
                                ViewBag.Debug.Add($"Transaction ID: {result.Id}");
                                return RedirectToAction(nameof(Index));
                            }
                            catch (Exception innerEx)
                            {
                                ViewBag.Debug.Add($"Error in BorrowBookAsync: {innerEx.Message}");
                                ViewBag.Debug.Add($"Stack trace: {innerEx.StackTrace}");
                                ModelState.AddModelError(string.Empty, $"Error during book borrowing: {innerEx.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Debug.Add($"Outer exception: {ex.Message}");
                ViewBag.Debug.Add($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
            }

            // If we get here, something failed, redisplay form
            var booksForView = await _bookRepository.GetAllBooksAsync();
            var usersForView = await _userRepository.GetAllUsersAsync();
            ViewBag.Books = new SelectList(booksForView, "Id", "Title");
            ViewBag.Users = new SelectList(usersForView, "Id", "Email");

            // Return view with debug information
            return View(transaction);
        }

        // GET: Transaction/Return/5
        public async Task<IActionResult> Return(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            // Check if book is already returned
            if (transaction.IsReturned)
            {
                return RedirectToAction(nameof(Index));
            }

            // Return the book
            transaction = await _transactionRepository.ReturnBookAsync(id);

            // Check if book is overdue, if so, create a fine
            if (transaction.DueDate < DateTime.Now)
            {
                // Calculate days overdue
                int daysOverdue = (DateTime.Now - transaction.DueDate).Days;

                // Create a fine (for example, $1 per day overdue)
                var fine = new Fine
                {
                    TransactionId = transaction.Id,
                    UserId = transaction.UserId,
                    Amount = daysOverdue * 1.0m, // $1 per day
                    Reason = $"Book returned {daysOverdue} days late"
                };

                await _fineRepository.AddFineAsync(fine);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Transaction/UserTransactions/5
        public async Task<IActionResult> UserTransactions(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(id);
            ViewBag.UserName = $"{user.FirstName} {user.LastName}";

            return View(transactions);
        }

        // GET: Transaction/Overdue
        public async Task<IActionResult> Overdue()
        {
            var overdueTransactions = await _transactionRepository.GetOverdueTransactionsAsync();
            return View(overdueTransactions);
        }
    }
}