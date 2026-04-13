namespace GreenFieldLocalHub.Models
{
    public class Basket
    {
        public int BasketId { get; set; } //The Primary key
        public bool Status { get; set; }
        public DateTime BasketCreatedAt { get; set; } = DateTime.UtcNow; //Makes the time of the basket created at on what time it is currently 
        public string UserId { get; set; } //This links each basket to a user
        public ICollection<BasketProducts>? BasketProducts { get; set; } //The navigation property, One basket can have many BasketProducts
    }
}
