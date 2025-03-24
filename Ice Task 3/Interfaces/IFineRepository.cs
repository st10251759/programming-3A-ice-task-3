using System.Collections.Generic;
using System.Threading.Tasks;
using Ice_Task_3.Models;

namespace Ice_Task_3.Interfaces
{
    public interface IFineRepository
    {
        Task<IEnumerable<Fine>> GetAllFinesAsync();
        Task<Fine> GetFineByIdAsync(int id);
        Task<IEnumerable<Fine>> GetFinesByUserIdAsync(int userId);
        Task<IEnumerable<Fine>> GetFinesByTransactionIdAsync(int transactionId);
        Task<IEnumerable<Fine>> GetUnpaidFinesAsync();
        Task<Fine> AddFineAsync(Fine fine);
        Task<Fine> PayFineAsync(int fineId);
        Task<Fine> UpdateFineAsync(Fine fine);
        Task<bool> DeleteFineAsync(int id);
        Task<bool> FineExistsAsync(int id);
    }
}