using Microsoft.EntityFrameworkCore;

namespace MyPlan.Data
{
    // Extend the scaffolded DbContext with OnModelCreatingPartial
    public partial class MyTripDbContext
    {
        // This method is called from the scaffolded OnModelCreating
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // Configure the Image-Experience relationship
            modelBuilder.Entity<Models.Image>(entity =>
            {
                entity.HasOne(d => d.Experience)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.ExperienceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Image_Experience");
            });

            // You can add other custom configurations here
        }
    }
}
