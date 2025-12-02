using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public int UserId { get; set; }

    public string? AdminFirstName { get; set; }

    public string? AdminLastName { get; set; }

    public string? AdminPhoneNumber { get; set; }

    public string? AdminPosition { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual ICollection<Highlight> Highlights { get; set; } = new List<Highlight>();

    public virtual User User { get; set; } = null!;
}
