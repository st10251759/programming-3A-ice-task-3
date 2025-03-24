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
    public class FineRepository : IFineRepository
    {
        private readonly LibraryContext _context;

        public FineRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Fine>> GetAllFinesAsync()
        {
            return await _context.Fines
                .Include(f => f.Transaction)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<Fine> GetFineByIdAsync(int id)
        {
            return await _context.Fines
                .Include(f => f.Transaction)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Fine>> GetFinesByUserIdAsync(int userId)
        {
            return await _context.Fines
                .Include(f => f.Transaction)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Fine>> GetFinesByTransactionIdAsync(int transactionId)
        {
            return await _context.Fines
                .Include(f => f.User)
                .Where(f => f.TransactionId == transactionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Fine>> GetUnpaidFinesAsync()
        {
            return await _context.Fines
                .Include(f => f.Transaction)
                .Include(f => f.User)
                .Where(f => !f.IsPaid)
                .ToListAsync();
        }

        public async Task<Fine> AddFineAsync(Fine fine)
        {
            // Set default values
            fine.FineDate = DateTime.Now;
            fine.IsPaid = false;
            fine.PaymentDate = null;

            _context.Fines.Add(fine);
            await _context.SaveChangesAsync();
            return fine;
        }

        public async Task<Fine> PayFineAsync(int fineId)
        {
            var fine = await _context.Fines.FindAsync(fineId);
            if (fine == null)
                return null;

            fine.IsPaid = true;
            fine.PaymentDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return fine;
        }

        public async Task<Fine> UpdateFineAsync(Fine fine)
        {
            _context.Entry(fine).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return fine;
        }

        public async Task<bool> DeleteFineAsync(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null)
                return false;

            _context.Fines.Remove(fine);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FineExistsAsync(int id)
        {
            return await _context.Fines.AnyAsync(e => e.Id == id);
        }
    }
}