namespace GreenFieldLocalHub.Models
{
    public class BasketProducts //Middle table between Basket and Products as One Basket can have MANY products, but one product can be in MANY baskets, and visual studio doesnt support many to many relationships
    {
        public int BasketProductsId { get; set; } //Primary Key
        public int BasketId { get; set; } //Foreign Key connecting the BasketId to BasketProducts table
        public int ProductsId { get; set; } //Foreign Key connecting the ProductsId to BasketProducts table
        public int ProductQuantity { get; set; } //The quantaty of the products in the basket
        public Products Products { get; set; } //Many BasketProducts for one Products
        public Basket Basket { get; set; } //Many BasketProducts for one Basket (s)
    }
}
