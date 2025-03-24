using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ice_Task_3.Data;
using Ice_Task_3.Interfaces;
using Ice_Task_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Ice_Task_3.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly LibraryContext _context;

        public TransactionRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Book)
                .Include(t => t.User)
                .ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Book)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Book)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByBookIdAsync(int bookId)
        {
            return await _context.Transactions
                .Include(t => t.User)
                .Where(t => t.BookId == bookId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetOverdueTransactionsAsync()
        {
            var today = DateTime.Today;
            return await _context.Transactions
                .Include(t => t.Book)
                .Include(t => t.User)
                .Where(t => !t.IsReturned && t.DueDate < today)
                .ToListAsync();
        }

        public async Task<Transaction> BorrowBookAsync(Transaction transaction)
        {
            // Set borrowing defaults
            transaction.BorrowDate = DateTime.Now;
            transaction.IsReturned = false;
            transaction.ReturnDate = null;

            // Add the transaction
            _context.Transactions.Add(transaction);

            // Update book availability
            var book = await _context.Books.FindAsync(transaction.BookId);
            if (book != null)
            {
                book.Quantity--;
                if (book.Quantity <= 0)
                {
                    book.IsAvailable = false;
                }
            }

            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> ReturnBookAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Book)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return null;

            // Update transaction details
            transaction.IsReturned = true;
            transaction.ReturnDate = DateTime.Now;

            // Update book availability
            var book = transaction.Book;
            book.Quantity++;
            book.IsAvailable = true;

            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransactionExistsAsync(int id)
        {
            return await _context.Transactions.AnyAsync(e => e.Id == id);
        }
    }
}