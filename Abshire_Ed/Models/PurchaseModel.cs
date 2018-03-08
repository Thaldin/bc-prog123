namespace Abshire_Ed.Models
{
    public class PurchaseModel
    {
        public PersonModel Person { get; set; }
        public ProductModel Product { get; set; }
        public int SalesId { get; set; }
    }
}
