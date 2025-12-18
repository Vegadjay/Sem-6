CREATE TABLE LOC_Country (
    CountryID INT IDENTITY(1,1) PRIMARY KEY,
    CountryName VARCHAR(100) NOT NULL,
    CountryCode VARCHAR(10),
    CreationDate DATETIME
);

CREATE TABLE LOC_State (
    StateID INT IDENTITY(1,1) PRIMARY KEY,
    StateName VARCHAR(100) NOT NULL,
    CountryID INT NOT NULL,
    CreationDate DATETIME,
    FOREIGN KEY (CountryID) REFERENCES LOC_Country(CountryID)
);

CREATE TABLE LOC_City (
    CityID INT IDENTITY(1,1) PRIMARY KEY,
    CityName VARCHAR(100) NOT NULL,
    StateID INT NOT NULL,
    CountryID INT NOT NULL,
    Pincode VARCHAR(6),
    StdCode VARCHAR(5),
    CreationDate DATETIME,
    FOREIGN KEY (StateID) REFERENCES LOC_State(StateID),
    FOREIGN KEY (CountryID) REFERENCES LOC_Country(CountryID)
);
