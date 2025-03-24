using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ice_Task_3.Models
{
    public class Fine
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public Transaction Transaction { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }

        public DateTime FineDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }
    }
}