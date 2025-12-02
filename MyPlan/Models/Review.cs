using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int VisitorId { get; set; }

    public int ExperienceId { get; set; }

    public int? ReviewRating { get; set; }

    public string? ReviewComment { get; set; }

    public DateTime? ReviewTime { get; set; }

    public DateTime? AddedAt { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Experience Experience { get; set; } = null!;

    public virtual Visitor Visitor { get; set; } = null!;
}
