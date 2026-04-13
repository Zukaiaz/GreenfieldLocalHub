namespace GreenFieldLocalHub.Models
{
    public class LoyaltyAccount
    {
        public int LoyaltyAccountId { get; set; } //PK
        public string UserId { get; set; } //FK
        public int Points { get; set; }
        public string Tier { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        public ICollection<LoyaltyTransactions>? LoyaltyTransactions { get; set; } //Nav property 
    }
}
