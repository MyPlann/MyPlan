namespace MyPlan.ViewModels.HomeVMs
{
    public class ExperienceSummaryVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Price { get; set; } = string.Empty;
        public decimal? PriceNum { get; set; }
        public string Time { get; set; } = string.Empty; 
        public string Type { get; set; } = string.Empty;
        public int? MaxCapacity { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public RatingVM Rating { get; set; } = new RatingVM();
        public int ReviewCount { get; set; } 
    }

    public class RatingVM
    {
        public double Average { get; set; }
        public int Count { get; set; }
    }

    public class MapExperienceVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public bool IsFree { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }
}
