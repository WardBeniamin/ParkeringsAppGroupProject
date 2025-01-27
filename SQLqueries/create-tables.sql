-- Create Users table
CREATE TABLE Users (
    UserId INT PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Adress VARCHAR(250),
    PhoneNumber VARCHAR(50),
    Password VARCHAR(50)
);

-- Create the PaymentMethods table
CREATE TABLE PaymentMethods (
    PaymentId INT PRIMARY KEY,
    PaymentType VARCHAR(50) NOT NULL
);

-- Create the UserPaymentMethods table (many-to-many relationship between Users and PaymentMethods)
CREATE TABLE UserPaymentMethods (
    UserId INT NOT NULL,
    PaymentId INT NOT NULL,
    PRIMARY KEY (UserId, PaymentId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PaymentId) REFERENCES PaymentMethods(PaymentId)
);

-- Create the Cars table
CREATE TABLE Cars (
    CarId INT PRIMARY KEY,
    PlateNumber VARCHAR(15) NOT NULL UNIQUE,
    Model VARCHAR(50),
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Create the Zones table
CREATE TABLE Zones (
    ZoneId INT PRIMARY KEY,
    Fee DECIMAL(10, 2) NOT NULL,
    Adress VARCHAR(250)
);

-- Create the Receipts table
CREATE TABLE Receipts (
    TransactionId INT PRIMARY KEY,
    UserId INT NOT NULL,
    ZoneId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME,
    Amount DECIMAL(10, 2) NOT NULL,
    CarId INT NOT NULL,
    PaymentId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ZoneId) REFERENCES Zones(ZoneId),
    FOREIGN KEY (CarId) REFERENCES Cars(CarId),
    FOREIGN KEY (PaymentId) REFERENCES PaymentMethods(PaymentId)
);