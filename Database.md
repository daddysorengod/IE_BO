# 📦 EXPORT ORDER MANAGEMENT DATABASE (FULL SCHEMA)

## 1. Overview

Hệ thống cơ sở dữ liệu này phục vụ quản lý quy trình xuất khẩu:

* Quản lý đối tác (Partners)
* Quản lý sản phẩm (Products + Images)
* Đơn hàng (Sales Orders)
* Hợp đồng (Contracts)
* Proforma Invoice
* Commercial Invoice
* Packing List
* Thanh toán (Payments)

Hỗ trợ đầy đủ CRUD:

* Create (Thêm)
* Read (Xem)
* Update (Sửa)
* Delete (Xóa mềm)

---

## 2. Database Tables

---

# 🔹 2.1 partners

Lưu thông tin khách hàng, nhà cung cấp, hãng tàu

| Column       | Type        | Description                    |
| ------------ | ----------- | ------------------------------ |
| partner_id   | BIGINT (PK) | ID                             |
| partner_code | VARCHAR     | Mã                             |
| partner_name | VARCHAR     | Tên                            |
| partner_type | VARCHAR     | CUSTOMER / SUPPLIER / SHIPPING |
| address      | TEXT        | Địa chỉ                        |
| phone        | VARCHAR     | SĐT                            |
| email        | VARCHAR     | Email                          |
| is_active    | BOOLEAN     | Trạng thái                     |

---

# 🔹 2.2 products

Danh mục sản phẩm

| Column          | Type        |
| --------------- | ----------- |
| product_id      | BIGINT (PK) |
| product_code    | VARCHAR     |
| product_name    | VARCHAR     |
| hs_code         | VARCHAR     |
| unit_of_measure | VARCHAR     |

---

# 🔹 2.3 product_images

Lưu ảnh sản phẩm (URL)

| Column           | Type        |
| ---------------- | ----------- |
| product_image_id | BIGINT (PK) |
| product_id       | BIGINT (FK) |
| image_url        | TEXT        |
| is_main          | BOOLEAN     |
| sort_order       | INT         |

---

# 🔹 2.4 sales_orders

Đơn hàng

| Column            | Type        |
| ----------------- | ----------- |
| sales_order_id    | BIGINT (PK) |
| order_no          | VARCHAR     |
| order_date        | DATE        |
| buyer_id          | BIGINT (FK) |
| seller_id         | BIGINT (FK) |
| delivery_location | VARCHAR     |
| status            | VARCHAR     |
| deleted_flag      | BOOLEAN     |

---

# 🔹 2.5 sales_order_items

Chi tiết đơn hàng

| Column              | Type        |
| ------------------- | ----------- |
| sales_order_item_id | BIGINT (PK) |
| sales_order_id      | BIGINT (FK) |
| product_id          | BIGINT (FK) |
| ordered_qty         | DECIMAL     |
| unit_price_vnd      | DECIMAL     |
| line_amount_vnd     | DECIMAL     |

---

# 🔹 2.6 contracts

Hợp đồng

| Column             | Type        |
| ------------------ | ----------- |
| contract_id        | BIGINT (PK) |
| contract_no        | VARCHAR     |
| contract_date      | DATE        |
| sales_order_id     | BIGINT (FK) |
| incoterm           | VARCHAR     |
| payment_terms      | VARCHAR     |
| shipment_date_plan | VARCHAR     |

---

# 🔹 2.7 proforma_invoices

Proforma Invoice

| Column              | Type        |
| ------------------- | ----------- |
| proforma_invoice_id | BIGINT (PK) |
| pi_no               | VARCHAR     |
| contract_id         | BIGINT (FK) |
| pi_date             | DATE        |
| currency_code       | VARCHAR     |

---

# 🔹 2.8 proforma_invoice_items

Chi tiết PI

| Column              | Type        |
| ------------------- | ----------- |
| id                  | BIGINT (PK) |
| proforma_invoice_id | BIGINT (FK) |
| product_id          | BIGINT (FK) |
| quantity            | DECIMAL     |
| unit_price          | DECIMAL     |
| amount              | DECIMAL     |

---

# 🔹 2.9 commercial_invoices

Commercial Invoice

| Column                | Type        |
| --------------------- | ----------- |
| commercial_invoice_id | BIGINT (PK) |
| invoice_no            | VARCHAR     |
| contract_id           | BIGINT (FK) |
| total_amount          | DECIMAL     |
| deposit_amount        | DECIMAL     |
| balance_amount        | DECIMAL     |

---

# 🔹 2.10 commercial_invoice_items

Chi tiết CI

| Column                | Type        |
| --------------------- | ----------- |
| id                    | BIGINT (PK) |
| commercial_invoice_id | BIGINT (FK) |
| product_id            | BIGINT (FK) |
| quantity              | DECIMAL     |
| unit_price            | DECIMAL     |
| amount                | DECIMAL     |

---

# 🔹 2.11 packing_lists

Packing List

| Column          | Type        |
| --------------- | ----------- |
| packing_list_id | BIGINT (PK) |
| packing_list_no | VARCHAR     |
| contract_id     | BIGINT (FK) |
| container_no    | VARCHAR     |
| total_ctns      | DECIMAL     |
| total_cbm       | DECIMAL     |

---

# 🔹 2.12 packing_list_items

Chi tiết Packing

| Column          | Type        |
| --------------- | ----------- |
| id              | BIGINT (PK) |
| packing_list_id | BIGINT (FK) |
| product_id      | BIGINT (FK) |
| total_units     | DECIMAL     |
| total_ctns      | DECIMAL     |
| total_cbm       | DECIMAL     |

---

# 🔹 2.13 payments

Thanh toán

| Column                | Type        |
| --------------------- | ----------- |
| payment_id            | BIGINT (PK) |
| commercial_invoice_id | BIGINT (FK) |
| payment_date          | DATE        |
| amount                | DECIMAL     |
| payment_type          | VARCHAR     |

---

## 3. Relationships

```
partners → sales_orders → sales_order_items
products → order_items / invoice_items / packing_items
sales_orders → contracts
contracts → proforma_invoices → commercial_invoices → packing_lists
commercial_invoices → payments
products → product_images
```

---

## 4. CRUD Operations

### CREATE

* Thêm sản phẩm, đối tác, đơn hàng

### READ

* Query join giữa các bảng

### UPDATE

* Cập nhật giá, số lượng, trạng thái

### DELETE (Soft Delete)

```sql
UPDATE sales_orders
SET deleted_flag = TRUE;
```

---

## 5. Best Practices

* Dùng `deleted_flag` thay vì DELETE
* Tách bảng header + detail
* Không lưu ảnh trực tiếp trong DB
* Dùng URL cho ảnh

---

## 6. Index Suggestions

```sql
CREATE INDEX idx_order_no ON sales_orders(order_no);
CREATE INDEX idx_product_code ON products(product_code);
CREATE INDEX idx_invoice_no ON commercial_invoices(invoice_no);
```

---

## 7. Conclusion

Thiết kế này:

✔ Chuẩn hóa dữ liệu
✔ Dễ mở rộng
✔ Phù hợp thực tế xuất khẩu
✔ Tối ưu CRUD
✔ Hỗ trợ scale hệ thống

---
