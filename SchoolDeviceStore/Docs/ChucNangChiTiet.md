# Tài Liệu Đặc Tả Chức Năng Chi Tiết (Functional Specification)
## Dự án: School Device Store — Desktop Management System

Phần mềm **School Device Store** là một ứng dụng desktop chuyên nghiệp xây dựng trên nền tảng **C# Windows Forms (.NET Framework 4.8 / Visual Studio 2026)** dùng để quản lý toàn diện hoạt động kinh doanh thiết bị trường học. 

Dưới đây là đặc tả chi tiết về mặt chức năng, kiến trúc kỹ thuật và hệ thống giao diện (UI/UX) của ứng dụng dựa trên mã nguồn thực tế.

---

## 1. Kiến Trúc Hệ Thống (System Architecture)

Ứng dụng được thiết kế theo mô hình **3 lớp (3-Tier Architecture)** chuẩn Enterprise giúp tách biệt rõ ràng các trách nhiệm kiểm soát, nâng cao khả năng bảo trì và mở rộng:

```mermaid
graph TD
    subgraph GUI (Presentation Layer)
        MainForm[MainForm - Khung Giao Diện Chính]
        LoginForm[LoginForm - Đăng Nhập Hệ Thống]
        Pages[Thành Phần Trang - Pages: Dashboard, Sales, Products...]
        Controls[Component UI Tùy Biến: RoundedTextBox, KpiCard...]
    end
    
    subgraph BLL (Business Logic Layer)
        AuthService[AuthService - Xác Thực]
        SalesService[SalesService - Nghiệp Vụ Bán Hàng]
        ProductService[ProductService - Quản Lý Sản Phẩm]
        ReportService[ReportService - Tổng Hợp Báo Cáo]
        BackupService[BackupService - Sao Lưu Hệ Thống]
        ValidationHelper[ValidationHelper - Kiểm Tra Dữ Liệu]
    end

    subgraph DAL (Data Access Layer)
        DbHelper[DbHelper - Kết Nối SQLite/SQL Server]
        Repositories[Repositories - Lưu Trữ: ProductRepo, SalesRepo...]
        DemoDbInit[DemoDatabaseInitializer - Khởi Tạo Database Offline]
    end

    subgraph DTO (Data Transfer Objects)
        Entities[Entities: Product, Category, Supplier, Employee, SalesOrder...]
    end

    GUI --> BLL
    BLL --> DAL
    DAL --> DTO
    GUI -.-> DTO
```

*   **Lớp Giao Diện (GUI - Presentation Layer):** Chịu trách nhiệm hiển thị dữ liệu và tiếp nhận phản hồi từ người dùng. Sử dụng các điều khiển tùy biến cao cấp (`UserControl`) để nạp động trang mà không gây nhấp nháy màn hình.
*   **Lớp Nghiệp Vụ (BLL - Business Logic Layer):** Xử lý các quy tắc nghiệp vụ như kiểm tra tính hợp lệ của dữ liệu đầu vào, tính toán tiền thuế VAT, chiết khấu, kiểm tra hàng tồn kho trước khi tạo hóa đơn và xử lý mã hóa mật khẩu.
*   **Lớp Truy Xuất Dữ Liệu (DAL - Data Access Layer):** Thực hiện kết nối trực tiếp với cơ sở dữ liệu (SQLite cho chế độ chạy thử nghiệm offline và SQL Server cho môi trường Production). Sử dụng `DbHelper` với truy vấn tham số hóa (Parameterized Queries) ngăn chặn tuyệt đối lỗi bảo mật SQL Injection.
*   **Lớp Đối Tượng Truyền Tải Dữ Liệu (DTO - Data Transfer Objects):** Chứa các lớp thực thể thuần túy ánh xạ trực tiếp 1-1 với cấu trúc bảng trong cơ sở dữ liệu (`Product`, `Category`, `Supplier`, `Employee`, `SalesOrder`, `SalesOrderDetail`, `Role`).

---

## 2. Chi Tiết Các Phân Hệ Chức Năng (Functional Modules)

### 2.1. Phân Hệ Xác Thực & Phân Quyền (Authentication & Authorization)
*   **Đăng nhập hệ thống (`LoginForm`):**
    *   Cung cấp màn hình đăng nhập bảo mật hai nửa (Split Screen):
        *   *Nửa bên trái (Brand Panel):* Chứa hình họa trang trí hình học chuyển màu (Gradient) tinh tế, nghệ thuật và 3 thẻ giới thiệu các thế mạnh của hệ thống ("Giao diện quản trị thống nhất", "Phân quyền linh hoạt chuẩn xác", "Báo cáo thống kê thời gian thực").
        *   *Nửa bên phải (Login Form Card):* Thẻ đăng nhập đổ bóng mềm, ô nhập liệu bo góc hiện đại, hỗ trợ tính năng ẩn/hiện mật khẩu động bằng nút checkbox và nút "Thoát ứng dụng" xác nhận an toàn.
    *   Hỗ trợ tài khoản quản trị mặc định: Tài khoản đăng nhập demo (`admin` / `admin123`) dùng thuật toán băm mật khẩu bảo mật cao thông qua `AuthService.Authenticate`.
*   **Phân quyền dựa trên vai trò (Role-based Authorization):**
    *   Khi người dùng đăng nhập thành công, vai trò (`RoleId`) sẽ được lưu vào phiên làm việc.
    *   **Quản trị viên (Admin - RoleId = 1):** Toàn quyền truy cập tất cả các tính năng trong hệ thống.
    *   **Nhân viên & Kế toán (RoleId ≠ 1):** Hệ thống tự động ẩn đi các tính năng quản lý nhạy cảm để đảm bảo an toàn dữ liệu, bao gồm:
        1.  *Nhà cung cấp (Suppliers)*
        2.  *Báo cáo doanh thu & sản phẩm bán chạy (Reports)*
        3.  *Sao lưu & khôi phục cơ sở dữ liệu (Backup/Restore)*

### 2.2. Phân Hệ Trang Chủ - Dashboard Tổng Quan (`DashboardPage`)
*   **Các thẻ KPI phân tích trực tiếp:** Hiển thị 4 chỉ số vận hành quan trọng trong vòng 30 ngày gần nhất:
    *   *Doanh thu (30 ngày):* Tổng số tiền bán hàng (sau chiết khấu + thuế) được định dạng tiền tệ sắc nét.
    *   *Tổng số hóa đơn:* Tổng số đơn hàng thành công.
    *   *Tổng sản phẩm:* Số lượng mã thiết bị hiện có trong danh mục.
    *   *Cảnh báo sắp hết hàng:* Số lượng mặt hàng có tồn kho nguy hiểm (Số lượng $\le$ 5).
*   **Biểu đồ xu hướng doanh thu (`Chart`):** 
    *   Biểu đồ cột (Column Chart) trực quan hóa doanh thu hàng ngày trong 14 ngày gần nhất.
    *   Tích hợp định dạng rút gọn nhãn trục Y thông minh (Ví dụ: `1,500,000 ₫` tự động rút gọn thành `1.5 Tr` để biểu đồ không bị rối chữ).
*   **Danh sách cảnh báo tồn kho tối thiểu:**
    *   Bảng hiển thị danh sách 10 sản phẩm sắp hết hàng nhất (tồn $\le$ 5) sắp xếp tăng dần để người quản lý chủ động nhập thêm thiết bị.
*   **Danh sách hóa đơn gần đây:** 
    *   Bảng hiển thị 10 hóa đơn lập gần nhất với đầy đủ thông tin: Mã hóa đơn, Ngày giờ lập, Tổng tiền, Giảm giá, và VAT.
*   **Thao tác nhanh:** Các nút tắt hỗ trợ chuyển hướng trang nhanh chóng đến phân hệ Bán hàng, Sản phẩm, Báo cáo và Sao lưu dữ liệu.

### 2.3. Phân Hệ Quản Lý Sản Phẩm (Product Management)
*   **Giao diện chia đôi màn hình (Split Panel):**
    *   *Bên trái:* Bảng danh sách sản phẩm hiển thị các thông tin cơ bản được thiết kế phẳng đẹp mắt, hỗ trợ tô màu dòng so le (Alternate Row Color), tự động co giãn cột.
    *   *Bên phải (Detail Preview Card):* Thẻ xem nhanh thông tin chi tiết của sản phẩm đang được chọn trên lưới (hiển thị đầy đủ thông tin giá mua, giá bán, danh mục, nhà cung cấp, trạng thái, giúp giảm tải thao tác nhấp đúp của người dùng).
*   **Tìm kiếm & Bộ lọc nâng cao:**
    *   Tìm kiếm tức thời (Real-time Search) khi gõ ký tự theo **Mã sản phẩm** hoặc **Tên sản phẩm**.
    *   Lọc trạng thái kho hàng: "Tất cả", "Còn hàng" (Số lượng > 0), "Hết hàng" (Số lượng = 0).
*   **Thao tác CRUD hoàn chỉnh:**
    *   **Thêm & Sửa sản phẩm (`ProductEditorForm`):** Mở cửa sổ hội thoại (Dialog) thiết kế riêng để điền thông tin chi tiết: Mã sản phẩm, Tên sản phẩm, Danh mục (Combobox tải từ DB), Nhà cung cấp (Combobox tải từ DB), Số lượng, Giá nhập, Giá bán, Trạng thái (Available/Out of Stock) và mô tả chi tiết.
    *   **Xóa sản phẩm:** Hiển thị hộp thoại cảnh báo nguy hiểm trước khi xóa thực tế để tránh thao tác nhầm lẫn từ phía người dùng.
*   **Xuất báo cáo:** Tích hợp nút xuất nhanh toàn bộ danh sách sản phẩm hiện tại ra file định dạng CSV (`products.csv`).

### 2.4. Phân Hệ Quản Lý Danh Mục & Nhà Cung Cấp (Category & Supplier)
*   **Quản lý danh mục thiết bị (`CategoryManagementPage`):**
    *   Hiển thị danh sách các danh mục thiết bị trường học (Ví dụ: Thiết bị phòng thí nghiệm, Dụng cụ thể thao, Thiết bị tin học...).
    *   Hỗ trợ thêm mới danh mục, sửa đổi thông tin hoặc xóa danh mục không còn sử dụng.
*   **Quản lý nhà cung cấp đối tác (`SupplierManagementPage`):**
    *   Hồ sơ chi tiết của từng nhà cung cấp bao gồm: Tên nhà cung cấp, Người liên hệ đại diện, Số điện thoại, Email và Địa chỉ văn phòng.
    *   Quản lý danh sách đối tác cung ứng thiết bị giúp tối ưu hóa chuỗi nhập hàng.

### 2.5. Phân Hệ Bán Hàng & Lập Hóa Đơn (Sales & Invoicing - `SalesPage`)
Đây là phân hệ nghiệp vụ cốt lõi và phức tạp nhất của ứng dụng:
*   **Tìm kiếm thông minh Autocomplete:**
    *   Ô tìm kiếm Khách hàng và ô tìm kiếm Sản phẩm tích hợp khả năng gợi ý danh sách tự động (`SuggestAppend`) khi người dùng bắt đầu gõ ký tự, hỗ trợ tìm nhanh theo mã, tên hoặc số điện thoại.
*   **Kiểm tra tồn kho thời gian thực (Real-time Stock Validation):**
    *   Khi chọn một sản phẩm, nhãn tồn kho sẽ hiển thị số lượng còn lại trong kho. Nếu sản phẩm sắp hết hàng ($\le$ 5), nhãn sẽ tự động chuyển sang màu vàng/đỏ cảnh báo.
    *   Khi bấm **"Thêm vào giỏ"**, hệ thống kiểm tra BLL sẽ so sánh tổng số lượng yêu cầu (bao gồm cả số lượng đã nằm trong giỏ) với số lượng tồn thực tế. Nếu vượt quá, hệ thống sẽ từ chối và hiển thị thông báo lỗi để tránh tình trạng xuất âm kho.
*   **Giỏ hàng tương tác linh hoạt:**
    *   Lưới giỏ hàng (`_cartGrid`) hỗ trợ người dùng sửa số lượng trực tiếp trên từng ô. Hệ thống tự động kích hoạt sự kiện kiểm tra tồn kho và tính toán lại giá trị hóa đơn ngay lập tức.
    *   Cung cấp nút **"Xóa"** nhanh tích hợp trên mỗi dòng sản phẩm và nút **"Xóa giỏ hàng"** có cảnh báo để dọn dẹp giỏ hàng nháp.
*   **Bảng tính toán tài chính tự động (Real-time Calculator):**
    *   Nhập giá trị chiết khấu trực tiếp (hỗ trợ giảm giá tiền mặt).
    *   Nhập phần trăm thuế VAT (mặc định 10%).
    *   Bộ máy tính toán tự động cập nhật:
        $$\text{Tạm tính} = \sum (\text{Số lượng} \times \text{Đơn giá})$$
        $$\text{Tiền thuế VAT} = (\text{Tạm tính} - \text{Giảm giá}) \times \text{VAT}\%$$
        $$\text{Tổng cộng thanh toán} = (\text{Tạm tính} - \text{Giảm giá}) + \text{Tiền thuế VAT}$$
*   **Chọn trạng thái thanh toán:** Tùy chọn 3 trạng thái của đơn hàng ("Đã thanh toán", "Chờ thanh toán", "Thanh toán một phần").
*   **Quy trình ghi nhận hóa đơn an toàn:**
    *   Khi nhấn nút **"Tạo hóa đơn"**, hệ thống sẽ thực thi một SQLite Transaction để lưu đồng thời hóa đơn vào bảng `SalesOrders`, lưu chi tiết các dòng hàng vào bảng `SalesOrderDetails` và tự động trừ số lượng tồn kho của các sản phẩm tương ứng trong bảng `Products`.
*   **In ấn chuyên nghiệp (`InvoicePrintService`):**
    *   Sau khi tạo hóa đơn thành công, các nút **"Xem trước"** và **"In hóa đơn"** sẽ được kích hoạt.
    *   Hệ thống hiển thị hộp thoại xem trước tài liệu in (Print Preview) chuẩn hóa với đầy đủ thông tin: Logo cửa hàng, Tên trung tâm thiết bị, Thông tin khách hàng, Danh sách mặt hàng mua, Tổng tiền bằng chữ và phần ký tên xác nhận. Hỗ trợ xuất trực tiếp ra máy in giấy hoặc in ảo sang tệp PDF.

### 2.6. Phân Hệ Thống Kê & Báo Cáo Doanh Thu (Analytics & Reporting)
*   **Bộ lọc thời gian linh hoạt:** Lọc dữ liệu báo cáo kinh doanh theo bất kỳ khoảng thời gian nào bằng hai ô chọn ngày (`DateTimePicker`) "Từ ngày" và "Đến ngày".
*   **Bộ chỉ số kinh doanh cốt lõi (KPIs):**
    *   *Tổng doanh thu:* Tổng doanh số thu về trong khoảng thời gian đã chọn.
    *   *Tổng hóa đơn:* Số lượng giao dịch thành công.
    *   *Giá trị đơn hàng trung bình (AOV):* Bằng Tổng doanh thu chia cho Tổng số hóa đơn, giúp đánh giá hiệu quả sức mua.
*   **Biểu đồ cột doanh thu:** Vẽ trực quan doanh thu theo các ngày trong khoảng thời gian được lọc để nhìn ra chu kỳ bán hàng cao điểm.
*   **Hai bảng dữ liệu báo cáo chi tiết:**
    *   *Bảng doanh thu theo ngày:* Liệt kê chi tiết số tiền bán hàng của từng ngày.
    *   *Bảng sản phẩm bán chạy:* Liệt kê top các sản phẩm có số lượng bán nhiều nhất kèm theo tổng doanh thu mà sản phẩm đó mang lại.
*   **Xuất file báo cáo:** Cho phép xuất dữ liệu của cả hai bảng báo cáo ra các tệp tin CSV riêng biệt (`revenue-report.csv`, `top-products.csv`) phục vụ lưu trữ hoặc phân tích sâu bằng Microsoft Excel.

### 2.7. Phân Hệ Sao Lưu & Khôi Phục Dữ Liệu (Backup & Restore - `BackupRestorePage`)
*   **Sao lưu cơ sở dữ liệu (Database Backup):**
    *   Cho phép sao chép toàn bộ tệp cơ sở dữ liệu SQLite hiện hành thành một tệp sao lưu có đuôi mở rộng `.bak` tại bất kỳ đường dẫn lưu trữ nào trong máy tính (sử dụng hộp thoại `SaveFileDialog`).
*   **Khôi phục dữ liệu (Database Restore):**
    *   Cho phép người dùng khôi phục cơ sở dữ liệu từ một tệp sao lưu `.bak` trước đó (sử dụng `OpenFileDialog`).
    *   Hệ thống có cơ chế cảnh báo ghi đè dữ liệu hiện tại trước khi thực hiện để đảm bảo an toàn tuyệt đối cho tệp tin đang vận hành.

---

## 3. Điểm Nhấn Thiết Kế Giao Diện & Trải Nghiệm (UI/UX Highlights)

Để vượt qua những hạn chế thô kệch mặc định của Windows Forms truyền thống, ứng dụng đã được áp dụng bộ nguyên tắc thiết kế **Premium Utilitarian Minimalism** (Tối giản cao cấp và thực dụng):

*   **Bảng màu sắc tối giản & Sang trọng (Color Palette):**
    *   *Màu thương hiệu chính (Primary Color):* Ceramic Blue (`#2563EB` - Màu xanh dương cao cấp của Tailwind) tạo cảm giác chuyên nghiệp, tin cậy.
    *   *Nền chính:* Màu trắng tinh khiết (`#FFFFFF`) kết hợp đường viền mỏng Slate nhẹ nhàng (`#E2E8F0`) thay thế cho các màu xám Windows 98 truyền thống.
    *   *Thanh điều hướng bên trái (Sidebar):* Warm Bone (`#F7F6F3` - màu xám ấm phong cách Notion) giúp mắt thư giãn khi vận hành thời gian dài.
    *   *Các màu tín hiệu rõ ràng:* Xanh lục Emerald (`#10B981`) báo hiệu thành công/đủ hàng; Màu cam Amber (`#F59E0B`) cảnh báo sắp hết hàng; Màu đỏ Rose (`#EF4444`) cảnh báo lỗi/hết hàng.
*   **Phông chữ đồng bộ (Typography):**
    *   Sử dụng phông chữ **Segoe UI** với hệ thống phân cấp kích thước rõ ràng (từ phông nền BaseFont 9.5pt đến SectionTitleFont 13pt và TitleFont 18pt đậm nét), mang lại cảm giác chữ sắc sảo giống như các hệ thống web hiện đại sử dụng phông chữ Inter hoặc SF Pro.
*   **Các thành phần UI Tùy biến Cao cấp (Custom Controls):**
    *   `RoundedTextBox`: Ô nhập liệu bo tròn góc với hiệu ứng gợi ý mờ (Placeholder) và đường viền chuyển màu khi được trỏ chuột.
    *   `KpiCardControl`: Thẻ hiển thị chỉ số có đường viền bo tròn nhẹ, có tiêu đề phụ và màu sắc nhấn tương ứng với loại dữ liệu.
    *   `LoadingOverlayControl`: Màn che mờ hiển thị vòng xoay tải dữ liệu khi trang thực hiện các tác vụ nặng (nhập hàng, in hóa đơn), tạo phản hồi UX liên tục cho người dùng.
    *   `EmptyStateControl`: Thay thế các bảng trống trơn đơn điệu bằng hình ảnh minh họa nhẹ nhàng và thông điệp hướng dẫn cụ thể (ví dụ: "Giỏ hàng trống - Hãy chọn sản phẩm để tiếp tục").
