using System;
using System.ComponentModel.DataAnnotations;

namespace Ice_Task_3.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [StringLength(20)]
        public string ISBN { get; set; }

        [StringLength(50)]
        public string Publisher { get; set; }

        public int PublicationYear { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        public bool IsAvailable { get; set; }

        [StringLength(100)]
        public string Category { get; set; }
    }
}