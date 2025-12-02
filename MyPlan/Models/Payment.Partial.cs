namespace MyPlan.Models
{
    public partial class Payment
    {
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
