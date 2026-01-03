namespace MOGDEOW.DTOs
{
    public class CartDetailDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public bool IsOutOfStock { get; set; } = false;
        public int MaxAvailable { get; set; }

    }
}
