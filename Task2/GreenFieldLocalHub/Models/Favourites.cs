namespace GreenFieldLocalHub.Models
{
    public class Favourites
    {
        public int FavouritesId { get; set; } //Primary key
        public string UserId { get; set; } //Foreign key linking users to favourites
        public int ProductsId { get; set; } //Foreign key linking products to favourites

        public Products Products { get; set; } //Many Favourites for one product
    }
}
