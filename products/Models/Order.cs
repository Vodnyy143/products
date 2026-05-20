namespace products.Models;

public class Order
{
    public int  Id { get; set; }
    public string Article { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public int? PickupPointId { get; set; }
    public string? ClientFullName { get; set; }
    public string? Code { get; set; }
    public int StatusId { get; set; }
    
    public PickupPoint? PickupPoint { get; set; }
    public OrderStatus? Status { get; set; }
    public ICollection<OrderProduct>? OrderProducts { get; set; } = new List<OrderProduct>();
}