ERD (High level)

Entities:
- Roles (RoleId)
- Employees (EmployeeId) -> RoleId
- Categories (CategoryId)
- Suppliers (SupplierId)
- Manufacturers (ManufacturerId)
- Products (ProductId) -> CategoryId, ManufacturerId, SupplierId
- Customers (CustomerId)
- PurchaseOrders (PurchaseOrderId) -> SupplierId, CreatedBy(Employee)
- PurchaseOrderDetails -> PurchaseOrderId, ProductId
- SalesOrders (SalesOrderId) -> CustomerId, CreatedBy(Employee)
- SalesOrderDetails -> SalesOrderId, ProductId
- InventoryLogs -> ProductId, ChangedBy
- AuditLogs -> UserId
- Settings (key/value)

Notes:
- All PKs are INT IDENTITY
- FK constraints defined to ensure referential integrity
- Indexes on ProductCode and OrderDate for performance

A diagram can be exported later as PNG from a modeling tool; this file lists relationships for quick reference.
