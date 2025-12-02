using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int ExperienceId { get; set; }

    public int VisitorId { get; set; }

    public DateOnly BookingDate { get; set; }

    public int? BookingNumberOfTicket { get; set; }

    public decimal? BookingPricePerTicket { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? BookingStatus { get; set; }

    public string? BookingDescription { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Experience Experience { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual Visitor Visitor { get; set; } = null!;
}
