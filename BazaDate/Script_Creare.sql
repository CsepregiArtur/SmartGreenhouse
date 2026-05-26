-- =============================================
-- Baza de date: SmartGreenhouseDB
-- Descriere: Sistem monitorizare sera inteligenta
-- Autor: Csepregi Artur
-- Data: 2026
-- =============================================

CREATE DATABASE SmartGreenhouseDB;
GO

USE SmartGreenhouseDB;
GO

-- =============================================
-- Tabela: Users
-- =============================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Email NVARCHAR(100),
    Role INT NOT NULL DEFAULT 2, -- 1=Admin, 2=Operator, 3=Viewer
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    LastLogin DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- =============================================
-- Tabela: Sensors
-- =============================================
CREATE TABLE Sensors (
    SensorId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Type INT NOT NULL, -- 1=Temperature, 2=Humidity, 3=SoilMoisture, 4=Light, 5=CO2
    Unit NVARCHAR(10) NOT NULL,
    Location NVARCHAR(100),
    MinValue FLOAT NOT NULL,
    MaxValue FLOAT NOT NULL,
    WarningLow FLOAT NOT NULL,
    WarningHigh FLOAT NOT NULL,
    CriticalLow FLOAT NOT NULL,
    CriticalHigh FLOAT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    InstalledDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByUserId INT NOT NULL,
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId)
);

-- =============================================
-- Tabela: SensorReadings
-- =============================================
CREATE TABLE SensorReadings (
    ReadingId INT IDENTITY(1,1) PRIMARY KEY,
    SensorId INT NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    Value FLOAT NOT NULL,
    Quality INT NOT NULL DEFAULT 1, -- 1=Normal, 2=Suspect, 3=Invalid
    FOREIGN KEY (SensorId) REFERENCES Sensors(SensorId)
);

-- =============================================
-- Tabela: Actuators
-- =============================================
CREATE TABLE Actuators (
    ActuatorId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Type INT NOT NULL, -- 1=Ventilation, 2=WaterPump, 3=Heater, 4=Light, 5=Window
    Location NVARCHAR(100),
    Status INT NOT NULL DEFAULT 0, -- 0=Off, 1=On, 2=Error, 3=Maintenance
    IsAutoMode BIT NOT NULL DEFAULT 1,
    InstalledDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByUserId INT NOT NULL,
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId)
);

-- =============================================
-- Tabela: ActuatorCommands
-- =============================================
CREATE TABLE ActuatorCommands (
    CommandId INT IDENTITY(1,1) PRIMARY KEY,
    ActuatorId INT NOT NULL,
    IssuedByUserId INT NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    Command INT NOT NULL, -- 1=TurnOn, 2=TurnOff, 3=SetAuto, 4=SetManual
    DurationSeconds INT NULL,
    Executed BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (ActuatorId) REFERENCES Actuators(ActuatorId),
    FOREIGN KEY (IssuedByUserId) REFERENCES Users(UserId)
);

-- =============================================
-- Tabela: Alerts
-- =============================================
CREATE TABLE Alerts (
    AlertId INT IDENTITY(1,1) PRIMARY KEY,
    SensorId INT NULL,
    ActuatorId INT NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    Level INT NOT NULL, -- 1=Info, 2=Warning, 3=Critical
    Message NVARCHAR(500) NOT NULL,
    Value FLOAT NULL,
    IsAcknowledged BIT NOT NULL DEFAULT 0,
    AcknowledgedByUserId INT NULL,
    AcknowledgedAt DATETIME NULL,
    FOREIGN KEY (SensorId) REFERENCES Sensors(SensorId),
    FOREIGN KEY (ActuatorId) REFERENCES Actuators(ActuatorId),
    FOREIGN KEY (AcknowledgedByUserId) REFERENCES Users(UserId)
);

-- =============================================
-- Indexes pentru performanță
-- =============================================
CREATE INDEX IX_SensorReadings_SensorId_Timestamp ON SensorReadings(SensorId, Timestamp DESC);
CREATE INDEX IX_Alerts_Timestamp ON Alerts(Timestamp DESC);
CREATE INDEX IX_Alerts_IsAcknowledged ON Alerts(IsAcknowledged);

-- =============================================
-- View: Ultimele citiri pentru fiecare senzor
-- =============================================
GO
CREATE VIEW vw_LatestReadings AS
SELECT 
    s.SensorId,
    s.Name,
    s.Type,
    s.Unit,
    s.Location,
    r.Value,
    r.Timestamp,
    r.Quality,
    CASE 
        WHEN r.Value >= s.CriticalHigh OR r.Value <= s.CriticalLow THEN 'Critical'
        WHEN r.Value >= s.WarningHigh OR r.Value <= s.WarningLow THEN 'Warning'
        ELSE 'Normal'
    END AS Status
FROM Sensors s
OUTER APPLY (
    SELECT TOP 1 Value, Timestamp, Quality
    FROM SensorReadings
    WHERE SensorId = s.SensorId
    ORDER BY Timestamp DESC
) r
WHERE s.IsActive = 1;

-- =============================================
-- Date de test
-- =============================================
GO
-- Utilizatori (parola = "admin" sau "operator" - hash simplu)
INSERT INTO Users (Username, PasswordHash, Email, Role) VALUES
('admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'admin@greenhouse.local', 1),
('operator', 'BuVbYzSB97sHKVfqvPEQyXLoZpHDz+2r4IgCS//kLyM=', 'operator@greenhouse.local', 2);

-- Senzori
INSERT INTO Sensors (Name, Type, Unit, Location, MinValue, MaxValue, WarningLow, WarningHigh, CriticalLow, CriticalHigh, CreatedByUserId) VALUES
('Senzor Temperatură Stânga', 1, '°C', 'Zona Nord', -10, 50, 15, 30, 5, 35, 1),
('Senzor Temperatură Dreapta', 1, '°C', 'Zona Sud', -10, 50, 15, 30, 5, 35, 1),
('Senzor Umiditate Aer', 2, '%', 'Central', 0, 100, 40, 80, 30, 90, 1),
('Senzor Umiditate Sol', 3, '%', 'Rădăcini', 0, 100, 30, 70, 20, 80, 1),
('Senzor Lumină', 4, 'lux', 'Pliafon', 0, 1000, 100, 800, 50, 900, 1),
('Senzor CO2', 5, 'ppm', 'Central', 300, 2000, 350, 800, 330, 1000, 1);

-- Actuatoare
INSERT INTO Actuators (Name, Type, Location, Status, IsAutoMode, CreatedByUserId) VALUES
('Ventilator Principal', 1, 'Pliafon', 0, 1, 1),
('Pompă Apă Stânga', 2, 'Zona Nord', 0, 1, 1),
('Pompă Apă Dreapta', 2, 'Zona Sud', 0, 1, 1),
('Încălzitor', 3, 'Pardoseală', 0, 1, 1),
('Lampă Creștere', 4, 'Pliafon', 0, 1, 1);

-- Citiri test
INSERT INTO SensorReadings (SensorId, Timestamp, Value, Quality)
SELECT 
    SensorId,
    DATEADD(MINUTE, -n, GETDATE()),
    20 + (n % 10),
    1
FROM Sensors
CROSS JOIN (
    SELECT TOP 100 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) as n
    FROM sys.objects
) numbers
WHERE Type = 1;
GO