using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public DateOnly? InvoiceDate { get; set; }

    public string? InvoiceVisitorAddress { get; set; }

    public decimal? InvoiceTotalAmount { get; set; }

    public decimal? TaxAmount { get; set; }

    public DateTime? AddedAt { get; set; }

    public int? PaymentId { get; set; }
}
