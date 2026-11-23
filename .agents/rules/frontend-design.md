---
trigger: model_decision
description: Nguyên tắc chung thiết kế giao diện Front-end React cho hệ thống AgriTrace OCOP
---

# Nguyên tắc thiết kế Front-end — AgriTrace OCOP

## 1. Bản sắc thị giác & Màu sắc (Nông sản sạch & Minh bạch)
- **Màu chủ đạo (Primary):** Xanh lá nông nghiệp (`#15803D` / `#166534`) — tạo cảm giác tin cậy, nông sản sạch.
- **Màu nhấn (Accent):** Vàng lúa chín (`#D97706`) — dùng cho huy hiệu **OCOP 3★ / 4★ / 5★** và nút hành động phụ.
- **Nền & Chữ:** Nền sáng ấm (`#FAFAF9` / `#FFFFFF`), chữ xám than (`#1C1917`), độ tương phản cao, dễ đọc.
- **Màu trạng thái:** Xanh (`Đã qua cách ly / Đã duyệt`), Vàng (`Chờ duyệt / Đang cách ly`), Đỏ (`Vi phạm / Hết hàng`).

## 2. Typography & Bố cục (Ưu tiên trải nghiệm)
- Font chữ hỗ trợ tốt tiếng Việt (`Be Vietnam Pro` hoặc `Inter`), phân cấp kích thước rõ ràng giữa Tiêu đề – Mô tả – Giá/Nhãn.
- Thiết kế **Mobile-first Responsive**, khoảng trắng thoáng (`padding`/`gap` rộng rãi), không nhồi nhét quá nhiều thông tin vào một thẻ sản phẩm.
- Dùng ảnh thật/minh họa nông sản rõ nét; **không** dùng dữ liệu giả vô nghĩa kiểu "Lorem ipsum".

## 3. Thành phần đặc thù của AgriTrace OCOP
- **Thẻ sản phẩm OCOP:** Luôn hiển thị đủ: Tên đặc sản, Tỉnh thành (63 tỉnh) + Vùng miền (Bắc/Trung/Nam), Hạng sao OCOP, Giá & Tồn kho, và nút **"Xem Tem QR"**.
- **Trang Truy xuất nguồn gốc QR (`/trace/:token`):** Thiết kế dạng **Timeline (Dòng thời gian)** minh bạch nhật ký canh tác đã chốt (`IsLocked`), làm nổi bật trạng thái **Đã đủ ngày cách ly thuốc (PHI)** và chữ ký xác thực tem.
- **Dashboard phân quyền:** Tách biệt giao diện cho `Cooperative` (HTX), `Moderator` (Nhân viên trực web) và `Admin` (Báo cáo thống kê).

## 4. Chuẩn kỹ thuật React + TypeScript
- TypeScript strict mode, **không dùng `any`**.
- Mọi lời gọi API đặt trong `src/api/`, có interceptor tự động gắn Bearer JWT và tự gọi `/api/auth/refresh` khi gặp `401`.
- Mọi màn hình gọi API bắt buộc xử lý đủ 3 trạng thái: **Loading (Skeleton/Spinner)**, **Error (hiển thị `detail` từ chuẩn `ProblemDetails`)**, và **Empty State**.