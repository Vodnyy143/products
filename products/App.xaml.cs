using System.Configuration;
using System.Data;
using System.Windows;
using products.Models;

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
    }
}