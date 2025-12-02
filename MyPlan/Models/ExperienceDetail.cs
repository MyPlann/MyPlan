using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class ExperienceDetail
{
    public int ExperienceDetailId { get; set; }

    public int ExperienceId { get; set; }

    public DateOnly? ExperienceDetailDate { get; set; }

    public TimeOnly? ExperienceDetailTime { get; set; }

    public decimal? ExperienceDetailPrice { get; set; }

    public string? ExperienceDetailStatus { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Experience Experience { get; set; } = null!;

    public virtual ICollection<FriendInvitation> FriendInvitations { get; set; } = new List<FriendInvitation>();
}
