using Microsoft.EntityFrameworkCore;
using Ice_Task_3.Models;

namespace Ice_Task_3.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Fine> Fines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships if needed
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Book)
                .WithMany()
                .HasForeignKey(t => t.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.Transaction)
                .WithMany()
                .HasForeignKey(f => f.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add this line to specify precision for the Amount property
            modelBuilder.Entity<Fine>()
                .Property(f => f.Amount)
                .HasPrecision(10, 2);
        }
    }
}