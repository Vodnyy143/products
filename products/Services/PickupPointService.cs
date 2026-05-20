using ClosedXML.Excel;
using products.Models;

namespace products.Services;

public class PickupPointService
{
    private readonly AppDbContext _db = new();

    public void Import(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        
        var worksheet = workbook.Worksheet(1);
        
        var rows =  worksheet.RowsUsed();

        foreach (var row in rows)
        {
            var address = row.Cell(1).GetString();
         
            if(_db.PickupPoints.Any(x => x.Address == address)) 
                continue;
            
            var pickupPoint = new PickupPoint
            {
                Address = address,
            };
            
            _db.PickupPoints.Add(pickupPoint);
        }
        
        _db.SaveChanges();
    }
}