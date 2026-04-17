# TÀI LIỆU MÔ TẢ CHỨC NĂNG HỆ THỐNG

## 1. Giới thiệu

Tài liệu này mô tả các chức năng chính của hệ thống quản lý sản phẩm, tài khoản và phân quyền người dùng.  
Hệ thống hỗ trợ các nhóm chức năng sau:

- Đăng nhập
- Quản lý tài khoản
- Phân quyền user
- Quản lý sản phẩm

---

## 2. Chức năng đăng nhập

### 2.1. Mục đích
Cho phép người dùng truy cập vào hệ thống bằng tài khoản đã được cấp.

### 2.2. Mô tả chức năng
Người dùng nhập:

- Tên đăng nhập hoặc email
- Mật khẩu

Hệ thống sẽ kiểm tra thông tin đăng nhập trong cơ sở dữ liệu.

### 2.3. Kết quả xử lý
- Nếu đúng thông tin: đăng nhập thành công và chuyển vào trang chính
- Nếu sai thông tin: hiển thị thông báo lỗi
- Nếu tài khoản bị khóa hoặc ngưng hoạt động: không cho phép đăng nhập

### 2.4. Ràng buộc
- Tên đăng nhập không được để trống
- Mật khẩu không được để trống
- Mật khẩu cần được mã hóa khi lưu trong cơ sở dữ liệu

### 2.5. Dữ liệu liên quan
Bảng sử dụng:
- `users`

---

## 3. Chức năng quản lý tài khoản

### 3.1. Mục đích
Cho phép quản trị viên quản lý thông tin tài khoản người dùng trong hệ thống.

### 3.2. Các chức năng con
- Thêm tài khoản
- Sửa tài khoản
- Xóa tài khoản
- Xem danh sách tài khoản
- Xem chi tiết tài khoản
- Khóa / mở khóa tài khoản

### 3.3. Thông tin tài khoản bao gồm
- Mã tài khoản
- Tên đăng nhập
- Họ tên
- Email
- Số điện thoại
- Mật khẩu
- Trạng thái
- Vai trò / nhóm quyền

### 3.4. Quy tắc xử lý
- Tên đăng nhập là duy nhất
- Email là duy nhất
- Không cho phép xóa tài khoản quản trị chính
- Mật khẩu phải được mã hóa
- Chỉ người có quyền quản trị mới được thêm, sửa, xóa tài khoản

### 3.5. Dữ liệu liên quan
Bảng sử dụng:
- `users`
- `roles`
- `user_roles`

---

## 4. Chức năng phân quyền user

### 4.1. Mục đích
Cho phép hệ thống kiểm soát quyền truy cập của từng người dùng theo vai trò.

### 4.2. Mô hình phân quyền
Hệ thống áp dụng phân quyền theo vai trò (Role-Based Access Control - RBAC).

### 4.3. Các vai trò đề xuất
- `ADMIN`: toàn quyền hệ thống
- `MANAGER`: quản lý dữ liệu nghiệp vụ
- `STAFF`: thao tác dữ liệu được phân công
- `VIEWER`: chỉ được xem dữ liệu

### 4.4. Quyền hạn ví dụ

| Vai trò | Đăng nhập | Quản lý tài khoản | Phân quyền | Sản phẩm |
|--------|-----------|-------------------|------------|----------|
| ADMIN | Có | Có | Có | Toàn quyền |
| MANAGER | Có | Không / hạn chế | Không | Thêm, sửa, xem, tìm kiếm |
| STAFF | Có | Không | Không | Xem, tìm kiếm, có thể thêm tùy cấu hình |
| VIEWER | Có | Không | Không | Chỉ xem, tìm kiếm |

### 4.5. Chức năng phân quyền
- Gán vai trò cho người dùng
- Cập nhật vai trò
- Thu hồi vai trò
- Kiểm tra quyền trước khi truy cập chức năng

### 4.6. Quy tắc xử lý
- Một user có thể có một hoặc nhiều vai trò
- Chỉ `ADMIN` mới được phân quyền
- Khi user đăng nhập, hệ thống nạp danh sách quyền tương ứng

### 4.7. Dữ liệu liên quan
Bảng sử dụng:
- `users`
- `roles`
- `permissions`
- `role_permissions`
- `user_roles`

---

## 5. Chức năng quản lý sản phẩm (*)

### 5.1. Mục đích 
Cho phép người dùng quản lý thông tin sản phẩm trong hệ thống.

### 5.2. Các chức năng con
- Thêm sản phẩm
- Sửa sản phẩm
- Xóa sản phẩm
- Xem chi tiết sản phẩm
- Tìm kiếm sản phẩm

---

## 6. Thêm sản phẩm (*)

### 6.1. Mô tả
Người dùng nhập thông tin sản phẩm mới vào hệ thống.

### 6.2. Thông tin sản phẩm
- Mã sản phẩm
- Tên sản phẩm
- Mô tả
- Nhóm sản phẩm
- Đơn vị tính
- Giá
- Số lượng tồn
- Ảnh sản phẩm
- Trạng thái

### 6.3. Quy tắc xử lý
- Mã sản phẩm là duy nhất
- Tên sản phẩm không được để trống
- Giá phải lớn hơn hoặc bằng 0
- Số lượng tồn phải lớn hơn hoặc bằng 0

### 6.4. Dữ liệu liên quan
Bảng sử dụng:
- `products`
- `product_images`

---

## 7. Sửa sản phẩm (*)

### 7.1. Mô tả
Cho phép cập nhật thông tin của sản phẩm đã tồn tại.

### 7.2. Nội dung có thể sửa
- Tên sản phẩm
- Mô tả
- Giá
- Số lượng tồn
- Ảnh sản phẩm
- Trạng thái

### 7.3. Quy tắc xử lý
- Không được sửa mã sản phẩm nếu hệ thống quy định mã là cố định
- Kiểm tra dữ liệu hợp lệ trước khi lưu
- Ghi nhận thời gian cập nhật

---

## 8. Xóa sản phẩm (*)

### 8.1. Mô tả
Cho phép loại bỏ sản phẩm khỏi danh sách quản lý.

### 8.2. Hình thức xóa
Khuyến nghị sử dụng **xóa mềm**:
- Đánh dấu sản phẩm là ngưng hoạt động
- Không xóa vật lý khỏi cơ sở dữ liệu

### 8.3. Quy tắc xử lý
- Không cho phép xóa sản phẩm đã phát sinh giao dịch nếu dùng xóa cứng
- Chỉ người có quyền mới được xóa

---

## 9. Xem chi tiết sản phẩm (*)

### 9.1. Mô tả
Hiển thị đầy đủ thông tin của một sản phẩm.

### 9.2. Thông tin hiển thị
- Mã sản phẩm
- Tên sản phẩm
- Mô tả
- Danh mục
- Giá
- Số lượng tồn
- Ảnh đại diện
- Danh sách ảnh chi tiết
- Trạng thái
- Ngày tạo
- Ngày cập nhật

### 9.3. Dữ liệu liên quan
Bảng sử dụng:
- `products`
- `product_images`

---

## 10. Tìm kiếm sản phẩm (*)

### 10.1. Mô tả
Cho phép người dùng tra cứu sản phẩm theo nhiều tiêu chí.

### 10.2. Tiêu chí tìm kiếm
- Mã sản phẩm
- Tên sản phẩm
- Nhóm sản phẩm
- Trạng thái
- Khoảng giá

### 10.3. Kết quả
Hiển thị danh sách sản phẩm phù hợp với điều kiện tìm kiếm.

### 10.4. Yêu cầu
- Tìm kiếm nhanh
- Có thể kết hợp nhiều điều kiện
- Hỗ trợ phân trang khi số lượng dữ liệu lớn

---

## 11. Phân quyền theo chức năng sản phẩm

| Chức năng | ADMIN | MANAGER | STAFF | VIEWER |
|----------|-------|---------|-------|--------|
| Thêm sản phẩm | Có | Có | Tùy quyền | Không |
| Sửa sản phẩm | Có | Có | Tùy quyền | Không |
| Xóa sản phẩm | Có | Có | Không | Không |
| Xem chi tiết | Có | Có | Có | Có |
| Tìm kiếm | Có | Có | Có | Có |

---

## 12. Các bảng cơ sở dữ liệu liên quan

### 12.1. `users`
Lưu thông tin người dùng

### 12.2. `roles`
Lưu danh sách vai trò

### 12.3. `permissions`
Lưu danh sách quyền

### 12.4. `user_roles`
Liên kết user và vai trò

### 12.5. `role_permissions`
Liên kết vai trò và quyền

### 12.6. `products`
Lưu thông tin sản phẩm

### 12.7. `product_images`
Lưu ảnh sản phẩm

---

## 13. Kết luận

Hệ thống gồm 4 nhóm chức năng chính:

- Đăng nhập để xác thực người dùng
- Quản lý tài khoản để quản trị user
- Phân quyền để kiểm soát truy cập
- Quản lý sản phẩm để hỗ trợ nghiệp vụ chính

Thiết kế này giúp hệ thống:
- An toàn
- Dễ quản lý
- Dễ mở rộng
- Phù hợp với mô hình CRUD thực tế





CÁC CHỨC NĂNG ĐÁNH (*) Ở ĐỀ MỤC SẼ CÓ BẢN DEMO DÙNG THỬ VÀO 13/04/2026