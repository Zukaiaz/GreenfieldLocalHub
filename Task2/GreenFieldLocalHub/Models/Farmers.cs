namespace GreenFieldLocalHub.Models
{
    public class Farmers
    {
        public int FarmersId { get; set; } //Primary Key
        public string UserId { get; set; } //Foreign Key, links user to farmers
        public string FarmerName { get; set; }
        public string FarmerEmail { get; set; }
        public string FarmingMethod { get; set; }
        public string FarmerInfo { get; set; }
        public string? ImagePath { get; set; }
        public ICollection<Products>? Products { get; set; } //One farmer for Many products
    }
}
