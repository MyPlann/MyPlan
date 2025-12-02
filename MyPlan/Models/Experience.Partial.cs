namespace MyPlan.Models
{
    // This extends your scaffolded Experience class without modifying it
    public partial class Experience
    {
        // Add the missing Images navigation property
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
