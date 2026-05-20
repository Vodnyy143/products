using System.Configuration;
using System.Data;
using System.Windows;
using products.Models;
using products.Services;

namespace products;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        using var database = new AppDbContext();
        database.Database.EnsureCreated();
        
        var userImport = new UserImportService();
        userImport.Import("Resources/user_import.xlsx");
        
        var pickupPointImport = new PickupPointService();
        pickupPointImport.Import("Resources/pick.xlsx");
        
        var productImport = new ProductImportService();
        productImport.Import("Resources/Tovar.xlsx");
    }
}