# MÔ TẢ CHỨC NĂNG GIAO DIỆN (CHI TIẾT)

Tài liệu này mô tả chi tiết chức năng và hành vi dự kiến của các form chính trong hệ thống quản lý trung tâm bán thiết bị trường học. Mục tiêu là cung cấp mô tả đủ chi tiết để phục vụ phần viết tài liệu hướng dẫn sử dụng, kiểm thử chức năng và mô tả nghiệp vụ cho người đánh giá.

---

## 2.3.2. Form trang chủ

Mục đích: cung cấp cái nhìn tổng quan về tình trạng vận hành của cửa hàng ngay khi người dùng đăng nhập.

Nội dung và chức năng chi tiết:
- Thanh chỉ số (dashboard widgets): hiển thị các chỉ số tóm tắt như tổng doanh thu trong ngày/tuần/tháng, số hóa đơn đã lập trong ngày, tổng số khách hàng mới, tổng tồn kho cảnh báo (số mặt hàng < ngưỡng), và doanh thu theo kênh (nếu có). Mỗi widget có tooltip giải thích, và click vào widget sẽ điều hướng tới màn hình chi tiết tương ứng (ví dụ click "Tồn kho cảnh báo" mở `Form lịch sử tồn kho` lọc theo mức thấp).
- Thanh thông báo: hiển thị chuỗi thông báo ngắn (ví dụ: "Sao lưu hôm nay chưa chạy", "Mã KM KHAI-GIANG sẽ hết hạn trong 2 ngày") cùng nút để đóng hoặc xem chi tiết.
- Tìm kiếm nhanh: ô tìm kiếm toàn cục cho phép nhập mã sản phẩm, tên sản phẩm, mã khách hàng, hoặc số hóa đơn; hỗ trợ gợi ý (typeahead) dựa trên dữ liệu gần đây.
- Phím tắt và menu hành động: các nút nhanh để vào `Bán hàng`, `Thêm sản phẩm`, `Sao lưu`, `Báo cáo`, v.v., có thể cấu hình theo quyền người dùng.
- Biểu đồ: biểu đồ doanh thu theo thời gian (line chart) và biểu đồ cơ cấu danh mục bán chạy (pie chart). Biểu đồ hỗ trợ thay đổi khoảng thời gian và xuất ảnh/Excel.

Yêu cầu phi chức năng:
- Tải nhanh (<1s) với dữ liệu tóm tắt (sử dụng cache hoặc truy vấn đã tổng hợp).
- Phân quyền: chỉ hiển thị các chỉ số và nút tương ứng với role (ví dụ: nhân viên bán hàng không thấy nút cấu hình hệ thống).

Pre/Post conditions:
- Pre: người dùng đã xác thực; DB có dữ liệu giao dịch.
- Post: chọn một widget/ảnh hưởng điều hướng tới form chi tiết.

---

## 2.3.3. Form bán hàng

Mục đích: thực hiện toàn bộ luồng lập hóa đơn bán lẻ/đơn hàng, tính giá, áp dụng khuyến mãi và hoàn tất thanh toán.

Chức năng chi tiết:
- Header nhập thông tin chung: chọn/tra cứu khách hàng (hỗ trợ tìm theo tên, mã, SĐT), chọn nhân viên bán, kênh bán (quầy, online), ngày giờ và ghi chú hóa đơn.
- Danh sách giỏ hàng (table): mỗi dòng gồm Mã SP, Tên SP, Đơn vị, Giá bán đơn vị (tự động lấy theo giá hiện tại), Số lượng (có spinner để tăng/giảm), Thành tiền, và nút Xóa dòng. Khi thay đổi số lượng, hệ thống kiểm tra tồn kho tức thì và hiển thị cảnh báo nếu vượt tồn.
- Nút thêm sản phẩm nhanh: hỗ trợ quét mã vạch, gợi ý từ khóa, kéo-thả từ danh sách sản phẩm.
- Khuyến mãi: ô nhập mã khuyến mãi, nút kiểm tra; sau khi kiểm tra, hiển thị thông tin chương trình (tên, điều kiện, mô tả) và số tiền giảm hiển thị rõ ràng; hỗ trợ chọn nhiều mã nếu hệ thống cho phép kết hợp (tùy cấu hình).
- Tính toán hợp lệ: hiển thị subtotal, tổng giảm, VAT (có checkbox để áp VAT), phí khác (nếu có), và tổng thanh toán; cập nhật tức thì khi thay đổi giỏ hàng hoặc mã KM.
- Thanh toán: các phương thức thanh toán (tiền mặt, chuyển khoản, thẻ), nhập số tiền khách đưa, tính tiền thối; nút "Thanh toán & Lưu" sẽ thực hiện các bước kiểm tra: (1) validate dữ liệu, (2) gọi BLL `ValidateAndCalculate` cho KM, (3) gọi `CreateInvoice` để lưu hóa đơn và chi tiết, (4) gọi `RecordUsage` nếu áp mã, (5) cập nhật tồn kho.
- Xác nhận và in hóa đơn: sau khi lưu thành công, hiển thị thông báo, cho phép in hóa đơn (template), gửi email hoặc lưu PDF.

Trường hợp ngoại lệ và xử lý:
- Nếu tồn kho không đủ: hiển thị modal cảnh báo, cho phép điều chỉnh hoặc đặt hàng trước.
- Nếu mã khuyến mãi không hợp lệ: hiển thị nguyên nhân (hết hạn, không đủ ngưỡng, không áp cho sản phẩm), không cho phép lưu hóa đơn với mã đó.
- Nếu lưu hóa đơn thất bại do lỗi DB: rollback và hiển thị lỗi chi tiết cho admin.

Yêu cầu phi chức năng:
- Độ trễ phản hồi cho thao tác tính toán <200ms trong điều kiện dữ liệu bình thường.
- Giao diện thân thiện để nhân viên thao tác bằng bàn phím nhiều hơn chuột (shortcut key).

Pre/Post:
- Pre: tài khoản có quyền bán hàng; tồn kho và thông tin sản phẩm có sẵn.
- Post: hóa đơn được lưu, tồn kho cập nhật, log giao dịch ghi nhận.

---

## 2.3.4. Form sản phẩm

Mục đích: quản lý danh mục hàng hóa chi tiết, hỗ trợ nghiệp vụ nhập mới, điều chỉnh giá và cập nhật tồn kho.

Chức năng chi tiết:
- Danh sách sản phẩm: bảng có cột Mã, Tên, Danh mục, Nhà cung cấp, Giá bán, Giá nhập, Số lượng tồn, Trạng thái; hỗ trợ sắp xếp, lọc theo cột, và phân trang (paging) để tối ưu hiệu năng.
- Form chi tiết sản phẩm (panel hoặc modal): các trường editable gồm Mã SP (khóa), Tên SP, Danh mục (dropdown), Nhà cung cấp (dropdown), Đơn vị tính, Giá bán, Giá nhập, Số lượng ban đầu, Ảnh sản phẩm (upload), Mô tả, Trạng thái (active/inactive).
- Import/Export: chức năng nhập khẩu bằng file CSV/Excel với mapping trường, kiểm tra dữ liệu trước khi lưu (validation report), và xuất danh sách ra Excel/PDF.
- Lịch sử thay đổi: mỗi sản phẩm có liên kết đến lịch sử chỉnh sửa (ai sửa, lúc nào, trường gì thay đổi), xem nhanh trong modal.
- Hành động nhanh: nút "Nhập kho" (mở form nhập kho), "Điều chỉnh giá", "Kích hoạt/Ngưng bán".

Trường hợp đặc biệt và kiểm tra:
- Khi thay đổi Mã SP: nếu cho phép, phải kiểm tra không trùng lặp; tốt nhất khóa Mã SP sau khi tạo.
- Khi giảm tồn (bằng điều chỉnh thủ công): cần ghi lý do và người thực hiện.
- Kiểm tra ràng buộc số: `UnitPrice` và `PurchasePrice` >= 0; `Quantity` là số nguyên >= 0.

Yêu cầu phi chức năng:
- Hiển thị tối đa 50 hàng/trang mặc định; hỗ trợ lazy-loading cho danh sách lớn.

Pre/Post:
- Pre: quyền quản lý sản phẩm.
- Post: bản ghi sản phẩm được tạo/sửa/xóa; nếu xóa mà tồn >0 thì cảnh báo hoặc không cho xóa.

---

## 2.3.5. Form danh mục

Mục đích: quản lý cấu trúc phân loại sản phẩm để hỗ trợ lọc, báo cáo và điều hướng trong UI.

Chức năng chi tiết:
- Cây danh mục / danh sách: cho phép xem cấu trúc phân cấp (nếu có), sắp xếp kéo-thả để thay đổi thứ tự/nhóm cha-con.
- Các thao tác CRUD: thêm danh mục con, sửa tên, mô tả, xóa (với kiểm tra nếu còn sản phẩm thuộc danh mục thì cảnh báo hoặc yêu cầu chuyển sản phẩm sang danh mục khác trước khi xóa).
- Gán/huỷ gán sản phẩm hàng loạt theo danh mục: chọn nhiều sản phẩm và chuyển qua danh mục khác.
- Truy vấn liên quan: hiển thị số lượng sản phẩm hiện có trong danh mục và thống kê cơ bản (tổng tồn, doanh thu theo danh mục trong khoảng thời gian xác định).

Ràng buộc và xử lý ngoại lệ:
- Không cho tạo hai danh mục cùng tên trên cùng một mức cha.
- Khi xóa danh mục chứa danh mục con: yêu cầu xác nhận và mô tả hành động (xóa cùng/dời con).

Pre/Post:
- Pre: quyền quản trị/danh mục.
- Post: cấu trúc danh mục cập nhật, sản phẩm liên kết thay đổi nếu cần.

---

## 2.3.6. Form nhà cung cấp

Mục đích: quản lý thông tin nhà cung cấp để phục vụ nhập kho và theo dõi nguồn hàng.

Chức năng chi tiết:
- Danh sách NCC: bảng hiển thị mã, tên, liên hệ, email, địa chỉ, trạng thái, số sản phẩm cung cấp; hỗ trợ tìm kiếm theo tên hoặc mã.
- Form chi tiết NCC: nhập/sửa thông tin liên hệ, điều khoản thanh toán, thời gian giao hàng dự kiến, điểm uy tín (rating) nếu có.
- Lịch sử giao dịch/đơn hàng: xem các phiếu nhập liên quan đến NCC, thời gian giao hàng, tình trạng thanh toán để làm cơ sở đánh giá NCC.
- Hỗ trợ import danh sách NCC từ file và xuất báo cáo danh sách NCC.

Ràng buộc và lưu ý:
- Khi xóa NCC: nếu còn sản phẩm tham chiếu hoặc đơn hàng chưa hoàn tất, cần cảnh báo và không cho xóa trực tiếp.
- Hỗ trợ ghi chú nội bộ (ví dụ điều kiện ưu đãi, hạn mức tín dụng) chỉ hiển thị cho quản lý.

Pre/Post:
- Pre: quyền quản lý NCC.
- Post: NCC tạo/sửa/xóa ảnh hưởng tới form nhập kho và báo cáo.

---

## 2.3.7. Form khuyến mãi

Mục đích: tạo, quản lý và theo dõi các chương trình khuyến mãi áp dụng cho hóa đơn hoặc sản phẩm cụ thể.

Chức năng chi tiết:
- Danh sách chương trình: xem tóm tắt mã, tên, loại giảm (PT/tiền), ngưỡng, ngày hiệu lực, trạng thái, lượt đã dùng/ giới hạn.
- Form chi tiết KM: nhập mã (unique), tên, mô tả, loại giảm (Percentage/Fixed), giá trị giảm, ngưỡng tối thiểu, tối đa giảm (cap), ngày bắt đầu/ kết thúc, phạm vi áp dụng (toàn bộ hoặc chọn sản phẩm/danh mục), giới hạn lượt sử dụng và các điều kiện nâng cao (chỉ áp cho khách VIP, chỉ áp vào giờ hành chính, v.v.).
- Công cụ kiểm thử mã: ô nhập mã và nút "Kiểm tra" để mô phỏng áp mã cho một subtotal giả lập, trả về `PromotionDiscountResult` với thông báo chi tiết (hợp lệ, lỗi do ngày, lỗi do ngưỡng, không áp với sản phẩm này, v.v.).
- Nút kích hoạt/tạm dừng/xóa chương trình; nhật ký sử dụng thể hiện danh sách hóa đơn đã áp mã.

Ràng buộc và xử lý:
- Mã phải unique; ngày bắt đầu phải <= ngày kết thúc; phần trăm giảm không vượt 100.
- Khi sửa chương trình đang có hóa đơn áp dụng: cần ghi chú lịch sử chỉnh sửa hoặc khóa sửa đổi các trường ảnh hưởng đến các hóa đơn cũ.

Pre/Post:
- Pre: quyền quản lý KM.
- Post: chương trình lưu và có thể được áp dụng trong `Form bán hàng`.

---

## 2.3.8. Form báo cáo

Mục đích: cung cấp các báo cáo tổng hợp và chi tiết phục vụ quản lý bán hàng, tồn kho và tài chính.

Chức năng chi tiết:
- Loại báo cáo: doanh thu theo ngày/tuần/tháng, báo cáo sản phẩm bán chạy, báo cáo tồn kho (tồn tối thiểu/tồn quá nhiều), báo cáo hiệu quả khuyến mãi (số đơn áp mã, doanh thu giảm), báo cáo công nợ (nếu có tính công nợ khách hàng).
- Bộ lọc: ngày bắt đầu/kết thúc, danh mục/sản phẩm, nhân viên, cửa hàng/kênh, trạng thái hóa đơn.
- Hiển thị: bảng dữ liệu, tổng hợp (sum, avg), biểu đồ tương tác, khả năng drill-down (nhấp vào dòng để xem hóa đơn liên quan).
- Xuất: hỗ trợ xuất CSV/Excel/PDF, lập lịch gửi báo cáo định kỳ qua email.

Yêu cầu phi chức năng:
- Hỗ trợ paging và truy vấn theo batch cho dữ liệu lớn; báo cáo nặng sẽ chạy background job và gửi thông báo khi hoàn tất.

Pre/Post:
- Pre: quyền xem báo cáo.
- Post: báo cáo hiển thị/được xuất/đăng ký gửi tự động.

---

## 2.3.9. Form lịch sử tồn kho

Mục đích: theo dõi chi tiết biến động tồn kho để giám sát nhập/xuất, điều chỉnh và phát hiện chênh lệch.

Chức năng chi tiết:
- Bảng biến động: mỗi dòng gồm Ngày giờ, Loại giao dịch (Nhập/Xuất/Điều chỉnh), Mã sản phẩm, Tên, Số lượng trước/sau, Người thực hiện, Ghi chú/ly do.
- Bộ lọc mạnh: lọc theo sản phẩm, khoảng thời gian, loại giao dịch, nhân viên, kho (nếu có nhiều kho).
- Truy vết giao dịch: từ một dòng lịch sử có thể điều hướng tới hóa đơn, phiếu nhập hoặc bản ghi điều chỉnh để xem ngữ cảnh.
- Công cụ đối chiếu tồn: cho phép chọn hai mốc thời gian và so sánh tồn, phát hiện sai khác và tạo báo cáo kiểm kê.

Xử lý ngoại lệ:
- Nếu lịch sử bị thiếu (do nhập thủ công trước đây), cung cấp công cụ ghi chú/điều chỉnh có xác nhận và lý do.

Pre/Post:
- Pre: quyền xem/chỉnh sửa lịch sử theo vai trò.
- Post: ghi nhận nhật ký, cập nhật tồn kho nếu có thao tác điều chỉnh.

---

## 2.3.10. Form nhật ký hệ thống

Mục đích: cung cấp bảng ghi audit cho mọi sự kiện quan trọng để phục vụ kiểm toán và xử lý sự cố.

Chức năng chi tiết:
- Log event: ghi nhận đăng nhập/đăng xuất, thay đổi bản ghi (tạo/sửa/xóa), thao tác cấu hình, backup/restore, lỗi hệ thống quan trọng.
- Trường log: Timestamp, User, Module, Action, TargetId (ví dụ: ProductId), OldValue (nếu có), NewValue (nếu có), IP/Workstation.
- Tìm kiếm & lọc: theo user, module, action, khoảng thời gian và mức độ (info/warn/error).
- Export và xóa log theo chính sách lưu trữ: cho phép export log cho mục đích báo cáo/hỗ trợ, và xóa theo chính sách (ví dụ giữ 1 năm) có audit trail.

Bảo mật & quy định:
- Chỉ admin được xem toàn bộ log; người dùng khác chỉ thấy log liên quan đến hoạt động của mình.
- Log nhạy cảm (mật khẩu, token) không lưu nguyên văn.

Pre/Post:
- Pre: quyền admin hoặc quyền xem log.
- Post: log được tạo/ xuất/ xóa theo chính sách.

---

## 2.3.11. Form nhân viên

Mục đích: quản lý hồ sơ nhân viên và quyền truy cập hệ thống.

Chức năng chi tiết:
- Danh sách nhân viên: bảng hiển thị tên, mã NV, vai trò, trạng thái, email, số điện thoại; hỗ trợ tìm kiếm và phân trang.
- Form chi tiết: thêm/sửa thông tin cá nhân, phân quyền (role-based), phân công cửa hàng/kênh, trạng thái làm việc (active/locked), ghi chú nội bộ.
- Bảo mật: reset mật khẩu, tắt/mở tài khoản, cấu hình quyền chi tiết (CRUD trên module), xác thực hai yếu tố nếu được tích hợp.
- Lịch sử hoạt động: xem lịch sử đăng nhập, các hành động quan trọng do nhân viên thực hiện.

Ràng buộc:
- Không cho phép xóa tài khoản đã dùng để tạo dữ liệu quan trọng; thay vào đó khóa tài khoản và gán người thay thế.

Pre/Post:
- Pre: quyền quản trị nhân sự/IT.
- Post: thông tin nhân viên thay đổi ảnh hưởng tới phân quyền và audit log.

---

## 2.3.12. Form sao lưu / khôi phục

Mục đích: bảo đảm an toàn dữ liệu thông qua chức năng sao lưu và khả năng khôi phục khi cần.

Chức năng chi tiết:
- Tạo backup thủ công: nút "Backup ngay" tạo file backup chứa dữ liệu, metadata (người thực hiện, mô tả) và checksum.
- Lập lịch backup: giao diện cho phép cấu hình lịch (hằng ngày/tuần/tháng), lưu vị trí backup (local/đĩa mạng/FTP/S3), và số bản giữ lại.
- Danh sách backup: hiển thị các bản backup có thông tin thời gian, kích thước, người tạo và mô tả; hỗ trợ tìm kiếm và phân trang.
- Khôi phục: chọn bản backup và thực hiện restore với bước xác nhận nhiều tầng (warning, yêu cầu mật khẩu admin, backup hiện tại trước khi restore); kiểm tra checksum trước khi restore.
- Kiểm tra integrity: công cụ kiểm tra nhanh tính toàn vẹn của bản backup và mô tả kết quả.

Bảo mật và lưu ý:
- Quyền thực hiện backup/restore giới hạn cho admin; thao tác restore nên được ghi vào nhật ký hệ thống.
- Khuyến nghị thực hiện backup trước khi nâng cấp hệ thống hoặc thực hiện thay đổi lớn.

Pre/Post:
- Pre: quyền admin và không có giao dịch đang chạy nặng.
- Post: file backup được tạo; restore cập nhật DB theo trạng thái của bản backup và ghi log hành động.

---

### Kết luận

Các mô tả trên đã mở rộng chi tiết nghiệp vụ, ràng buộc, ngoại lệ và yêu cầu phi chức năng cho từng form. Nếu bạn muốn, tôi sẽ:
- Chèn các mô tả này vào `file/tieu_luan_ltnc_2026.md` tại vị trí tương ứng, hoặc
- Rút gọn/định dạng lại để phù hợp với phần in ấn trong tiểu luận, hoặc
- Chuyển thành slide hướng dẫn sử dụng.
