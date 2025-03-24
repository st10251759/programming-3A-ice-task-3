using System.Collections.Generic;
using System.Threading.Tasks;
using Ice_Task_3.Models;

namespace Ice_Task_3.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction> GetTransactionByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId);
        Task<IEnumerable<Transaction>> GetTransactionsByBookIdAsync(int bookId);
        Task<IEnumerable<Transaction>> GetOverdueTransactionsAsync();
        Task<Transaction> BorrowBookAsync(Transaction transaction);
        Task<Transaction> ReturnBookAsync(int transactionId);
        Task<Transaction> UpdateTransactionAsync(Transaction transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task<bool> TransactionExistsAsync(int id);
    }
}