using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public string? ImageAttachment { get; set; }

    public DateTime? ImageTime { get; set; }

    public DateTime? AddedAt { get; set; }

    public int? ExperienceId { get; set; }
}
