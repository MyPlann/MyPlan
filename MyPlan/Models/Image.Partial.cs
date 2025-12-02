namespace MyPlan.Models
{
    // This extends your scaffolded Image class
    public partial class Image
    {
        // Add the missing Experience navigation property
        public virtual Experience? Experience { get; set; }
    }
}
