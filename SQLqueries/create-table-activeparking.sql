-- Create ActiveParking table
CREATE TABLE ActiveParkings (
    ActiveParkingID INT PRIMARY KEY Identity (1,1),
    UserId INT NOT NULL,
    CarId INT NOT NULL,
    ZoneId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Active', 'Cancelled', 'Completed')) 
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ZoneId) REFERENCES Zones(ZoneId),
    FOREIGN KEY (CarId) REFERENCES Cars(CarId),
);