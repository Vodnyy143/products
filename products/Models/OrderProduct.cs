namespace products.Models;

public class OrderProduct
{
    public int  Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}