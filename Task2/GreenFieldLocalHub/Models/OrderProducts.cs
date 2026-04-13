namespace GreenFieldLocalHub.Models
{
    public class OrderProducts
    {
        public int OrderProductsId { get; set; } //Primary Key
        public int OrdersId { get; set; } //Foreign Key connecting Orders table to OrderProducts table
        public int ProductsId { get; set; } //Foreign key connecting Products table to OrderProducts table
        public int ProductsQuantity { get; set; }
        public Products Products { get; set; } //Many OrderProducts for One Products
        public Orders Orders { get; set; } //Many OrderProducts for One Orders

    }
}
