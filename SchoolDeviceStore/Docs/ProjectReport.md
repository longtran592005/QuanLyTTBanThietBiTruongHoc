# Project Report

## 1. Topic
Build a desktop application for a school equipment sales center using C# WinForms.

## 2. Objectives
- Manage products, categories, suppliers, customers, employees, and sales.
- Provide a clean admin dashboard and a fast demo flow.
- Keep the architecture maintainable and easy to extend.

## 3. Architecture
- GUI layer: WinForms screens and user interaction.
- BLL layer: validation, business rules, and orchestration.
- DAL layer: ADO.NET data access with parameterized queries.
- DTO layer: data transfer objects.

## 4. Implemented Features
- Login with default admin account.
- Product CRUD and lookup selection.
- Category CRUD.
- Supplier CRUD.
- Sales invoice creation with cart and stock update.
- SQLite backup and restore.

## 5. Database Summary
- Roles, Employees
- Categories, Suppliers, Manufacturers
- Products
- Customers
- PurchaseOrders, PurchaseOrderDetails
- SalesOrders, SalesOrderDetails
- InventoryLogs, AuditLogs, Settings

## 6. Demo Flow
1. Login as admin.
2. Open product/category/supplier screens.
3. Create a sales invoice.
4. Run backup or restore from the sidebar.

## 7. Suggested Future Work
- Reporting charts and dashboards.
- Print preview and invoice templates.
- Export Excel/PDF.
- SQLite adapter for offline demo mode.