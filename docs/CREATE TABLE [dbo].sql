CREATE TABLE [dbo].[Orders] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    [CustomerName] NVARCHAR(200) NOT NULL,
    [ProductName] NVARCHAR(200) NOT NULL,
    [Total] DECIMAL(18, 2) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL
);

-- Insert sample orders with various statuses
INSERT INTO [dbo].[Orders] ([CustomerName], [ProductName], [Total], [Status], [CreatedAt], [UpdatedAt])
VALUES 
    ('John Smith', 'Laptop Computer', 1299.99, 'Completed', '2025-12-10 10:30:00', '2025-12-10 14:45:00'),
    ('Sarah Johnson', 'Wireless Mouse', 45.50, 'Completed', '2025-12-11 09:15:00', '2025-12-11 11:20:00'),
    ('Michael Brown', 'USB-C Cable (5-pack)', 29.99, 'Processing', '2025-12-12 08:00:00', '2025-12-12 09:30:00'),
    ('Emily Davis', '4K Monitor', 599.99, 'Pending', '2025-12-12 12:45:00', NULL),
    ('Robert Wilson', 'Mechanical Keyboard', 129.99, 'Completed', '2025-12-10 16:20:00', '2025-12-10 18:10:00'),
    ('Jessica Martinez', 'Gaming Headset', 189.50, 'Processing', '2025-12-12 11:00:00', '2025-12-12 12:15:00'),
    ('David Anderson', 'Webcam HD 1080p', 79.99, 'Pending', '2025-12-12 13:30:00', NULL),
    ('Lisa Thompson', 'Laptop Stand', 34.99, 'Completed', '2025-12-11 14:50:00', '2025-12-11 16:30:00'),
    ('James Taylor', 'External Hard Drive 2TB', 89.99, 'Pending', '2025-12-12 15:20:00', NULL),
    ('Patricia Garcia', 'Desk Lamp LED', 44.99, 'Completed', '2025-12-09 10:00:00', '2025-12-09 12:45:00');