using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Visitor
{
    public int VisitorId { get; set; }

    public int UserId { get; set; }

    public string? VisitorFirstName { get; set; }

    public string? VisitorLastName { get; set; }

    public string? VisitorPhoneNumber { get; set; }

    public DateTime? AddedAt { get; set; }

    public string? Bio { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<FriendInvitation> FriendInvitationReceivers { get; set; } = new List<FriendInvitation>();

    public virtual ICollection<FriendInvitation> FriendInvitationVisitors { get; set; } = new List<FriendInvitation>();

    public virtual ICollection<Highlight> Highlights { get; set; } = new List<Highlight>();

    public virtual ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User User { get; set; } = null!;
}
