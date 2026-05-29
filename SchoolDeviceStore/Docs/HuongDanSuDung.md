# Hướng Dẫn Sử Dụng Hệ Thống Quản Lý Cửa Hàng Thiết Bị Trường Học
**(School Device Store - User Manual)**

Chào mừng bạn đến với tài liệu hướng dẫn sử dụng phần mềm quản lý **School Device Store**. Tài liệu này sẽ hướng dẫn bạn chi tiết các bước để bắt đầu và khai thác toàn bộ các tính năng của phần mềm từ A-Z.

---

## MỤC LỤC
1. [Khởi Động & Đăng Nhập](#1-khởi-động--đăng-nhập)
2. [Trang Chủ (Dashboard)](#2-trang-chủ-dashboard)
3. [Phân Hệ Bán Hàng (POS)](#3-phân-hệ-bán-hàng-pos)
4. [Quản Lý Kho: Sản Phẩm, Danh Mục, Nhà Cung Cấp](#4-quản-lý-kho-sản-phẩm-danh-mục-nhà-cung-cấp)
5. [Quản Lý Khuyến Mãi](#5-quản-lý-khuyến-mãi)
6. [Thống Kê & Báo Cáo Doanh Thu](#6-thống-kê--báo-cáo-doanh-thu)
7. [Quản Lý Nhân Viên (Chỉ dành cho Admin)](#7-quản-lý-nhân-viên)
8. [Sao Lưu & Khôi Phục Dữ Liệu](#8-sao-lưu--khôi-phục-dữ-liệu)

---

## 1. Khởi Động & Đăng Nhập

Mở tệp thực thi của phần mềm, bạn sẽ được đưa tới màn hình Đăng Nhập.

![Đăng nhập](../../anh%20demo/Đăng%20nhập.png)

**Cách thực hiện:**
1. Nhập **Tên đăng nhập (Username)** và **Mật khẩu (Password)**.
2. Bấm nút **Đăng nhập**.

**Tài khoản dùng thử (Demo):**
- Quyền Quản trị viên (Toàn quyền): Tên đăng nhập: `admin` | Mật khẩu: `admin123`
- Quyền Quản lý: Tên đăng nhập: `manager1` | Mật khẩu: `manager123`
- Quyền Bán hàng: Tên đăng nhập: `sale1` | Mật khẩu: `sale123`

*(Lưu ý: Tùy theo vai trò của tài khoản, một số chức năng có thể bị ẩn đi để đảm bảo bảo mật và đúng quyền hạn).*

---

## 2. Trang Chủ (Dashboard)

Sau khi đăng nhập thành công, hệ thống sẽ đưa bạn đến **Trang chủ (Dashboard)**. Đây là nơi bạn có cái nhìn tổng quan về tình hình kinh doanh trong 30 ngày qua.

![Trang chủ](../../anh%20demo/Trang%20chủ.png)

**Các thông tin chính:**
- **Thẻ KPI:** Xem nhanh *Tổng doanh thu, Số lượng đơn hàng, Sản phẩm bán chạy nhất,...*
- **Biểu đồ doanh thu:** Trực quan hóa doanh thu theo ngày để bạn dễ dàng theo dõi xu hướng kinh doanh.
- **Cảnh báo tồn kho:** Danh sách các sản phẩm sắp hết hàng cần nhập thêm.
- **Hóa đơn gần đây:** Các hóa đơn bán hàng vừa được tạo.

---

## 3. Phân Hệ Bán Hàng (POS)

Phân hệ này dành cho thu ngân / nhân viên bán hàng để tạo hóa đơn cho khách.

![Bán hàng](../../anh%20demo/Bán%20hàng.png)

**Quy trình lập hóa đơn:**
1. **Thêm sản phẩm vào giỏ:** 
   - Nhập tên sản phẩm vào ô tìm kiếm (hệ thống sẽ tự gợi ý - Autocomplete).
   - Chọn sản phẩm, hệ thống tự động kiểm tra số lượng tồn kho. Nếu đủ hàng, sản phẩm sẽ được thêm vào giỏ.
2. **Cập nhật giỏ hàng:** Bạn có thể thay đổi số lượng hoặc xóa sản phẩm khỏi giỏ hàng.
3. **Thông tin khách hàng:** Nhập tên khách hàng (nếu cần thiết).
4. **Áp dụng khuyến mãi:** Nhập *Mã khuyến mãi* (nếu có) vào ô khuyến mãi để áp dụng giảm giá.
5. **Thanh toán:** Kiểm tra lại *Tạm tính, Giảm giá, VAT (nếu có)* và *Tổng cộng*. Bấm nút **Thanh toán** để hoàn tất.
6. **In hóa đơn:** Hệ thống sẽ hiển thị bảng xem trước (Print Preview) để bạn có thể in hóa đơn giao cho khách.

---

## 4. Quản Lý Kho: Sản Phẩm, Danh Mục, Nhà Cung Cấp

Dành cho thủ kho hoặc quản lý để theo dõi và cập nhật hàng hóa.

### 4.1. Quản Lý Sản Phẩm
![Sản phẩm](../../anh%20demo/Sản%20phẩm.png)
- **Xem thông tin:** Danh sách sản phẩm được hiển thị dạng bảng bên trái. Khi click vào một sản phẩm, chi tiết sẽ hiện ra ở bảng bên phải.
- **Thêm/Sửa/Xóa:** Sử dụng các nút chức năng để Thêm mới sản phẩm, Chỉnh sửa thông tin (tên, giá, tồn kho) hoặc Xóa.
- **Tìm kiếm & Lọc:** Hỗ trợ tìm kiếm theo tên và lọc theo tình trạng tồn kho.
- **Xuất CSV:** Bạn có thể xuất danh sách kho hàng ra file CSV để quản lý ngoài.

### 4.2. Quản Lý Danh Mục & Nhà Cung Cấp
![Danh mục](../../anh%20demo/Danh%20mục.png)
![Nhà cung cấp](../../anh%20demo/Nhà%20cung%20cấp.png)
- Quản lý các phân loại mặt hàng (Ví dụ: Dụng cụ học tập, Thiết bị điện tử,...) và các đối tác cung cấp hàng.
- Hỗ trợ đầy đủ các thao tác Thêm, Chỉnh sửa, Xóa.

---

## 5. Quản Lý Khuyến Mãi

Cho phép tạo và theo dõi các chương trình giảm giá.
- **Tạo khuyến mãi mới:** Chọn Thêm mới, điền các thông tin: *Mã khuyến mãi, Tên chương trình, Loại giảm giá (% hoặc số tiền), Giá trị, Số lượng giới hạn, và Thời gian áp dụng*.
- **Theo dõi:** Xem trạng thái của khuyến mãi (*Đang hoạt động, Đã hết hạn, Hết lượt...*).
- **Phạm vi áp dụng:** Khuyến mãi có thể thiết lập cho hóa đơn đạt giá trị tối thiểu.

---

## 6. Thống Kê & Báo Cáo Doanh Thu

Kế toán và Quản trị viên sử dụng phân hệ này để tổng hợp dữ liệu tài chính.

![Báo cáo](../../anh%20demo/Báo%20cáo.png)

- **Bộ lọc linh hoạt:** Bạn có thể xem báo cáo theo *Hôm nay, Tuần này, Tháng này* hoặc tự chọn một khoảng thời gian (Từ ngày - Đến ngày).
- **Biểu đồ đa dạng:** Hỗ trợ biểu đồ cột, đường hoặc vùng (Area) so sánh với kỳ trước. Biểu đồ tròn cơ cấu doanh thu theo danh mục.
- **Bảng số liệu chi tiết:** Xem báo cáo doanh thu theo ngày, top sản phẩm bán chạy.
- **Xuất báo cáo:** Hỗ trợ xuất dữ liệu ra file **Excel**, **CSV**, và **PDF**.

---

## 7. Quản Lý Nhân Viên

*(Lưu ý: Phân hệ này chỉ hiển thị khi đăng nhập bằng tài khoản Admin)*

- **Quản lý danh sách:** Xem toàn bộ nhân viên, thông tin liên lạc và vai trò của họ trong hệ thống (Admin, Manager, Salesperson, Warehouse, Accountant).
- **Phân quyền:** Cấp hoặc thay đổi quyền truy cập cho nhân viên.
- **Khóa tài khoản:** Thay vì xóa hoàn toàn, bạn có thể "Khóa" tài khoản để nhân viên nghỉ việc không thể đăng nhập, trong khi vẫn giữ lại lịch sử bán hàng của họ.
- **Reset mật khẩu:** Đặt lại mật khẩu mới khi nhân viên quên.

---

## 8. Sao Lưu & Khôi Phục Dữ Liệu

*(Lưu ý: Chức năng dành cho Quản trị viên, đảm bảo an toàn dữ liệu)*

![Sao lưu](../../anh%20demo/Sao%20lưu-Khôi%20phục.png)

- **Sao lưu (Backup):** Hệ thống sẽ tạo một bản sao lưu an toàn (`.bak`) của toàn bộ cơ sở dữ liệu hiện tại, giúp phòng tránh mất mát dữ liệu do sự cố.
- **Khôi phục (Restore):** Phục hồi lại dữ liệu từ một file `.bak` đã sao lưu trước đó. *Cảnh báo: Hành động này sẽ ghi đè lên dữ liệu hiện tại.*

---

**Chúc bạn có trải nghiệm tuyệt vời và quản lý công việc hiệu quả với School Device Store!** 🚀
