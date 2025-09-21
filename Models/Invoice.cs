using System;
using System.Collections.Generic;

namespace EcommerceBackend.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public decimal Total { get; set; }
        public decimal Tax { get; set; } = 0;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public string? BillingAddress { get; set; }

        public ICollection<InvoiceItem>? Items { get; set; }
    }
}
