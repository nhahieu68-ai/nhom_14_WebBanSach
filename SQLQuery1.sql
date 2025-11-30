-- ============================
-- DATABASE TRANG WEB BÁN SÁCH
-- ============================

CREATE DATABASE BookStoreDB;
GO
USE BookStoreDB;
GO

-- ============================
-- 1. ROLES & USERS
-- ============================

CREATE TABLE Roles (
    RoleID INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    UserID INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    PasswordHash NVARCHAR(255),
    Phone NVARCHAR(20),
    RoleID INT FOREIGN KEY REFERENCES Roles(RoleID),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- ============================
-- 2. AUTHORS, PUBLISHERS, CATEGORIES
-- ============================

CREATE TABLE Authors (
    AuthorID INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Publishers (
    PublisherID INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL
);

CREATE TABLE Categories (
    CategoryID INT IDENTITY PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

-- ============================
-- 3. BOOKS
-- ============================

CREATE TABLE Books (
    BookID INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    Stock INT DEFAULT 0,
    PublisherID INT FOREIGN KEY REFERENCES Publishers(PublisherID),
    PublishYear INT,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- ============================
-- 4. MANY-TO-MANY: AUTHORS & CATEGORIES
-- ============================

CREATE TABLE BookAuthors (
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    AuthorID INT FOREIGN KEY REFERENCES Authors(AuthorID),
    PRIMARY KEY (BookID, AuthorID)
);

CREATE TABLE BookCategories (
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    CategoryID INT FOREIGN KEY REFERENCES Categories(CategoryID),
    PRIMARY KEY (BookID, CategoryID)
);

-- ============================
-- 5. BOOK IMAGES
-- ============================

CREATE TABLE BookImages (
    ImageID INT IDENTITY PRIMARY KEY,
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    ImageUrl NVARCHAR(500)
);

-- ============================
-- 6. CART & CART ITEMS
-- ============================

CREATE TABLE Carts (
    CartID INT IDENTITY PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE CartItems (
    CartItemID INT IDENTITY PRIMARY KEY,
    CartID INT FOREIGN KEY REFERENCES Carts(CartID),
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    Quantity INT NOT NULL
);

-- ============================
-- 7. ORDERS & ORDER ITEMS
-- ============================

CREATE TABLE Orders (
    OrderID INT IDENTITY PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    TotalAmount DECIMAL(18,2),
    Status NVARCHAR(50) DEFAULT N'Pending',
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE OrderItems (
    OrderItemID INT IDENTITY PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    Quantity INT,
    UnitPrice DECIMAL(18,2)
);

-- ============================
-- 8. ADDRESSES
-- ============================

CREATE TABLE Addresses (
    AddressID INT IDENTITY PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    AddressLine NVARCHAR(255),
    City NVARCHAR(100),
    District NVARCHAR(100),
    Ward NVARCHAR(100),
    Phone NVARCHAR(20)
);

-- ============================
-- 9. PAYMENTS
-- ============================

CREATE TABLE Payments (
    PaymentID INT IDENTITY PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    Method NVARCHAR(50),
    Amount DECIMAL(18,2),
    PaidAt DATETIME
);

-- ============================
-- 10. COUPONS
-- ============================

CREATE TABLE Coupons (
    CouponID INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(50) UNIQUE,
    DiscountPercent INT,
    ExpiredAt DATETIME
);

-- ============================
-- 11. REVIEWS
-- ============================

CREATE TABLE Reviews (
    ReviewID INT IDENTITY PRIMARY KEY,
    BookID INT FOREIGN KEY REFERENCES Books(BookID),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- ============================
-- 12. BANNERS
-- ============================

CREATE TABLE Banners (
    BannerID INT IDENTITY PRIMARY KEY,
    ImageUrl NVARCHAR(500),
    Link NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- DONE!
