-- Insert users
INSERT INTO Users (FullName, Email, Adress, PhoneNumber, Password)
VALUES
('Alice Johansson', 'alice.johansson@example.com', '123 Main St, Stockholm', '0701234567', 'password123'),
('Bob Eriksson', 'bob.eriksson@example.com', '456 Elm St, Göteborg', '0707654321', 'securePass1'),
('Clara Svensson', 'clara.svensson@example.com', '789 Oak St, Malmö', '0709988776', 'clara123'),
('David Andersson', 'david.andersson@example.com', '321 Birch St, Uppsala', '0701122334', 'davidsPass');

-- Insert payment methods
INSERT INTO PaymentMethods (PaymentType)
VALUES
('Swish'),
('Mastercard');

-- Link users with payment methods
INSERT INTO UserPaymentMethods (UserId, PaymentId)
VALUES
(1, 1), (1, 2),
(2, 1),
(3, 2),
(4, 1), (4, 2);

-- Insert cars
INSERT INTO Cars (PlateNumber, Model, UserId)
VALUES
('ABC123', 'Volvo XC60', 1),
('XYZ789', 'Tesla Model 3', 1),
('LMN456', 'Audi A4', 2),
('QRS234', 'BMW 5 Series', 3),
('TUV567', 'Toyota Corolla', 4),
('JKL890', 'Volkswagen Golf', 4);

-- Insert zones
INSERT INTO Zones (Fee, Adress)
VALUES
(20.00, 'Avenyn, Göteborg'),
(15.00, 'Linnégatan, Göteborg'),
(25.00, 'Vasagatan, Göteborg'),
(18.00, 'Järntorget, Göteborg'),
(22.00, 'Haga Nygata, Göteborg'),
(17.00, 'Nordhemsgatan, Göteborg'),
(19.00, 'Övre Husargatan, Göteborg'),
(16.00, 'Kungsgatan, Göteborg'),
(21.00, 'Magasinsgatan, Göteborg'),
(14.00, 'Chalmersgatan, Göteborg'),
(23.00, 'Storgatan, Göteborg'),
(20.00, 'Esperantoplatsen, Göteborg'),
(18.00, 'Kungsportsavenyen, Göteborg'),
(19.00, 'Södra Vägen, Göteborg'),
(17.00, 'Teatergatan, Göteborg'),
(20.00, 'Kristinelundsgatan, Göteborg'),
(15.00, 'Aschebergsgatan, Göteborg'),
(22.00, 'Engelbrektsgatan, Göteborg'),
(21.00, 'Viktoriagatan, Göteborg'),
(19.00, 'Första Långgatan, Göteborg');

-- Insert receipts
INSERT INTO Receipts (UserId, ZoneId, StartTime, EndTime, Amount, CarId, PaymentId)
VALUES
-- Receipts for Alice
(1, 1001, '2025-01-25 08:00:00', '2025-01-25 09:00:00', 20.00, 1, 1),
(1, 1002, '2025-01-26 10:00:00', '2025-01-26 11:30:00', 30.00, 2, 2),
(1, 1003, '2025-01-27 07:00:00', '2025-01-27 08:15:00', 25.00, 1, 1),
(1, 1004, '2025-01-27 09:00:00', '2025-01-27 10:30:00', 27.00, 1, 2),
(1, 1005, '2025-01-27 11:00:00', '2025-01-27 12:30:00', 22.00, 2, 1),
(1, 1006, '2025-01-27 13:00:00', '2025-01-27 14:15:00', 17.00, 1, 1),
(1, 1007, '2025-01-28 08:00:00', '2025-01-28 09:30:00', 19.00, 2, 2),
(1, 1008, '2025-01-28 10:00:00', '2025-01-28 11:45:00', 16.00, 1, 1),
(1, 1009, '2025-01-28 12:00:00', '2025-01-28 13:30:00', 21.00, 2, 1),
(1, 1010, '2025-01-28 14:00:00', '2025-01-28 15:15:00', 14.00, 1, 2),
-- Receipts for Bob
(2, 1011, '2025-01-24 09:00:00', '2025-01-24 10:30:00', 23.00, 3, 1),
(2, 1012, '2025-01-25 14:00:00', '2025-01-25 15:00:00', 20.00, 3, 1),
(2, 1013, '2025-01-25 16:00:00', '2025-01-25 17:15:00', 18.00, 3, 2),
(2, 1014, '2025-01-26 08:00:00', '2025-01-26 09:30:00', 19.00, 3, 1),
(2, 1015, '2025-01-26 10:00:00', '2025-01-26 11:15:00', 17.00, 3, 1),
(2, 1016, '2025-01-26 12:00:00', '2025-01-26 13:30:00', 20.00, 3, 1),
(2, 1017, '2025-01-27 14:00:00', '2025-01-27 15:30:00', 15.00, 3, 2),
(2, 1018, '2025-01-27 16:00:00', '2025-01-27 17:15:00', 22.00, 3, 1),
-- Receipts for Clara
(3, 1019, '2025-01-23 11:00:00', '2025-01-23 12:15:00', 21.00, 4, 2),
(3, 1001, '2025-01-24 08:30:00', '2025-01-24 09:45:00', 19.00, 4, 2),
(3, 1001, '2025-01-25 10:00:00', '2025-01-25 11:30:00', 20.00, 4, 2),
(3, 1002, '2025-01-25 12:00:00', '2025-01-25 13:30:00', 22.00, 4, 2),
(3, 1003, '2025-01-26 09:00:00', '2025-01-26 10:15:00', 25.00, 4, 2),
(3, 1004, '2025-01-26 11:00:00', '2025-01-26 12:30:00', 18.00, 4, 2),
(3, 1005, '2025-01-27 13:00:00', '2025-01-27 14:15:00', 24.00, 4, 2),
(3, 1006, '2025-01-27 15:00:00', '2025-01-27 16:15:00', 21.00, 4, 2),
-- Receipts for David
(4, 1007, '2025-01-22 13:00:00', '2025-01-22 14:15:00', 17.00, 5, 1),
(4, 1008, '2025-01-23 09:00:00', '2025-01-23 10:00:00', 16.00, 6, 1),
(4, 1009, '2025-01-24 15:00:00', '2025-01-24 16:30:00', 19.00, 5, 2),
(4, 1010, '2025-01-24 17:00:00', '2025-01-24 18:15:00', 15.00, 6, 1),
(4, 1011, '2025-01-25 10:00:00', '2025-01-25 11:30:00', 22.00, 5, 1),
(4, 1012, '2025-01-25 12:00:00', '2025-01-25 13:15:00', 18.00, 6, 2),
(4, 1013, '2025-01-26 08:00:00', '2025-01-26 09:15:00', 23.00, 5, 1);