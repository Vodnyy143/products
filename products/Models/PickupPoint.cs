namespace products.Models;

public class PickupPoint
{
    public int Id { get; set; }
    public string Address { get; set; }
    
    public ICollection<Order>? Orders { get; set; } =  new List<Order>();
}