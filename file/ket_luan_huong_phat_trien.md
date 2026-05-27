# KẾT LUẬN

Qua việc thực hiện đề tài, nhóm đã xây dựng được một phần mềm quản lý hoạt động của trung tâm bán thiết bị trường học đạt được các mục tiêu cơ bản và góp phần hỗ trợ người quản lý trong công tác nghiệp vụ. Do hạn chế về thời gian, nguồn lực và phạm vi học phần, phiên bản hiện tại chỉ đáp ứng các yêu cầu cốt lõi nhưng đã chứng minh được tính khả thi của giải pháp.

Những kết quả chính đã đạt được:

- Thiết kế cơ sở dữ liệu trên nền tảng quan hệ đảm bảo tính khoa học, dễ quản lý và thuận lợi cho việc truy xuất báo cáo.
- Giao diện chương trình được thiết kế bằng Visual Studio (ứng dụng WinForms), chú ý đến tính thân thiện và khả năng trình bày thông tin bằng tiếng Việt.
- Hệ thống hiển thị và thao tác tất cả nội dung bằng tiếng Việt, phù hợp với người dùng địa phương.
- Đáp ứng các yêu cầu nghiệp vụ cơ bản: quản lý sản phẩm, khách hàng, khuyến mãi, lập và lưu hóa đơn, cập nhật tồn kho.
- Thực hiện được chức năng sao lưu (backup) dữ liệu và một số tiện ích trợ giúp để bảo đảm an toàn dữ liệu trong phạm vi nghiên cứu.

---

# HƯỚNG PHÁT TRIỂN

Để đưa phần mềm từ mức minh họa trong tiểu luận lên môi trường sản xuất, đề xuất các hướng phát triển sau:

- Bổ sung và hoàn thiện giao diện người dùng: cải tiến trải nghiệm, phân trang danh sách, hỗ trợ nhập liệu nhanh, và tối ưu hiển thị cho độ phân giải khác nhau.
- Triển khai cơ chế phân quyền và bảo mật: xác thực người dùng, phân quyền theo vai trò, mã hóa chuỗi kết nối và bảo mật dữ liệu nhạy cảm.
- Cải thiện xử lý đồng thời và nhất quán dữ liệu: áp dụng cơ chế locking hoặc versioning để phòng race condition khi nhiều người thao tác cùng lúc.
- Tăng cường logging và giám sát: ghi lại nhật ký hành vi, lỗi và giao dịch để dễ dàng truy vết và bảo trì.
- Mở rộng chức năng khuyến mãi: hỗ trợ quy tắc phức tạp hơn (theo nhóm khách hàng, theo giờ trong ngày, nhiều điều kiện kết hợp).
- Thêm kiểm thử tự động và quy trình CI: viết unit test và integration test cho các tầng BLL/DAL, tích hợp CI để kiểm soát chất lượng khi thay đổi mã nguồn.
- Tối ưu hiệu năng tìm kiếm và báo cáo: thêm paging, lọc theo nhiều tiêu chí, hoặc sử dụng cơ chế full-text search ở tầng cơ sở dữ liệu.
- Nâng cấp sao lưu và phục hồi: hỗ trợ lịch trình backup tự động, backup theo phiên bản và kiểm tra phục hồi định kỳ.

---

# ĐỀ XUẤT

Nếu có nguồn lực triển khai thực tế, nhóm đề xuất:

- Thử nghiệm triển khai trên một cửa hàng quy mô nhỏ để thu thập phản hồi người dùng trước khi mở rộng.
- Lập kế hoạch vận hành, đào tạo nhân viên và soạn quy trình sao lưu/khôi phục dữ liệu.
- Phân tích chi phí- lợi ích cho các tính năng mở rộng (báo cáo nâng cao, CRM, API tích hợp với hệ thống kế toán).

---

# TÀI LIỆU THAM KHẢO

STT  Tên tác giả, Tên tác phẩm, Nhà xuất bản, Năm xuất bản.

1. Nguyễn Ngọc Bình Phương, Thái Thanh Phong, “Các giải pháp lập trình C#”. NXB Giao Thông Vận Tải, 2007.
2. Phạm Công Ngô, “Lập trình C# từ cơ bản đến nâng cao”, NXB Giáo Dục, 2007.
3. Hoàng Hữu Việt, “Lập trình C# cho ứng dụng cơ sở dữ liệu”, NXB Đại học Vinh, 2009.
4. Nguyễn Trần Phương, “Giáo trình phân tích thiết kế hệ thống thông tin”, NXB Thống kê, 2010.

Các website tham khảo:

5. http://thuvien.utt.edu.vn:8080/jspui/bitstream/123456789/490/1/giaotrinh_pttkht_viet_20091_9516.pdf
6. https://vtc.edu.vn/quy-trinh-phat-trien-phan-mem


(Thêm nguồn tham khảo thực tế, báo cáo hoặc tài liệu sử dụng khi cần.)
