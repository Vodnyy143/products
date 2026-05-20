namespace products.Models;

public class User
{
    public int Id  { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
    
    public Role Role { get; set; } = null!;
}