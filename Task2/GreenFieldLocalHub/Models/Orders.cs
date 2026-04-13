namespace GreenFieldLocalHub.Models
{
    public class Orders
    {
        public int OrdersId { get; set; } //Primary Key
        public string UserId { get; set; } //Foreign Key linking Users to orders
        public decimal TotalAmount { get; set; }
        public bool Delivery { get; set; }
        public bool Collection { get; set; }
        public string? DeliveryType { get; set; } //Nullable as user may choose collection and not Delivery 
        public string OrderTrackingStatus { get; set; }
        public DateOnly? CollectionDate { get; set; } //Nullable as user may pick Delivery over Collection 
        public DateOnly OrderDate { get; set; }

        public ICollection<OrderProducts>? OrderProducts { get; set; } //Many Orders for one OrderProducts
        public ICollection<LoyaltyTransactions>? LoyaltyTransactions { get; set; }
    }
}
