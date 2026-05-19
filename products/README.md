# 📋 План подготовки к Демонстрационному Экзамену
## КОД 09.02.07-2-2026 | WPF + MVVM + PostgreSQL + EF Core + CommunityToolkit.Mvvm

---

## 🗄️ СТРУКТУРА БАЗЫ ДАННЫХ (изучи первой)

На основе данных из приложений, вот полная схема БД в **3НФ**:

### Таблицы:

```sql
-- 1. Роли
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL  -- 'Администратор', 'Менеджер', 'Авторизированный клиент'
);

-- 2. Пользователи
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(150) NOT NULL,
    login VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    role_id INT NOT NULL REFERENCES roles(id)
);

-- 3. Категории товаров
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

-- 4. Производители
CREATE TABLE manufacturers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

-- 5. Поставщики
CREATE TABLE suppliers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

-- 6. Единицы измерения
CREATE TABLE units (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL  -- 'шт.'
);

-- 7. Товары
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    article VARCHAR(20) NOT NULL UNIQUE,  -- артикул: А112Т4
    name VARCHAR(150) NOT NULL,
    unit_id INT REFERENCES units(id),
    price DECIMAL(10,2) NOT NULL CHECK (price >= 0),
    supplier_id INT REFERENCES suppliers(id),
    manufacturer_id INT REFERENCES manufacturers(id),
    category_id INT REFERENCES categories(id),
    discount INT DEFAULT 0,
    quantity INT DEFAULT 0 CHECK (quantity >= 0),
    description TEXT,
    photo VARCHAR(300)
);

-- 8. Пункты выдачи
CREATE TABLE pickup_points (
    id SERIAL PRIMARY KEY,
    address VARCHAR(200) NOT NULL
);

-- 9. Статусы заказов
CREATE TABLE order_statuses (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL  -- 'Новый', 'Завершен'
);

-- 10. Заказы
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    article VARCHAR(50) NOT NULL,         -- артикул заказа
    order_date DATE NOT NULL,
    delivery_date DATE,
    pickup_point_id INT REFERENCES pickup_points(id),
    client_full_name VARCHAR(150),
    code VARCHAR(10),
    status_id INT REFERENCES order_statuses(id)
);

-- 11. Состав заказа (товары в заказе - для проверки при удалении)
CREATE TABLE order_products (
    id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    product_id INT NOT NULL REFERENCES products(id),
    quantity INT NOT NULL DEFAULT 1
);
```

> **Важно для Модуля 4:** Товар нельзя удалить, если он присутствует в заказе — проверяй через `order_products`.

---

## 📁 СТРУКТУРА ПРОЕКТА WPF (рекомендуемая)

```
ShoeStoreApp/
├── Models/               # EF Core сущности
│   ├── AppDbContext.cs
│   ├── Product.cs
│   ├── User.cs
│   ├── Order.cs
│   └── ...
├── ViewModels/           # CommunityToolkit.Mvvm
│   ├── BaseViewModel.cs  (или ObservableObject)
│   ├── LoginViewModel.cs
│   ├── ProductListViewModel.cs
│   ├── ProductEditViewModel.cs
│   ├── OrderListViewModel.cs
│   └── OrderEditViewModel.cs
├── Views/
│   ├── LoginWindow.xaml
│   ├── GuestWindow.xaml
│   ├── ClientWindow.xaml
│   ├── ManagerWindow.xaml
│   ├── AdminWindow.xaml
│   ├── ProductEditWindow.xaml
│   └── OrderEditWindow.xaml
├── Resources/
│   ├── images/ (фото товаров)
│   ├── picture.png (заглушка)
│   ├── Icon.ico
│   └── Icon.png (логотип)
├── Helpers/
│   └── ImageHelper.cs
└── App.xaml
```

---

## ⚙️ НАСТРОЙКА EF CORE + POSTGRESQL

### Пакеты NuGet:
```
Npgsql.EntityFrameworkCore.PostgreSQL
Microsoft.EntityFrameworkCore.Tools
CommunityToolkit.Mvvm
```

### AppDbContext:
```csharp
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<PickupPoint> PickupPoints { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=shoe_store;Username=postgres;Password=ваш_пароль");
    }
}
```

---

## 🎨 СТИЛЬ ПРИЛОЖЕНИЯ (из руководства по стилю)

| Элемент | Значение |
|---|---|
| Шрифт | Times New Roman |
| Основной фон | `#FFFFFF` (белый) |
| Дополнительный фон | `#7FFF00` (салатовый) |
| Акцент (целевое действие) | `#00FA9A` (мятный) |
| Скидка > 15% — фон строки | `#2E8B57` (зелёный) |
| Нет на складе — фон строки | голубой (`LightBlue`) |

### App.xaml — базовые стили:
```xml
<Application.Resources>
    <Style TargetType="Window">
        <Setter Property="FontFamily" Value="Times New Roman"/>
        <Setter Property="Background" Value="#FFFFFF"/>
    </Style>
    <Style TargetType="Button">
        <Setter Property="Background" Value="#00FA9A"/>
        <Setter Property="FontFamily" Value="Times New Roman"/>
    </Style>
</Application.Resources>
```

---

## 🔑 ЛОГИКА АВТОРИЗАЦИИ

```csharp
// LoginViewModel.cs
[RelayCommand]
private async Task Login()
{
    var user = await _context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Login == Login && u.Password == Password);
    
    if (user == null)
    {
        ErrorMessage = "Неверный логин или пароль";
        return;
    }
    
    OpenWindowForRole(user);
}

private void OpenWindowForRole(User user)
{
    Window window = user.Role.Name switch
    {
        "Администратор" => new AdminWindow(user),
        "Менеджер"      => new ManagerWindow(user),
        _               => new ClientWindow(user)
    };
    window.Show();
    Application.Current.Windows[0].Close();
}

[RelayCommand]
private void ContinueAsGuest()
{
    new GuestWindow().Show();
    Application.Current.Windows[0].Close();
}
```

---

## 🖍️ ПОДСВЕТКА СТРОК ТОВАРОВ (DataGrid)

Используй `DataTemplateSelector` или `Style` с триггерами:

```xml
<!-- В ProductListWindow.xaml -->
<DataGrid.RowStyle>
    <Style TargetType="DataGridRow">
        <Style.Triggers>
            <!-- Нет на складе — голубой -->
            <DataTrigger Binding="{Binding Quantity}" Value="0">
                <Setter Property="Background" Value="LightBlue"/>
            </DataTrigger>
        </Style.Triggers>
    </Style>
</DataGrid.RowStyle>
```

Для скидки > 15% нужен IValueConverter или MultiDataTrigger:
```csharp
public class DiscountColorConverter : IValueConverter
{
    public object Convert(object value, ...) =>
        value is int d && d > 15 
            ? new SolidColorBrush(Color.FromRgb(0x2E, 0x8B, 0x57)) 
            : Brushes.White;
}
```

---

## 📸 РАБОТА С ФОТО ТОВАРОВ

```csharp
// ImageHelper.cs
public static class ImageHelper
{
    private static readonly string ImageFolder = 
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

    public static string SaveImage(string sourcePath)
    {
        if (!Directory.Exists(ImageFolder))
            Directory.CreateDirectory(ImageFolder);

        // Ресайз до 300x200
        using var img = System.Drawing.Image.FromFile(sourcePath);
        using var resized = new System.Drawing.Bitmap(img, 300, 200);
        var fileName = Guid.NewGuid() + Path.GetExtension(sourcePath);
        var destPath = Path.Combine(ImageFolder, fileName);
        resized.Save(destPath);
        return fileName; // Сохраняем только имя файла в БД
    }

    public static void DeleteImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;
        var path = Path.Combine(ImageFolder, fileName);
        if (File.Exists(path)) File.Delete(path);
    }
}
```

**В ViewModel при выборе фото:**
```csharp
[RelayCommand]
private void SelectPhoto()
{
    var dlg = new OpenFileDialog
    {
        Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
    };
    if (dlg.ShowDialog() == true)
    {
        // Удаляем старое
        if (!string.IsNullOrEmpty(Product.Photo))
            ImageHelper.DeleteImage(Product.Photo);
        Product.Photo = ImageHelper.SaveImage(dlg.FileName);
        OnPropertyChanged(nameof(PhotoPath));
    }
}

public string PhotoPath => string.IsNullOrEmpty(Product.Photo)
    ? "pack://application:,,,/Resources/picture.png"
    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", Product.Photo);
```

---

## 🔍 ПОИСК + ФИЛЬТРАЦИЯ + СОРТИРОВКА (реальное время)

```csharp
// ProductListViewModel.cs
public partial class ProductListViewModel : ObservableObject
{
    private ObservableCollection<Product> _allProducts = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private string _searchText = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private Supplier? _selectedSupplier;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredProducts))]
    private bool _sortAscending = true;

    public IEnumerable<Product> FilteredProducts
    {
        get
        {
            var query = _allProducts.AsEnumerable();

            // Фильтр по поставщику
            if (SelectedSupplier != null)
                query = query.Where(p => p.SupplierId == SelectedSupplier.Id);

            // Поиск по всем текстовым полям
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                query = query.Where(p =>
                    (p.Name?.ToLower().Contains(s) == true) ||
                    (p.Article?.ToLower().Contains(s) == true) ||
                    (p.Description?.ToLower().Contains(s) == true) ||
                    (p.Category?.Name?.ToLower().Contains(s) == true) ||
                    (p.Manufacturer?.Name?.ToLower().Contains(s) == true) ||
                    (p.Supplier?.Name?.ToLower().Contains(s) == true));
            }

            // Сортировка по количеству
            query = SortAscending
                ? query.OrderBy(p => p.Quantity)
                : query.OrderByDescending(p => p.Quantity);

            return query;
        }
    }
}
```

---

## ❌ УДАЛЕНИЕ ТОВАРА (проверка наличия в заказах)

```csharp
[RelayCommand]
private async Task DeleteProduct(Product product)
{
    // Проверка: товар в заказе?
    bool inOrder = await _context.OrderProducts
        .AnyAsync(op => op.ProductId == product.Id);

    if (inOrder)
    {
        MessageBox.Show(
            "Невозможно удалить товар, так как он присутствует в одном или нескольких заказах.",
            "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    var result = MessageBox.Show(
        $"Вы уверены, что хотите удалить товар \"{product.Name}\"? Это действие необратимо.",
        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

    if (result != MessageBoxResult.Yes) return;

    ImageHelper.DeleteImage(product.Photo);
    _context.Products.Remove(product);
    await _context.SaveChangesAsync();
    Products.Remove(product);
}
```

---

## 🔒 ОДИН ЭКРАН РЕДАКТИРОВАНИЯ

```csharp
// В AdminWindow.xaml.cs или ViewModel
private ProductEditWindow? _editWindow;

private void OpenEditProduct(Product product)
{
    if (_editWindow != null && _editWindow.IsVisible)
    {
        _editWindow.Activate();
        return; // Не открываем второй
    }
    _editWindow = new ProductEditWindow(product);
    _editWindow.Show();
}
```

---

## 📅 ПЛАН НА 10 ДНЕЙ

| День | Задача |
|---|---|
| **1** | Настройка проекта: WPF, NuGet пакеты, AppDbContext, миграции, скрипт БД. Создать все таблицы, заполнить данными из xlsx. Получить ER-диаграмму. |
| **2** | Модуль 2: LoginWindow + ViewModel. Окна для всех ролей (Гость, Клиент, Менеджер, Админ) — пустые, но с правильной навигацией. ФИО в правом верхнем углу. |
| **3** | Список товаров: DataGrid/ListView с фото, всеми полями, заглушкой. Подсветка строк (скидка >15%, нет на складе, перечёркнутая цена). |
| **4** | Отладка Модулей 1-2. Блок-схема алгоритма (ГОСТ 19.701-90) в PDF. Скриншоты в docx. |
| **5** | Модуль 3: Поиск + фильтрация по поставщику + сортировка по количеству в реальном времени. |
| **6** | Модуль 3: Форма добавления/редактирования товара. Загрузка/смена фото, ресайз, удаление старого. |
| **7** | Модуль 3: Удаление товара с проверкой. Обработка исключений (MessageBox с иконками). Комментарии в коде. Кнопка Назад везде. |
| **8** | Модуль 4: Список заказов (Артикул, Статус, Адрес, Дата заказа, Дата доставки). |
| **9** | Модуль 4: Форма добавления/редактирования заказа. Удаление заказа. Git репозиторий. |
| **10** | Финальная отладка всего. Проверить все роли. Подготовить исполняемые файлы. Пробный прогон по времени (4 часа). |

---

## ⏱️ ТАЙМИНГ НА ЭКЗАМЕНЕ (ГИА ДЭ ПУ = 4 часа)

| Модуль | Время |
|---|---|
| Модуль 1: БД + ER-диаграмма + импорт данных | 50 мин |
| Модуль 2: Алгоритм + авторизация + список товаров | 40 мин |
| Модуль 3: Интерфейс + CRUD товаров | 90 мин |
| Модуль 4: Заказы + Git | 60 мин |

---

## ✅ ЧЕКЛИСТ ПЕРЕД СДАЧЕЙ

### Модуль 1:
- [ ] БД в PostgreSQL, 3НФ, первичные и внешние ключи
- [ ] ER-диаграмма в PDF с атрибутами и связями
- [ ] Данные из xlsx импортированы
- [ ] Скрипт БД (.sql) создан

### Модуль 2:
- [ ] Блок-схема в PDF по ГОСТ 19.701-90
- [ ] Шрифт Times New Roman, цвета из руководства
- [ ] Иконка приложения установлена
- [ ] Логотип на главной форме
- [ ] Авторизация из БД работает
- [ ] Гость / Клиент / Менеджер / Администратор — разные окна
- [ ] Кнопка выхода на каждом окне
- [ ] ФИО в правом верхнем углу
- [ ] Список товаров из БД (фото, заглушка, все поля)
- [ ] Подсветка строк (скидка >15% = #2E8B57, нет на складе = голубой)
- [ ] Перечёркнутая цена красным + итоговая цена чёрным
- [ ] Скриншоты в .docx

### Модуль 3:
- [ ] Кнопка «Назад» / навигация между окнами
- [ ] Заголовки на всех окнах
- [ ] MessageBox с иконками (Error/Warning/Information)
- [ ] Поиск по всем текстовым полям в реальном времени
- [ ] Сортировка по количеству (↑↓)
- [ ] Фильтр по поставщику (первый элемент — «Все поставщики»)
- [ ] Поиск + фильтрация работают совместно
- [ ] Форма добавления/редактирования товара
- [ ] Поля: артикул, фото, название, категория (dropdown), описание, производитель (dropdown), поставщик, цена, ед.изм., кол-во, скидка
- [ ] ID не показывается при добавлении, readonly при редактировании
- [ ] Фото ресайз 300×200, хранится путь в БД
- [ ] Старое фото удаляется при замене
- [ ] Только одно окно редактирования одновременно
- [ ] Удаление товара: проверка наличия в заказах
- [ ] Обновление списка после CRUD
- [ ] Комментарии в неочевидных местах кода
- [ ] Стиль CamelCase для идентификаторов (C#)

### Модуль 4:
- [ ] Кнопка «Заказы» у Менеджера и Администратора
- [ ] Список заказов: артикул, статус, адрес выдачи, дата заказа, дата доставки
- [ ] Форма заказа: артикул, статус (dropdown), адрес выдачи, дата заказа, дата выдачи
- [ ] Только Администратор может добавлять/редактировать/удалять
- [ ] Обновление списка после CRUD
- [ ] Git репозиторий с исходниками, исполняемыми файлами, скриптом БД

---

## 🧠 ЧАСТЫЕ ОШИБКИ И КАК ИХ ИЗБЕЖАТЬ

1. **Забыл про заглушку** — всегда проверяй `photo == null || !File.Exists(...)` → показывай `picture.png`
2. **Скидка > 15% (строго больше)** — проверяй `discount > 15`, не `>= 15`
3. **Цена отрицательная** — добавь валидацию `price >= 0`
4. **Количество отрицательное** — добавь валидацию `quantity >= 0`
5. **Два окна редактирования** — храни ссылку на окно, проверяй `IsVisible`
6. **Сортировка сбрасывается при фильтрации** — используй единый `FilteredProducts` с сохранением параметра сортировки
7. **Дата 30.02.2025 в данных** — в Заказ_import.xlsx есть невалидная дата, обработай при импорте
8. **Не обновляется список** — после любого CRUD вызывай `OnPropertyChanged(nameof(FilteredProducts))`
9. **Пароли в открытом виде** — в задании так и есть, не усложняй
10. **Не передаёшь пользователя между окнами** — передавай `User` в конструктор каждого окна