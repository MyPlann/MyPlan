namespace MyPlan.ViewModels.ExploreVMs
{
    public class ExploreIndexVM
    {
        public List<FeaturedEventVM> FeaturedEvents { get; set; } = new List<FeaturedEventVM>();
        public List<RecommendationVM> Recommendations { get; set; } = new List<RecommendationVM>();
        public List<CategoryVM> Categories { get; set; } = new List<CategoryVM>();
        public List<FriendHighlightVM> FriendsHighlights { get; set; } = new List<FriendHighlightVM>();
        public List<FriendVM> AllFriends { get; set; } = new List<FriendVM>();
        public string? SearchQuery { get; set; }
        public string? Category { get; set; }
        public string? Filter { get; set; }
    }

    public class FeaturedEventVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly? Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Price { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty; 
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class RecommendationVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly? Date { get; set; }
        public string? ImageUrl { get; set; }
        public string Time { get; set; } = string.Empty; 
        public string Type { get; set; } = string.Empty;
    }

    public class CategoryVM
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty; 
        public string Count { get; set; } = string.Empty;
    }

    public class FriendHighlightVM
    {
        public int Id { get; set; }
        public string FriendName { get; set; } = string.Empty;
        public string? FriendAvatar { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? EventImage { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public int Likes { get; set; } 
        public int ExperienceId { get; set; } 
    }

    public class FriendVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string Bio { get; set; } = string.Empty;
        public int EventsAttended { get; set; }
        public int FriendsCount { get; set; } 
    }
}
