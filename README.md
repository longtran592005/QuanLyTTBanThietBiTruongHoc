# Hệ Thống Quản Lý Cửa Hàng Thiết Bị Trường Học
**(School Device Store - Desktop Management System)**

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET Framework](https://img.shields.io/badge/.NET_Framework_4.8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![WinForms](https://img.shields.io/badge/Windows_Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)

Hệ thống quản lý điểm bán hàng và kho thiết bị trường học chuyên nghiệp được xây dựng trên nền tảng **C# Windows Forms**, tuân thủ nghiêm ngặt **Kiến trúc 3 lớp (3-Tier Architecture)** và mẫu thiết kế **Repository Pattern**. Dự án được tái thiết kế toàn diện UI/UX mang hơi hướng của các phần mềm SaaS hiện đại (Minimalism, Custom Custom Controls, Fluent Design).

---

## 🌟 Điểm Nổi Bật Của Dự Án

*   **Kiến trúc Enterprise-Ready:** Phân tách hoàn toàn các tầng `GUI`, `BLL` (Business Logic), `DAL` (Data Access), và `DTO` (Data Transfer Object).
*   **Thiết Kế UI/UX Cao Cấp:** Từ bỏ các giới hạn mặc định của WinForms, ứng dụng sử dụng bộ giao diện tự tùy chỉnh hoàn toàn với các Controls (`RoundedTextBox`, `KpiCardControl`, `LoadingOverlay`) và bộ màu Ceramic Blue tinh tế.
*   **Kiểm Soát Nhất Quán (Transactional Integrity):** Các quy trình phức tạp như *lập hóa đơn bán hàng* được bao bọc trong SQLite/SQL Transactions, đảm bảo trừ kho và lưu hóa đơn đồng bộ thời gian thực.
*   **Bảo mật:** Tham số hóa truy vấn SQL (Parameterized Queries) chống SQL Injection 100%, kết hợp băm (hashing) mật khẩu đăng nhập.
*   **Đồ thị & Báo cáo:** Tích hợp bộ tạo biểu đồ doanh thu động và khả năng trích xuất toàn bộ dữ liệu ra tệp CSV.

## 📂 Cấu Trúc Mã Nguồn

Dự án nằm ở giải pháp chính `SchoolDeviceStore/` với cấu trúc như sau:

```text
📦 SchoolDeviceStore
 ┣ 📂 BLL/              # Lớp xử lý nghiệp vụ (Business Logic)
 ┣ 📂 DAL/              # Lớp giao tiếp CSDL & Repository (Data Access)
 ┣ 📂 DTO/              # Lớp thực thể dữ liệu (Data Transfer Objects)
 ┣ 📂 GUI.WinForms/     # Giao diện người dùng (Presentation Layer)
 ┃ ┣ 📂 Controls/       # Các UI Component tự tạo (Custom Controls)
 ┃ ┣ 📂 Pages/          # Các phân hệ chức năng (Dashboard, Sales, Products...)
 ┃ ┣ 📂 Helpers/        # Chứa UITheme, UIHelper định hình phong cách
 ┃ ┗ 📜 MainForm.cs     # Shell điều hướng trung tâm
 ┣ 📂 Database/         # Chứa kịch bản SQL khởi tạo CSDL
 ┗ 📂 Docs/             # Tài liệu đặc tả và hướng dẫn
```

## 🚀 Các Phân Hệ Chức Năng Chính

1.  **Dashboard Tổng Quan:** Theo dõi KPI doanh thu 30 ngày, hóa đơn, sản phẩm hết hàng và biểu đồ dạng cột động.
2.  **Quản Lý Bán Hàng (POS):** Tìm kiếm khách hàng/sản phẩm Autocomplete, giỏ hàng tương tác, tính thuế VAT/Giảm giá, xuất và in hóa đơn (Print Preview).
3.  **Quản Lý Kho Hàng & Sản Phẩm:** Giao diện Split-screen hiện đại, kiểm tra tồn kho thời gian thực, cảnh báo cạn kho, xuất CSV.
4.  **Báo Cáo & Thống Kê:** Lọc doanh thu theo ngày, xác định Top các sản phẩm bán chạy nhất.
5.  **Sao Lưu Dữ Liệu:** Backup và Restore cơ sở dữ liệu (định dạng `.bak`) an toàn trực tiếp từ giao diện.
6.  **Xác Thực & Phân Quyền:** Quyền Quản trị viên (Admin) và Nhân viên (Staff), tự động ẩn các tính năng nhạy cảm với nhân viên.

## 📄 Tài Liệu Tham Khảo

Để xem chi tiết mô tả kỹ thuật, cách luồng dữ liệu hoạt động và thiết kế cơ sở dữ liệu, vui lòng đọc:
👉 **[Tài liệu Đặc Tả Chức Năng Chi Tiết (ChucNangChiTiet.md)](SchoolDeviceStore/Docs/ChucNangChiTiet.md)**

## 🛠 Hướng Dẫn Chạy Dự Án
1. Mở Solution `SchoolDeviceStore/SchoolDeviceStore.sln` bằng Visual Studio 2022.
2. Build solution để khôi phục các thư viện (nếu có).
3. Khởi chạy dự án `GUI.WinForms`.
4. *Tài khoản đăng nhập Demo mặc định:*
   *   **Username:** `admin`
   *   **Password:** `admin123`

---
*Dự án Lập Trình Nâng Cao (LTNC) - Đồ án thực hành Nghiên Cứu Khoa Học.*
