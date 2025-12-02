using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Experience
{
    public int ExperienceId { get; set; }

    public string ExperienceTitle { get; set; } = null!;

    public string? ExperienceDescription { get; set; }

    public string? ExperienceType { get; set; }

    public string? ExperienceLocation { get; set; }

    public decimal? ExperienceMinPrice { get; set; }

    public decimal? ExperienceMaxPrice { get; set; }

    public DateOnly? ExperienceStartDate { get; set; }

    public DateOnly? ExperienceEndDate { get; set; }

    public int? MaxCapacity { get; set; }

    public DateTime? AddedAt { get; set; }

    public decimal? Lat { get; set; }

    public decimal? Long { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<ExperienceDetail> ExperienceDetails { get; set; } = new List<ExperienceDetail>();

    public virtual ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
