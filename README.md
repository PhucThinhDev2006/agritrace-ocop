@"
# AgriTrace OCOP (ĐT08 Mở rộng)
Hệ thống kinh doanh nông sản và đặc sản vùng miền chuẩn OCOP 3 miền (63 tỉnh thành) tích hợp truy xuất nguồn gốc bằng mã QR.

## Công nghệ
- **Backend:** C# / .NET 10, ASP.NET Core Web API (Clean Architecture 4 lớp), EF Core, Dapper, PostgreSQL 16, Identity + JWT + Argon2id
- **Frontend:** React, TypeScript, Vite, Tailwind CSS
- **Testing & DevOps:** xUnit, Moq, Coverlet, Docker, Docker Compose, GitHub Actions
"@ | Out-File -FilePath README.md -Encoding utf8