using System;
using System.Collections.Generic;

namespace MyPlan.Models;

public partial class FriendInvitation
{
    public int InvitationId { get; set; }

    public int VisitorId { get; set; }

    public string? InvitationReceiverEmail { get; set; }

    public string? InvitationStatus { get; set; }

    public string? InvitationToken { get; set; }

    public DateTime? InvitationSentAt { get; set; }

    public DateTime? AddedAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public int? ReceiverId { get; set; }

    public int? ExperienceDetailId { get; set; }

    public virtual ExperienceDetail? ExperienceDetail { get; set; }

    public virtual Visitor? Receiver { get; set; }

    public virtual Visitor Visitor { get; set; } = null!;
}
