using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int BookingId { get; set; }

    public string? TicketCode { get; set; }

    public string? TicketStatus { get; set; }

    public string? TicketType { get; set; }

    public string? TicketSeatNumber { get; set; }

    public DateTime? IssuedAt { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
