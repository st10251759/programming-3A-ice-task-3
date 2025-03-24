using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ice_Task_3.Models;
using Ice_Task_3.Interfaces;

namespace Ice_Task_3.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            return View(books);
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Author,ISBN,Publisher,PublicationYear,Description,Quantity,Category")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.IsAvailable = book.Quantity > 0;
                await _bookRepository.AddBookAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,ISBN,Publisher,PublicationYear,Description,Quantity,IsAvailable,Category")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                book.IsAvailable = book.Quantity > 0;
                await _bookRepository.UpdateBookAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookRepository.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Book/Search
        public IActionResult Search()
        {
            return View();
        }

        // POST: Book/Search
        [HttpPost]
        public async Task<IActionResult> Search(string searchString, string searchBy)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return View();
            }

            IEnumerable<Book> books;
            if (searchBy == "title")
            {
                books = await _bookRepository.SearchBooksByTitleAsync(searchString);
            }
            else
            {
                books = await _bookRepository.SearchBooksByAuthorAsync(searchString);
            }

            return View("SearchResults", books);
        }
    }
}