-- ============================================================
-- CƠ SỞ DỮ LIỆU: CỬA HÀNG THIẾT BỊ TRƯỜNG HỌC
-- ============================================================

IF DB_ID(N'QuanLyThietBiTruongHoc') IS NULL
    CREATE DATABASE QuanLyTBTH;
GO

USE QuanLyTBTH;
GO

-- XÓA BẢNG CŨ
DROP TABLE IF EXISTS HoaDon_KhuyenMai;
DROP TABLE IF EXISTS CaiDat;
DROP TABLE IF EXISTS KhuyenMai;
DROP TABLE IF EXISTS NhatKyHeThong;
DROP TABLE IF EXISTS NhatKyKho;
DROP TABLE IF EXISTS ChiTietHoaDon;
DROP TABLE IF EXISTS HoaDon;
DROP TABLE IF EXISTS ChiTietPhieuNhap;
DROP TABLE IF EXISTS PhieuNhap;
DROP TABLE IF EXISTS KhachHang;
DROP TABLE IF EXISTS SanPham;
DROP TABLE IF EXISTS HangSanXuat;
DROP TABLE IF EXISTS NhaCungCap;
DROP TABLE IF EXISTS DanhMuc;
DROP TABLE IF EXISTS NhanVien;
DROP TABLE IF EXISTS VaiTro;
GO

-- ============================================================
-- VAI TRÒ
-- ============================================================
CREATE TABLE VaiTro (
    MaVaiTro INT IDENTITY PRIMARY KEY,
    TenVaiTro NVARCHAR(50) UNIQUE
);

-- ============================================================
-- NHÂN VIÊN
-- ============================================================
CREATE TABLE NhanVien (
    MaNhanVien INT IDENTITY PRIMARY KEY,
    TenDangNhap NVARCHAR(50) UNIQUE,
    MatKhauHash VARBINARY(256),
    MatKhauSalt VARBINARY(128),
    HoTen NVARCHAR(150),
    MaVaiTro INT,
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1,
    FOREIGN KEY (MaVaiTro) REFERENCES VaiTro(MaVaiTro)
);

-- ============================================================
-- DANH MỤC
-- ============================================================
CREATE TABLE DanhMuc (
    MaDanhMuc INT IDENTITY PRIMARY KEY,
    TenDanhMuc NVARCHAR(150)
);

-- ============================================================
-- NHÀ CUNG CẤP
-- ============================================================
CREATE TABLE NhaCungCap (
    MaNhaCungCap INT IDENTITY PRIMARY KEY,
    TenNhaCungCap NVARCHAR(200)
);

-- ============================================================
-- HÃNG SẢN XUẤT
-- ============================================================
CREATE TABLE HangSanXuat (
    MaHang INT IDENTITY PRIMARY KEY,
    TenHang NVARCHAR(200)
);

-- ============================================================
-- SẢN PHẨM
-- ============================================================
CREATE TABLE SanPham (
    MaSanPham INT IDENTITY PRIMARY KEY,
    MaSanPhamCode NVARCHAR(50) UNIQUE,
    TenSanPham NVARCHAR(250),
    MaDanhMuc INT,
    MaHang INT,
    MaNhaCungCap INT,
    SoLuong INT DEFAULT 0,
    GiaBan DECIMAL(18,2),
    FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc),
    FOREIGN KEY (MaHang) REFERENCES HangSanXuat(MaHang),
    FOREIGN KEY (MaNhaCungCap) REFERENCES NhaCungCap(MaNhaCungCap)
);

-- ============================================================
-- KHÁCH HÀNG
-- ============================================================
CREATE TABLE KhachHang (
    MaKhachHang INT IDENTITY PRIMARY KEY,
    MaKhach NVARCHAR(50),
    TenKhach NVARCHAR(200),
    DienThoai NVARCHAR(20),
    DiaChi NVARCHAR(300)
);

-- ============================================================
-- PHIẾU NHẬP
-- ============================================================
CREATE TABLE PhieuNhap (
    MaPhieuNhap INT IDENTITY PRIMARY KEY,
    NgayNhap DATETIME DEFAULT GETDATE(),
    MaNhanVien INT,
    MaNhaCungCap INT,
    TongTien DECIMAL(18,2),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    FOREIGN KEY (MaNhaCungCap) REFERENCES NhaCungCap(MaNhaCungCap)
);

CREATE TABLE ChiTietPhieuNhap (
    MaChiTiet INT IDENTITY PRIMARY KEY,
    MaPhieuNhap INT,
    MaSanPham INT,
    SoLuong INT,
    GiaNhap DECIMAL(18,2),
    FOREIGN KEY (MaPhieuNhap) REFERENCES PhieuNhap(MaPhieuNhap),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

-- ============================================================
-- HÓA ĐƠN (KHÔNG CÒN CỘT KHUYẾN MÃI SAI)
-- ============================================================
CREATE TABLE HoaDon (
    MaHoaDon INT IDENTITY PRIMARY KEY,
    NgayBan DATETIME DEFAULT GETDATE(),
    MaNhanVien INT,
    MaKhachHang INT,
    TongTien DECIMAL(18,2),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang)
);

CREATE TABLE ChiTietHoaDon (
    MaChiTiet INT IDENTITY PRIMARY KEY,
    MaHoaDon INT,
    MaSanPham INT,
    SoLuong INT,
    GiaBan DECIMAL(18,2),
    FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

-- ============================================================
-- 🎁 KHUYẾN MÃI (FULL TIẾNG VIỆT)
-- ============================================================
CREATE TABLE KhuyenMai (
    MaKhuyenMai INT IDENTITY PRIMARY KEY,
    MaCode NVARCHAR(50) UNIQUE,
    TenKhuyenMai NVARCHAR(200),
    MoTa NVARCHAR(500),

    LoaiGiam NVARCHAR(50), -- PhanTram / TienMat
    GiaTriGiam DECIMAL(18,2),

    DonHangToiThieu DECIMAL(18,2),
    GiamToiDa DECIMAL(18,2),

    NgayBatDau DATETIME,
    NgayKetThuc DATETIME,

    GioiHanSuDung INT,
    SoLanDaDung INT DEFAULT 0,

    TrangThai BIT DEFAULT 1
);

-- ============================================================
-- 🔗 LIÊN KẾT HÓA ĐƠN - KHUYẾN MÃI (QUAN TRỌNG)
-- ============================================================
CREATE TABLE HoaDon_KhuyenMai (
    MaLienKet INT IDENTITY PRIMARY KEY,
    MaHoaDon INT,
    MaKhuyenMai INT,
    SoTienGiam DECIMAL(18,2),

    FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon),
    FOREIGN KEY (MaKhuyenMai) REFERENCES KhuyenMai(MaKhuyenMai)
);

-- ============================================================
-- NHẬT KÝ KHO
-- ============================================================
CREATE TABLE NhatKyKho (
    MaLog INT IDENTITY PRIMARY KEY,
    MaSanPham INT,
    SoLuongThayDoi INT,
    Loai NVARCHAR(50), -- Nhap / Ban
    ThoiGian DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

-- ============================================================
-- NHẬT KÝ HỆ THỐNG
-- ============================================================
CREATE TABLE NhatKyHeThong (
    MaLog INT IDENTITY PRIMARY KEY,
    MaNhanVien INT,
    HanhDong NVARCHAR(200),
    ThoiGian DATETIME DEFAULT GETDATE()
);

-- ============================================================
-- CÀI ĐẶT
-- ============================================================
CREATE TABLE CaiDat (
    MaCaiDat INT IDENTITY PRIMARY KEY,
    Ten NVARCHAR(100),
    GiaTri NVARCHAR(500)
);