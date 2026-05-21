using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using products.Models;
using products.Views;

namespace products.ViewModels;

public partial class LoginViewModel: ObservableObject
{
    private readonly AppDbContext _db = new(); 
    
    [ObservableProperty] private  string _login = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private  string _errorMessage = "";
    
    [RelayCommand]
    private async Task SignIn()
    {
        ErrorMessage = "";
        
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Заполните все поля";
            return;
        }
        
        var user = await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == Login && u.Password == Password);

        if (user == null)
        {
            ErrorMessage = "Неверный логин или пароль";
            return;
        }
        
        OpenWindowForRole(user);
    }

    [RelayCommand]
    private void ContinueAsGuest()
    {
        var guestWindow = new GuestWindow();
        guestWindow.Show();
        Application.Current.Windows[0]?.Close();
    }
    

    private void OpenWindowForRole(User user)
    {
        Window window = user.Role.Name switch
        {
            "Администратор" => new AdminWindow(user),
            "Менеджер" => new ManagerWindow(user),
            _ => new ClientWindow(user),
        };
        
        window.Show();
        Application.Current.Windows[0]?.Close();
    }

}