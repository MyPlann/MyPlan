using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public decimal? PaymentAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
