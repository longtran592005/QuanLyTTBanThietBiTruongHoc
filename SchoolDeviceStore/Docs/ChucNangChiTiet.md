# Tài Liệu Đặc Tả Chức Năng Chi Tiết (Functional Specification)
## Dự án: School Device Store — Desktop Management System

Phần mềm **School Device Store** là một ứng dụng desktop chuyên nghiệp xây dựng trên nền tảng **C# Windows Forms (.NET Framework 4.8 / Visual Studio 2026)** dùng để quản lý toàn diện hoạt động kinh doanh thiết bị trường học. 

Dưới đây là đặc tả chi tiết về mặt chức năng, kiến trúc kỹ thuật và hệ thống giao diện (UI/UX) của ứng dụng dựa trên mã nguồn thực tế.

---

## 1. Kiến Trúc Hệ Thống (System Architecture)

Ứng dụng được thiết kế theo mô hình **3 lớp (3-Tier Architecture)** chuẩn Enterprise giúp tách biệt rõ ràng các trách nhiệm kiểm soát, nâng cao khả năng bảo trì và mở rộng:

*   **Lớp Giao Diện (GUI - Presentation Layer):** Chịu trách nhiệm hiển thị dữ liệu và tiếp nhận phản hồi từ người dùng. Sử dụng các điều khiển tùy biến cao cấp (`UserControl`) để nạp động trang mà không gây nhấp nháy màn hình.
*   **Lớp Nghiệp Vụ (BLL - Business Logic Layer):** Xử lý các quy tắc nghiệp vụ như kiểm tra tính hợp lệ của dữ liệu đầu vào, tính toán tiền thuế VAT, chiết khấu, kiểm tra hàng tồn kho trước khi tạo hóa đơn, xử lý mã hóa mật khẩu, và quản lý phân quyền RBAC.
*   **Lớp Truy Xuất Dữ Liệu (DAL - Data Access Layer):** Thực hiện kết nối trực tiếp với cơ sở dữ liệu (SQLite cho chế độ chạy thử nghiệm offline và SQL Server cho môi trường Production). Sử dụng `DbHelper` với truy vấn tham số hóa (Parameterized Queries) ngăn chặn tuyệt đối lỗi bảo mật SQL Injection.
*   **Lớp Đối Tượng Truyền Tải Dữ Liệu (DTO):** Chứa các lớp thực thể thuần túy ánh xạ trực tiếp 1-1 với cấu trúc bảng trong cơ sở dữ liệu (`Product`, `Category`, `Supplier`, `Employee`, `Promotion`, `SalesOrder`, `SalesOrderDetail`, `Role`).

---

## 2. Chi Tiết Các Phân Hệ Chức Năng (Functional Modules)

### 2.1. Phân Hệ Xác Thực & Phân Quyền RBAC (Authentication & Role-Based Authorization)

#### Đăng nhập hệ thống (`LoginForm`):
*   Cung cấp màn hình đăng nhập bảo mật hai nửa (Split Screen).
*   Hỗ trợ tài khoản quản trị mặc định: Tài khoản đăng nhập demo (`admin` / `admin123`) dùng thuật toán băm mật khẩu PBKDF2 bảo mật cao thông qua `AuthService.Authenticate`.

#### Hệ thống 5 vai trò phân quyền chi tiết (5-Role RBAC):

| RoleId | Vai trò | Tên Tiếng Việt | Mô tả nghiệp vụ |
|--------|---------|----------------|-----------------|
| 1 | Admin | Quản trị viên | Toàn quyền: quản lý nhân viên, cấu hình hệ thống, sao lưu/khôi phục, xem mọi báo cáo |
| 2 | Manager | Quản lý cửa hàng | Quản lý sản phẩm, nhà cung cấp, danh mục, khuyến mãi, xem báo cáo. Không quản lý nhân viên/sao lưu |
| 3 | Salesperson | Nhân viên bán hàng | Bán hàng, xem sản phẩm (chỉ đọc), áp dụng khuyến mãi |
| 4 | Warehouse | Thủ kho | Quản lý sản phẩm (thêm/sửa tồn kho), quản lý nhà cung cấp, xem danh mục |
| 5 | Accountant | Kế toán | Xem báo cáo doanh thu, xuất dữ liệu. Không bán hàng, không quản lý sản phẩm |

#### Ma trận phân quyền chi tiết:

| Chức năng | Admin | Manager | Salesperson | Warehouse | Accountant |
|-----------|-------|---------|-------------|-----------|------------|
| Dashboard | ✅ | ✅ | ✅ | ✅ | ✅ |
| Bán hàng | ✅ | ✅ | ✅ | ❌ | ❌ |
| Sản phẩm (Xem) | ✅ | ✅ | ✅ | ✅ | ✅ |
| Sản phẩm (CRUD) | ✅ | ✅ | ❌ | ✅ | ❌ |
| Danh mục | ✅ | ✅ | ❌ | ✅ | ❌ |
| Nhà cung cấp | ✅ | ✅ | ❌ | ✅ | ❌ |
| Khuyến mãi | ✅ | ✅ | ✅ (áp dụng) | ❌ | ✅ (đọc) |
| Báo cáo | ✅ | ✅ | ❌ | ❌ | ✅ |
| Quản lý nhân viên | ✅ | ❌ | ❌ | ❌ | ❌ |
| Sao lưu/Khôi phục | ✅ | ❌ | ❌ | ❌ | ❌ |

#### Dữ liệu mẫu nhân viên (8 tài khoản):

| Username | Họ tên | Vai trò | Mật khẩu mẫu |
|----------|--------|---------|---------------|
| admin | Nguyễn Văn Admin | Admin | admin123 |
| manager1 | Trần Thị Quản Lý | Manager | manager123 |
| sale1 | Lê Văn Bán Hàng | Salesperson | sale123 |
| sale2 | Phạm Thị Kinh Doanh | Salesperson | sale123 |
| warehouse1 | Hoàng Văn Kho | Warehouse | warehouse123 |
| kttoan1 | Ngô Thị Kế Toán | Accountant | accountant123 |
| sale3 | Vũ Minh Đức | Salesperson | sale123 |
| manager2 | Đỗ Anh Tuấn | Manager | manager123 |

### 2.2. Phân Hệ Quản Lý Nhân Viên (`EmployeeManagementPage` - Chỉ Admin)
*   **Bảng danh sách nhân viên:** Hiển thị Mã NV, Tên đăng nhập, Họ tên, Email, SĐT, Vai trò, Trạng thái, Ngày tạo.
*   **Bộ lọc:** Lọc theo vai trò (5 roles) và trạng thái (Hoạt động / Đã khóa).
*   **Thêm nhân viên:** Dialog `EmployeeEditorForm` với các trường: tên đăng nhập, họ tên, email, SĐT, vai trò, mật khẩu.
*   **Sửa nhân viên:** Cập nhật thông tin (không đổi được username), thay đổi vai trò.
*   **Khóa/Kích hoạt tài khoản:** Vô hiệu hóa (Deactivate) thay vì xóa. Nhân viên bị khóa không thể đăng nhập. Không thể khóa tài khoản admin chính.
*   **Đặt lại mật khẩu:** Dialog nhập mật khẩu mới (tối thiểu 6 ký tự), hash bằng PBKDF2.

### 2.3. Phân Hệ Trang Chủ - Dashboard Tổng Quan (`DashboardPage`)
*   **Các thẻ KPI phân tích trực tiếp:** Hiển thị 4 chỉ số vận hành quan trọng trong vòng 30 ngày gần nhất.
*   **Biểu đồ xu hướng doanh thu (`Chart`):** Biểu đồ cột trực quan hóa doanh thu hàng ngày trong 14 ngày gần nhất.
*   **Danh sách cảnh báo tồn kho tối thiểu:** Bảng hiển thị 10 sản phẩm sắp hết hàng nhất.
*   **Danh sách hóa đơn gần đây:** Bảng hiển thị 10 hóa đơn lập gần nhất.

### 2.4. Phân Hệ Quản Lý Sản Phẩm (Product Management)
*   **Giao diện chia đôi màn hình:** Bảng danh sách + Thẻ xem nhanh chi tiết.
*   **Tìm kiếm & Bộ lọc nâng cao:** Real-time Search, lọc trạng thái kho hàng.
*   **Thao tác CRUD hoàn chỉnh:** Thêm/Sửa/Xóa sản phẩm qua `ProductEditorForm`.
*   **Xuất báo cáo:** Xuất danh sách sản phẩm ra CSV.

### 2.5. Phân Hệ Quản Lý Danh Mục & Nhà Cung Cấp
*   **Quản lý danh mục thiết bị:** 10 danh mục mẫu, CRUD đầy đủ.
*   **Quản lý nhà cung cấp:** 5 nhà cung cấp mẫu, CRUD đầy đủ.

### 2.6. Phân Hệ Khuyến Mãi (`PromotionManagementPage`)
#### Tính năng quản lý:
*   **Bảng danh sách khuyến mãi:** Hiển thị mã, tên, loại giảm giá, giá trị, thời gian, số lần sử dụng, trạng thái.
*   **Bộ lọc trạng thái:** Đang hoạt động / Sắp diễn ra / Đã hết hạn / Ngừng hoạt động / Đã hết lượt.
*   **Panel chi tiết:** Xem nhanh thông tin đầy đủ của khuyến mãi đang chọn.
*   **CRUD:** Tạo/Sửa/Xóa chương trình khuyến mãi qua `PromotionEditorForm`.

#### Loại khuyến mãi:
*   **Phần trăm (%):** Giảm X% trên tổng đơn hàng, có giới hạn tối đa (MaxDiscountAmount).
*   **Số tiền cố định (₫):** Giảm trực tiếp X đồng trên đơn hàng.

#### Điều kiện áp dụng:
*   **Đơn hàng tối thiểu:** Chỉ áp dụng khi subtotal >= MinOrderAmount.
*   **Thời gian hiệu lực:** StartDate → EndDate, kiểm tra tự động.
*   **Giới hạn sử dụng:** Số lần tối đa (hoặc không giới hạn). Tự động đếm UsageCount.
*   **Phạm vi áp dụng:** Tất cả sản phẩm / Danh mục cụ thể / Sản phẩm cụ thể.

#### Dữ liệu mẫu (5 chương trình):
| Mã | Tên | Loại | Giá trị | Trạng thái |
|----|-----|------|---------|-----------|
| SUMMER2026 | Khuyến mãi Hè 2026 | % | 10% (max 5M) | Đang hoạt động |
| BIGORDER | Ưu đãi đơn lớn | Cố định | 2,000,000 ₫ | Đang hoạt động |
| STEM50 | Ưu đãi thiết bị STEM | % | 15% (max 3M) | Đang hoạt động |
| WELCOME | Chào mừng khách mới | % | 5% (max 2M) | Đang hoạt động |
| EXPIRED01 | Khuyến mãi Tết | % | 20% | Đã hết hạn |

### 2.7. Phân Hệ Bán Hàng & Lập Hóa Đơn (`SalesPage`)
*   **Tìm kiếm thông minh Autocomplete:** Gợi ý Khách hàng và Sản phẩm.
*   **Kiểm tra tồn kho thời gian thực.**
*   **Giỏ hàng tương tác linh hoạt.**
*   **Bảng tính toán tài chính tự động:** Tạm tính, Giảm giá, VAT, Tổng cộng.
*   **Tích hợp khuyến mãi:** Nhập mã khuyến mãi để áp dụng giảm giá tự động (thông qua `PromotionService`).
*   **Quy trình ghi nhận hóa đơn an toàn:** SQLite Transaction.
*   **In ấn chuyên nghiệp:** Print Preview chuẩn hóa.

### 2.8. Phân Hệ Thống Kê & Báo Cáo Doanh Thu Nâng Cao (`ReportsPage`)

#### Bộ lọc nâng cao:
*   **Bộ lọc nhanh (Quick Filters):** Các nút "Hôm nay", "Hôm qua", "7 ngày qua", "Tháng này", "Quý này", "Năm nay".
*   **Lọc thời gian thủ công:** DateTimePicker "Từ ngày" - "Đến ngày".
*   **Lọc theo đối tượng:** Dropdown lọc theo Danh mục sản phẩm và Nhân viên bán hàng.

#### KPI Cards với chỉ số tăng trưởng:
*   **Tổng doanh thu:** Kèm % tăng/giảm so với kỳ trước (↑ xanh / ↓ đỏ).
*   **Tổng số hóa đơn:** Kèm % tăng/giảm.
*   **Giá trị đơn hàng trung bình (AOV):** Kèm % tăng/giảm.

#### Biểu đồ trực quan hóa nâng cao:
*   **Biểu đồ doanh thu:** Hỗ trợ chuyển đổi Cột / Đường / Vùng (Column/Line/Area).
*   **So sánh kỳ trước:** Dual-series hiển thị kỳ hiện tại và kỳ trước trên cùng biểu đồ.
*   **Biểu đồ Donut:** Cơ cấu doanh thu theo danh mục sản phẩm.
*   **Định dạng rút gọn:** Trục Y tự động rút gọn (1,500,000 → "1.5 Tr").

#### Bảng dữ liệu tương tác:
*   **Bảng doanh thu theo ngày:** Hỗ trợ sort (click header).
*   **Bảng sản phẩm bán chạy:** Top 10, hỗ trợ sort.
*   **Panel cảnh báo tồn kho:** Hiển thị sản phẩm sắp hết hàng / hết hàng.

#### Xuất báo cáo đa định dạng:
*   **Xuất CSV:** Dữ liệu thuần cho phân tích.
*   **Xuất Excel (XLS):** HTML-table format mở được bằng Excel.
*   **Xuất PDF:** Print Preview với layout báo cáo chuyên nghiệp.

### 2.9. Phân Hệ Sao Lưu & Khôi Phục (`BackupRestorePage`)
*   **Sao lưu cơ sở dữ liệu:** Sao chép file `.bak`.
*   **Khôi phục dữ liệu:** Khôi phục từ file `.bak` với cảnh báo ghi đè.

---

## 3. Điểm Nhấn Thiết Kế Giao Diện & Trải Nghiệm (UI/UX Highlights)

Ứng dụng sử dụng bộ nguyên tắc thiết kế **Premium Utilitarian Minimalism**:

*   **Bảng màu tối giản:** Ceramic Blue (`#2563EB`), nền trắng, Warm Bone sidebar.
*   **Phông chữ Segoe UI** với hệ thống phân cấp kích thước rõ ràng.
*   **Custom Controls:** `RoundedTextBox`, `KpiCardControl`, `LoadingOverlayControl`, `EmptyStateControl`.
