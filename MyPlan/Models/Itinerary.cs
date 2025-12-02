using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Itinerary
{
    public int ItineraryId { get; set; }

    public int VisitorId { get; set; }

    public int ExperienceId { get; set; }

    public DateOnly? ItineraryStartDate { get; set; }

    public DateOnly? ItineraryEndDate { get; set; }

    public int? ItineraryDay { get; set; }

    public string? ItineraryDescription { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Experience Experience { get; set; } = null!;

    public virtual Visitor Visitor { get; set; } = null!;
}
