using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyPlan.Models;

namespace MyPlan.Data;

public partial class MyTripDbContext : DbContext
{
    public MyTripDbContext()
    {
    }

    public MyTripDbContext(DbContextOptions<MyTripDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Experience> Experiences { get; set; }

    public virtual DbSet<ExperienceDetail> ExperienceDetails { get; set; }

    public virtual DbSet<FriendInvitation> FriendInvitations { get; set; }

    public virtual DbSet<Highlight> Highlights { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Itinerary> Itineraries { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Visitor> Visitors { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__43AA4141A3BE3A07");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.AdminFirstName)
                .HasMaxLength(50)
                .HasColumnName("admin_first_name");
            entity.Property(e => e.AdminLastName)
                .HasMaxLength(50)
                .HasColumnName("admin_last_name");
            entity.Property(e => e.AdminPhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("admin_phone_number");
            entity.Property(e => e.AdminPosition)
                .HasMaxLength(50)
                .HasColumnName("admin_position");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Admins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admin__user_id__73BA3083");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__5DE3A5B1C357E41F");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.BookingDate).HasColumnName("booking_date");
            entity.Property(e => e.BookingDescription)
                .HasMaxLength(255)
                .HasColumnName("booking_description");
            entity.Property(e => e.BookingNumberOfTicket).HasColumnName("booking_number_of_ticket");
            entity.Property(e => e.BookingPricePerTicket)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("booking_price_per_ticket");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(20)
                .HasColumnName("booking_status");
            entity.Property(e => e.ExperienceId).HasColumnName("experience_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.VisitorId).HasColumnName("visitor_id");

            entity.HasOne(d => d.Experience).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ExperienceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__experie__74AE54BC");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__visitor__75A278F5");
        });

        modelBuilder.Entity<Experience>(entity =>
        {
            entity.HasKey(e => e.ExperienceId).HasName("PK__Experien__EB216AFC07A0C420");

            entity.ToTable("Experience");

            entity.Property(e => e.ExperienceId).HasColumnName("experience_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.ExperienceDescription).HasColumnName("experience_description");
            entity.Property(e => e.ExperienceEndDate).HasColumnName("experience_end_date");
            entity.Property(e => e.ExperienceLocation)
                .HasMaxLength(100)
                .HasColumnName("experience_location");
            entity.Property(e => e.ExperienceMaxPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("experience_max_price");
            entity.Property(e => e.ExperienceMinPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("experience_min_price");
            entity.Property(e => e.ExperienceStartDate).HasColumnName("experience_start_date");
            entity.Property(e => e.ExperienceTitle)
                .HasMaxLength(100)
                .HasColumnName("experience_title");
            entity.Property(e => e.ExperienceType)
                .HasMaxLength(50)
                .HasColumnName("experience_type");
            entity.Property(e => e.Lat)
                .HasColumnType("decimal(10, 8)")
                .HasColumnName("lat");
            entity.Property(e => e.Long)
                .HasColumnType("decimal(11, 8)")
                .HasColumnName("long");
            entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");
        });

        modelBuilder.Entity<ExperienceDetail>(entity =>
        {
            entity.HasKey(e => e.ExperienceDetailId).HasName("PK__Experien__06D6E53C473EDC81");

            entity.ToTable("ExperienceDetail");

            entity.Property(e => e.ExperienceDetailId).HasColumnName("experience_detail_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.ExperienceDetailDate).HasColumnName("experience_detail_date");
            entity.Property(e => e.ExperienceDetailPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("experience_detail_price");
            entity.Property(e => e.ExperienceDetailStatus)
                .HasMaxLength(50)
                .HasColumnName("experience_detail_status");
            entity.Property(e => e.ExperienceDetailTime).HasColumnName("experience_detail_time");
            entity.Property(e => e.ExperienceId).HasColumnName("experience_id");

            entity.HasOne(d => d.Experience).WithMany(p => p.ExperienceDetails)
                .HasForeignKey(d => d.ExperienceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Experienc__exper__76969D2E");
        });

        modelBuilder.Entity<FriendInvitation>(entity =>
        {
            entity.HasKey(e => e.InvitationId).HasName("PK__FriendIn__94B74D7C3F15AB9F");

            entity.ToTable("FriendInvitation");

            entity.HasIndex(e => e.InvitationToken, "UQ__FriendIn__5CE21AFFC6D1907C").IsUnique();

            entity.Property(e => e.InvitationId).HasColumnName("invitation_id");
            entity.Property(e => e.AcceptedAt)
                .HasColumnType("datetime")
                .HasColumnName("accepted_at");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.ExperienceDetailId).HasColumnName("experience_detail_id");
            entity.Property(e => e.InvitationReceiverEmail)
                .HasMaxLength(100)
                .HasColumnName("invitation_receiver_email");
            entity.Property(e => e.InvitationSentAt)
                .HasColumnType("datetime")
                .HasColumnName("invitation_sent_at");
            entity.Property(e => e.InvitationStatus)
                .HasMaxLength(50)
                .HasColumnName("invitation_status");
            entity.Property(e => e.InvitationToken)
                .HasMaxLength(255)
                .HasColumnName("invitation_token");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.VisitorId).HasColumnName("visitor_id");

            entity.HasOne(d => d.ExperienceDetail).WithMany(p => p.FriendInvitations)
                .HasForeignKey(d => d.ExperienceDetailId)
                .HasConstraintName("FK_FriendInvitation_ExperienceDetail");

            entity.HasOne(d => d.Receiver).WithMany(p => p.FriendInvitationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK_FriendInvitation_Receiver");

            entity.HasOne(d => d.Visitor).WithMany(p => p.FriendInvitationVisitors)
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FriendInv__visit__778AC167");
        });

        modelBuilder.Entity<Highlight>(entity =>
        {
            entity.HasKey(e => e.HighlightId).HasName("PK__Highligh__B622AD492598D8FD");

            entity.ToTable("Highlight");

            entity.Property(e => e.HighlightId).HasColumnName("highlight_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.HighlightContent).HasColumnName("highlight_content");
            entity.Property(e => e.HighlightDescription)
                .HasMaxLength(255)
                .HasColumnName("highlight_description");
            entity.Property(e => e.HighlightImage)
                .HasMaxLength(255)
                .HasColumnName("highlight_image");
            entity.Property(e => e.HighlightTime)
                .HasColumnType("datetime")
                .HasColumnName("highlight_time");
            entity.Property(e => e.HighlightTitle)
                .HasMaxLength(100)
                .HasColumnName("highlight_title");
            entity.Property(e => e.VisitorId).HasColumnName("visitor_id");

            entity.HasOne(d => d.Admin).WithMany(p => p.Highlights)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_Highlight_Admin");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Highlights)
                .HasForeignKey(d => d.VisitorId)
                .HasConstraintName("FK_Highlight_Visitor");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Image__DC9AC9550166523F");

            entity.ToTable("Image");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.ExperienceId).HasColumnName("experience_id");
            entity.Property(e => e.ImageAttachment)
                .HasMaxLength(255)
                .HasColumnName("image_attachment");
            entity.Property(e => e.ImageTime)
                .HasColumnType("datetime")
                .HasColumnName("image_time");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__F58DFD49039660FE");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.InvoiceDate).HasColumnName("invoice_date");
            entity.Property(e => e.InvoiceTotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("invoice_total_amount");
            entity.Property(e => e.InvoiceVisitorAddress)
                .HasMaxLength(255)
                .HasColumnName("invoice_visitor_address");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.TaxAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("tax_amount");
        });

        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity.HasKey(e => e.ItineraryId).HasName("PK__Itinerar__6E8B21D67D5BB04F");

            entity.ToTable("Itinerary");

            entity.Property(e => e.ItineraryId).HasColumnName("itinerary_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.ExperienceId).HasColumnName("experience_id");
            entity.Property(e => e.ItineraryDay).HasColumnName("itinerary_day");
            entity.Property(e => e.ItineraryDescription)
                .HasMaxLength(255)
                .HasColumnName("itinerary_description");
            entity.Property(e => e.ItineraryEndDate).HasColumnName("itinerary_end_date");
            entity.Property(e => e.ItineraryStartDate).HasColumnName("itinerary_start_date");
            entity.Property(e => e.VisitorId).HasColumnName("visitor_id");

            entity.HasOne(d => d.Experience).WithMany(p => p.Itineraries)
                .HasForeignKey(d => d.ExperienceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__exper__7C4F7684");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Itineraries)
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__visit__7D439ABD");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EAA3C69312");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("payment_amount");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasColumnName("payment_status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__booking__7E37BEF6");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__60883D90D6E93DC3");

            entity.ToTable("Review");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ExperienceId).HasColumnName("experience_id");
            entity.Property(e => e.ReviewComment)
                .HasMaxLength(255)
                .HasColumnName("review_comment");
            entity.Property(e => e.ReviewRating).HasColumnName("review_rating");
            entity.Property(e => e.ReviewTime)
                .HasColumnType("datetime")
                .HasColumnName("review_time");
            entity.Property(e => e.VisitorId).HasColumnName("visitor_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Review_Booking");

            entity.HasOne(d => d.Experience).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ExperienceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__experien__7F2BE32F");

            entity.HasOne(d => d.Visitor).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__visitor___00200768");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__D596F96BB3D442F5");

            entity.ToTable("Ticket");

            entity.HasIndex(e => e.TicketCode, "UQ__Ticket__628DB75FB46360D5").IsUnique();

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("issued_at");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(50)
                .HasColumnName("ticket_code");
            entity.Property(e => e.TicketSeatNumber)
                .HasMaxLength(20)
                .HasColumnName("ticket_seat_number");
            entity.Property(e => e.TicketStatus)
                .HasMaxLength(50)
                .HasColumnName("ticket_status");
            entity.Property(e => e.TicketType)
                .HasMaxLength(50)
                .HasColumnName("ticket_type");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__booking___02084FDA");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__B9BE370FBA72961D");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__AB6E6164109B0FE6").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasKey(e => e.VisitorId).HasName("PK__Visitor__87ED1B51C37287C1");

            entity.ToTable("Visitor");

            entity.Property(e => e.VisitorId).HasColumnName("visitor_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VisitorFirstName)
                .HasMaxLength(50)
                .HasColumnName("visitor_first_name");
            entity.Property(e => e.VisitorLastName)
                .HasMaxLength(50)
                .HasColumnName("visitor_last_name");
            entity.Property(e => e.VisitorPhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("visitor_phone_number");

            entity.HasOne(d => d.User).WithMany(p => p.Visitors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visitor__user_id__02FC7413");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
