using System.Collections.Generic;
using System.Threading.Tasks;
using Ice_Task_3.Models;

namespace Ice_Task_3.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task<IEnumerable<Book>> SearchBooksByTitleAsync(string title);
        Task<IEnumerable<Book>> SearchBooksByAuthorAsync(string author);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(int id);
    }
}