namespace GreenFieldLocalHub.Models
{
    public class LoyaltyTransactions
    {
        public int LoyaltyTransactionsId { get; set; } //PK
        public int LoyaltyAccountId { get; set; } //FK to LoyaltyAccount
        public int OrdersId { get; set; } //FK to Orders
        public int PointsChange { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public LoyaltyAccount LoyaltyAccount { get; set; } //Navigation
        public Orders Orders { get; set; } //Navigation
    }
}
