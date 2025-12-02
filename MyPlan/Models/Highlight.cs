using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Highlight
{
    public int HighlightId { get; set; }

    public string? HighlightTitle { get; set; }

    public string? HighlightContent { get; set; }

    public string? HighlightImage { get; set; }

    public string? HighlightDescription { get; set; }

    public DateTime? HighlightTime { get; set; }

    public DateTime? AddedAt { get; set; }

    public int? AdminId { get; set; }

    public int? VisitorId { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Visitor? Visitor { get; set; }
}
