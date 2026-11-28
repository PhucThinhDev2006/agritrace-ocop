# AgriTrace OCOP (ĐT08 Mở rộng)

> Hệ thống thương mại điện tử kinh doanh **nông sản và đặc sản vùng miền chuẩn OCOP 3 miền (63 tỉnh thành)** tích hợp **truy xuất nguồn gốc minh bạch bằng mã QR** (kiểm soát thời gian cách ly PHI, khóa bất biến lô thu hoạch và chữ ký HMAC-SHA256).

---

## Vai trò người dùng (4 Roles)
- **Buyer (Người mua):** Tìm kiếm/lọc đặc sản theo 3 miền, 63 tỉnh thành, chứng nhận (`OCOP`, `VietGAP`, `GlobalGAP`, `Organic`, `VSATTP`, `HACCP`), quét QR truy xuất nguồn gốc, đặt hàng và gửi báo cáo vi phạm.
- **Cooperative (Hợp tác xã / Nhà cung cấp):** Quản lý gian hàng HTX, đăng bán nông sản, tải chứng nhận chất lượng, ghi nhật ký canh tác (`FarmingLog`) và khóa lô thu hoạch phát hành mã QR (`HarvestBatch`).
- **Moderator (Kiểm duyệt viên):** Duyệt hoặc tạm ẩn sản phẩm vi phạm, xác minh giấy chứng nhận và xử lý báo cáo tố cáo (`ViolationReport`).
- **Admin (Quản trị viên):** Quản trị hệ thống, xác thực Hợp tác xã và phân quyền người dùng.

---

## Công nghệ sử dụng
- **Backend:** C# / .NET 11, ASP.NET Core Web API (**Clean Architecture 4 lớp**), Entity Framework Core (Npgsql), Dapper, PostgreSQL 16.
- **Frontend:** React, TypeScript, Vite.
- **Testing & DevOps:** xUnit, Coverlet, Docker, Docker Compose, GitHub Actions CI.

---

## Cấu trúc thư mục hiện tại

```text
agritrace-ocop/
├── .github/workflows/ci.yml        # CI tự động build & test
├── backend/
│   ├── AgriTrace.sln
│   ├── docker-compose.yml          # PostgreSQL 16 (container: agritrace-postgres)
│   ├── AgriTrace.Domain/           # Tầng lõi: Entities, Enums, Exceptions, Interfaces (không phụ thuộc hạ tầng)
│   │   ├── Entities/               # Region, Province, Cooperative, AgriProduct, ProductCertification,
│   │   │                           # HarvestBatch, FarmingLog, Order, OrderItem, ViolationReport
│   │   ├── Enums/                  # CertificationType, ProductStatus, OrderStatus, ReportStatus, ReportTargetType
│   │   ├── Exceptions/             # BusinessRuleException (400), NotFoundException (404), ConflictException (409)
│   │   └── Interfaces/             # IUnitOfWork & 6 Repository Interfaces
│   ├── AgriTrace.Application/      # Use Cases / Services / DTOs (sắp triển khai)
│   ├── AgriTrace.Infrastructure/   # EF Core DbContext, Fluent API Configurations (jsonb, xmin), DependencyInjection
│   ├── AgriTrace.API/              # ASP.NET Core Web API Entry Point & appsettings
│   └── AgriTrace.Tests/            # xUnit Tests (12/12 Domain Unit Tests Passed)
└── frontend/                       # React + TypeScript + Vite
```

---

## Tiến độ phát triển (Git Workflow)

- [x] `feat/project-skeleton` — Khởi tạo Clean Architecture 4 lớp, Frontend React+Vite, Docker Compose & CI.
- [x] `chore/setup-skills-spec` — Thiết lập tài liệu đặc tả & quy tắc giao diện.
- [x] `feat/domain-model` — Hoàn thiện 10 Entities, 5 Enums, 3 Domain Exceptions, `IUnitOfWork`, 6 Repository Interfaces và **12/12 xUnit Domain Tests** (PHI, khóa bất biến QR, tồn kho, hạn thanh toán 15 phút, báo cáo vi phạm).
- [ ] `feat/database-efcore` *(Đang thực hiện)*:
  - [x] Cài đặt `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `Dapper`.
  - [x] Tạo `AgriTraceDbContext` & 10 class Fluent API (`jsonb` cho `NutritionInfoJson`, `xmin` concurrency token trên `AgriProduct`, Unique Index cho `BatchCode` & `Order.IdempotencyKey`, các Index lọc).
  - [ ] Bật PostgreSQL 16 (Docker), sinh Migration `Init` & chạy `database update`.
  - [ ] Viết `SeedAsync` (3 Miền, 63 Tỉnh/Thành, 6–9 HTX đại diện, 15–25 đặc sản OCOP).
  - [ ] Cài đặt `EfUnitOfWork` & 6 `EfRepository`.
  - [ ] Viết Integration Test Repository với SQLite in-memory.
- [ ] `feat/auth-security` — Identity, Argon2id, JWT & phân quyền 4 vai trò.
- [ ] `feat/catalog-traceability-api` — API Danh mục đặc sản, Nhật ký canh tác & Truy xuất QR HMAC-SHA256.
- [ ] `feat/orders-moderation-reports` — API Đặt hàng, Kiểm duyệt & Báo cáo vi phạm.
- [ ] `feat/frontend-react` — Giao diện người dùng React + TypeScript.
- [ ] `chore/testing-docker-deploy` — Kiểm thử tổng thể, đóng gói Docker & triển khai.

---

## Hướng dẫn chạy cục bộ

### 1. Yêu cầu môi trường
- [.NET 11 SDK](https://dotnet.microsoft.com/)
- [Node.js](https://nodejs.org/) (v20+)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 2. Khởi chạy PostgreSQL 16 (Docker)
```bash
docker compose -f backend/docker-compose.yml up -d
```

### 3. Build & Chạy kiểm thử Backend
```bash
dotnet build backend/AgriTrace.sln
dotnet test backend/AgriTrace.sln
```
