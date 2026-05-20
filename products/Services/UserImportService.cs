using ClosedXML.Excel;
using products.Models;

namespace products.Services;

public class UserImportService
{
    private readonly AppDbContext _db = new();

    public void Import(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        
        var worksheet = workbook.Worksheet(1);
        
        var rows =  worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            var roleName = row.Cell(1).GetString();
         
            var fullName = row.Cell(2).GetString();
            
            var login = row.Cell(3).GetString();
            
            var password = row.Cell(4).GetString();
            
            var role = this.GetOrCreateRole(roleName);

            if(_db.Users.Any(x => x.Login == login)) 
                continue;
            
            var user = new User
            {
                FullName = fullName,
                Login = login,
                Password = password,
                RoleId = role.Id
            };
            
            _db.Users.Add(user);
        }
        
        _db.SaveChanges();
    }

    private Role GetOrCreateRole(string name)
    {
        var role = _db.Roles.FirstOrDefault(x => x.Name == name);

        if (role != null)
        {
            return role;
        }

        role = new Role
        {
            Name = name,
        };

        _db.Roles.Add(role);
        _db.SaveChanges();
        return role;
    }
}