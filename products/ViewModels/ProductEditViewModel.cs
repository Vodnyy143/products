using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using products.Models;

namespace products.ViewModels;

public partial class ProductEditViewModel: ObservableObject
{
   private readonly AppDbContext _db = new();
    private readonly bool _isNew;
    private readonly string? _originalPhoto;  // для отката, если отменили

    [ObservableProperty] private Product _product = null!;
    [ObservableProperty] private string _windowTitle = "";

    // Списки для ComboBox
    public List<Category> Categories { get; }
    public List<Manufacturer> Manufacturers { get; }
    public List<Supplier> Suppliers { get; }
    public List<Unit> Units { get; }

    // Видимость поля ID (скрыто при добавлении)
    public Visibility IdVisibility => _isNew ? Visibility.Collapsed : Visibility.Visible;

    // Превью фото для UI
    public string PhotoPreview
    {
        get
        {
            if (string.IsNullOrEmpty(Product.Photo))
                return "pack://application:,,,/Resources/picture.png";

            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "images", Product.Photo);
            return File.Exists(path) ? path
                : "pack://application:,,,/Resources/picture.png";
        }
    }

    public ProductEditViewModel(Product? existing)
    {
        Categories = _db.Categories.ToList();
        Manufacturers = _db.Manufacturers.ToList();
        Suppliers = _db.Suppliers.ToList();
        Units = _db.Units.ToList();

        if (existing == null)
        {
            _isNew = true;
            // ID не отображается, вычислится автоматически
            Product = new Product
            {
                CategoryId = Categories.FirstOrDefault()?.Id,
                ManufacturerId = Manufacturers.FirstOrDefault()?.Id,
                SupplierId = Suppliers.FirstOrDefault()?.Id,
                UnitId = Units.FirstOrDefault()?.Id
            };
            WindowTitle = "Добавление товара";
        }
        else
        {
            _isNew = false;
            // Загружаем из БД для возможности отмены
            Product = _db.Products.First(p => p.Id == existing.Id);
            _originalPhoto = Product.Photo;
            WindowTitle = $"Редактирование товара: {Product.Name}";
        }
    }

    // === Выбор фото ===
    [RelayCommand]
    private void PickPhoto()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Выберите изображение товара",
            Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
        };

        if (dlg.ShowDialog() != true) return;

        try
        {
            OnPropertyChanged(nameof(PhotoPreview));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // === Сохранение ===
    [RelayCommand]
    private async Task Save(Window window)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(Product.Name))
        {
            MessageBox.Show("Введите наименование товара",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(Product.Article))
        {
            MessageBox.Show("Введите артикул товара",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Product.Price < 0)
        {
            MessageBox.Show("Цена не может быть отрицательной",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Product.Quantity < 0)
        {
            MessageBox.Show("Количество не может быть отрицательным",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Product.Discount < 0 || Product.Discount > 100)
        {
            MessageBox.Show("Скидка должна быть от 0 до 100",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            if (_isNew)
            {
                // ID вычислится EF Core автоматически (SERIAL в Postgres)
                _db.Products.Add(Product);
            }
            // При редактировании Product уже отслеживается контекстом

            await _db.SaveChangesAsync();
            window.DialogResult = true;
            window.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // === Отмена ===
    [RelayCommand]
    private void Cancel(Window window)
    {
        // При отмене редактирования — откатываем смену фото
        if (!_isNew && Product.Photo != _originalPhoto)
        {
            Product.Photo = _originalPhoto;
        }

        window.DialogResult = false;
        window.Close();
    }


}