namespace GreenFieldLocalHub.Models
{
    public class Products
    {
        public int ProductsId { get; set; } //Primary Key
        public int FarmersId { get; set; } //Foreign Key that links farmers to orders
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int StockQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImagePath { get; set; } //To be able to seed image links using url later on
        public Farmers? Farmers { get; set; } //Many products for one farmer
        public ICollection<OrderProducts>? OrderProducts { get; set; } //One Product can have many order products
        public ICollection<BasketProducts>? BasketProducts { get; set; } //One Product can have many Basket Products

    }
}
