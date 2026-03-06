# MarketFlow

Platform jual beli modern dengan fitur lengkap untuk buyer dan seller, menggunakan ASP.NET Core dan MySQL.

## ✨ Fitur

### 🛍️ Umum
- Landing page yang menarik dan modern
- Login & Register dengan role (Buyer/Seller)
- Dashboard interaktif
- Desain minimalis dan responsif

### 👤 Buyer (Pembeli)
- Jelajahi dan beli produk
- Lihat pesanan dengan status real-time
- Lihat bukti pengiriman (gambar)
- Download struk pembelian dalam format PDF

### 🏪 Seller (Penjual)
- Upload dan kelola produk
- Kelola pesanan dari buyer
- Upload bukti pengiriman (gambar)
- Download struk penjualan dalam format PDF

## 🛠️ Teknologi

- **ASP.NET Core 8.0** - Framework web
- **MySQL** - Database (menggunakan Pomelo.EntityFrameworkCore.MySql)
- **Entity Framework Core** - ORM
- **QuestPDF** - Generate PDF untuk struk
- **BCrypt** - Hashing password
- **Cookie Authentication** - Sistem autentikasi

## 📋 Setup

1. **Install MySQL** dan pastikan MySQL server berjalan

2. **Update connection string** di `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MarketplaceDB;User=root;Password=YOUR_PASSWORD;Port=3306;"
}
```

3. **Restore dependencies**:
```bash
dotnet restore
```

4. **Jalankan aplikasi**:
```bash
dotnet run
```

5. **Buka browser** di `https://localhost:5001` atau `http://localhost:5000`

## 📁 Struktur Project

```
Marketplace/
├── Controllers/
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── DashboardController.cs
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   └── ShopController.cs
├── Models/
│   ├── User.cs
│   ├── Product.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Views/
│   ├── Home/
│   ├── Account/
│   ├── Dashboard/
│   ├── Products/
│   ├── Orders/
│   └── Shop/
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   └── uploads/
│       ├── products/
│       └── shipping/
├── Program.cs
├── appsettings.json
└── Marketplace.csproj
```

## 🔐 Role & Akses

### Buyer
- Dapat melihat semua produk
- Dapat membeli produk
- Dapat melihat pesanan sendiri
- Dapat melihat bukti pengiriman
- Dapat download struk PDF

### Seller
- Dapat mengelola produk sendiri
- Dapat melihat pesanan untuk produknya
- Dapat update status pesanan
- Dapat upload bukti pengiriman
- Dapat download struk PDF

## 📝 Catatan

- Database akan otomatis dibuat saat aplikasi pertama kali dijalankan
- Password di-hash menggunakan BCrypt
- File upload (gambar produk & bukti pengiriman) disimpan di `wwwroot/uploads/`
- Struk PDF menggunakan QuestPDF library
- Pastikan folder `wwwroot/uploads/products/` dan `wwwroot/uploads/shipping/` ada untuk upload file

## 🎨 Desain

- Desain modern minimalis dengan gradient
- Responsive untuk mobile dan desktop
- Animasi smooth dan interaktif
- Color scheme yang menarik dan profesional

## Documentation

All associated resources are licensed under the [Read-Only Source License](./LICENSE.md).
