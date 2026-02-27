
CREATE DATABASE MOM_Management_Database;
GO

USE MOM_Management_Database;
GO

-- =============================================
--                      Tables
-- =============================================

CREATE TABLE dbo.MOM_MeetingType
(
    MeetingTypeID   INT IDENTITY(1,1) PRIMARY KEY,
    MeetingTypeName NVARCHAR(100) NOT NULL,
    Remarks         NVARCHAR(100) NULL,
    Created         DATETIME DEFAULT GETDATE(),
    Modified        DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.MOM_Department
(
    DepartmentID   INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL,
    Created        DATETIME DEFAULT GETDATE(),
    Modified       DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.MOM_MeetingVenue
(
    MeetingVenueID   INT IDENTITY(1,1) PRIMARY KEY,
    MeetingVenueName NVARCHAR(100) NOT NULL,
    Created          DATETIME DEFAULT GETDATE(),
    Modified         DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.MOM_Staff
(
    StaffID       INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentID  INT NOT NULL,
    StaffName     NVARCHAR(50) NOT NULL,
    MobileNo      NVARCHAR(20) NULL,
    EmailAddress  NVARCHAR(50) NULL,
    Remarks       NVARCHAR(250) NULL,
    Created       DATETIME DEFAULT GETDATE(),
    Modified      DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_MOM_Staff_Department 
        FOREIGN KEY (DepartmentID) REFERENCES dbo.MOM_Department(DepartmentID)
);

CREATE TABLE dbo.MOM_Meetings
(
    MeetingID            INT IDENTITY(1,1) PRIMARY KEY,
    MeetingDate          DATETIME NOT NULL,
    MeetingVenueID       INT NOT NULL,
    MeetingTypeID        INT NOT NULL,
    DepartmentID         INT NOT NULL,
    MeetingDescription   NVARCHAR(250) NULL,
    DocumentPath         NVARCHAR(250) NULL,
    Created              DATETIME DEFAULT GETDATE(),
    Modified             DATETIME NOT NULL DEFAULT GETDATE(),
    IsCancelled          BIT NULL DEFAULT (0),
    CancellationDateTime DATETIME NULL,
    CancellationReason   NVARCHAR(250) NULL,
    CONSTRAINT FK_MOM_Meetings_Venue 
        FOREIGN KEY (MeetingVenueID) REFERENCES dbo.MOM_MeetingVenue(MeetingVenueID),
    CONSTRAINT FK_MOM_Meetings_Type 
        FOREIGN KEY (MeetingTypeID) REFERENCES dbo.MOM_MeetingType(MeetingTypeID),
    CONSTRAINT FK_MOM_Meetings_Department 
        FOREIGN KEY (DepartmentID) REFERENCES dbo.MOM_Department(DepartmentID)
);

CREATE TABLE dbo.MOM_MeetingMember
(
    MeetingMemberID INT IDENTITY(1,1) PRIMARY KEY,
    MeetingID       INT NOT NULL,
    StaffID         INT NOT NULL,
    IsPresent       BIT NOT NULL DEFAULT (0),
    Remarks         NVARCHAR(250) NULL,
    Created         DATETIME DEFAULT GETDATE(),
    Modified        DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_MOM_MeetingMember_Meeting 
        FOREIGN KEY (MeetingID) REFERENCES dbo.MOM_Meetings(MeetingID),
    CONSTRAINT FK_MOM_MeetingMember_Staff 
        FOREIGN KEY (StaffID) REFERENCES dbo.MOM_Staff(StaffID)
);
GO

-- =============================================
--             Stored Procedures 
-- =============================================

-- Meeting Type

CREATE PROCEDURE PR_MOM_MeetingType_SelectAll
AS
BEGIN
    SELECT * FROM MOM_MeetingType
    ORDER BY MeetingTypeName;
END
GO

CREATE PROCEDURE PR_MOM_MeetingType_Insert
    @MeetingTypeName NVARCHAR(100),
    @Remarks NVARCHAR(100)
AS
BEGIN
    INSERT INTO MOM_MeetingType
    (
        MeetingTypeName,
        Remarks,
        Created,
        Modified
    )
    VALUES
    (
        @MeetingTypeName,
        @Remarks,
        GETDATE(),
        GETDATE()
    );
END
GO

CREATE PROCEDURE PR_MOM_MeetingType_UpdateByPK
    @MeetingTypeID INT,
    @MeetingTypeName NVARCHAR(100),
    @Remarks NVARCHAR(100)
AS
BEGIN
    UPDATE MOM_MeetingType
    SET
        MeetingTypeName = @MeetingTypeName,
        Remarks = @Remarks,
        Modified = GETDATE()
    WHERE MeetingTypeID = @MeetingTypeID;
END
GO

CREATE PROCEDURE PR_MOM_MeetingType_DeleteByPK
    @MeetingTypeID INT
AS
BEGIN
    DELETE FROM MOM_MeetingType
    WHERE MeetingTypeID = @MeetingTypeID;
END
GO

-- Department

CREATE PROCEDURE PR_MOM_Department_SelectAll
AS
BEGIN
    SELECT * FROM MOM_Department
    ORDER BY DepartmentName;
END
GO

CREATE PROCEDURE PR_MOM_Department_Insert
    @DepartmentName NVARCHAR(100)
AS
BEGIN
    INSERT INTO MOM_Department
    (
        DepartmentName,
        Created,
        Modified
    )
    VALUES
    (
        @DepartmentName,
        GETDATE(),
        GETDATE()
    );
END
GO

CREATE PROCEDURE PR_MOM_Department_UpdateByPK
    @DepartmentID INT,
    @DepartmentName NVARCHAR(100)
AS
BEGIN
    UPDATE MOM_Department
    SET
        DepartmentName = @DepartmentName,
        Modified = GETDATE()
    WHERE DepartmentID = @DepartmentID;
END
GO

CREATE PROCEDURE PR_MOM_Department_DeleteByPK
    @DepartmentID INT
AS
BEGIN
    DELETE FROM MOM_Department
    WHERE DepartmentID = @DepartmentID;
END
GO

-- Meeting Venue

CREATE PROCEDURE PR_MOM_MeetingVenue_SelectAll
AS
BEGIN
    SELECT * FROM MOM_MeetingVenue
    ORDER BY MeetingVenueName;
END
GO

CREATE PROCEDURE PR_MOM_MeetingVenue_Insert
    @MeetingVenueName NVARCHAR(100)
AS
BEGIN
    INSERT INTO MOM_MeetingVenue
    (
        MeetingVenueName,
        Created,
        Modified
    )
    VALUES
    (
        @MeetingVenueName,
        GETDATE(),
        GETDATE()
    );
END
GO

CREATE PROCEDURE PR_MOM_MeetingVenue_UpdateByPK
    @MeetingVenueID INT,
    @MeetingVenueName NVARCHAR(100)
AS
BEGIN
    UPDATE MOM_MeetingVenue
    SET
        MeetingVenueName = @MeetingVenueName,
        Modified = GETDATE()
    WHERE MeetingVenueID = @MeetingVenueID;
END
GO

CREATE PROCEDURE PR_MOM_MeetingVenue_DeleteByPK
    @MeetingVenueID INT
AS
BEGIN
    DELETE FROM MOM_MeetingVenue
    WHERE MeetingVenueID = @MeetingVenueID;
END
GO

-- Staff

CREATE OR ALTER PROCEDURE PR_MOM_Staff_SelectAll
AS
BEGIN
    SELECT
        s.*,
        d.DepartmentName
    FROM MOM_Staff s
    LEFT JOIN MOM_Department d
    ON d.DepartmentID = s.DepartmentID
    ORDER BY s.StaffName;
END
GO

-- Meetings

CREATE PROCEDURE PR_MOM_Meetings_SelectAll
AS
BEGIN
    SELECT
        m.*,
        mv.MeetingVenueName,
        mt.MeetingTypeName,
        d.DepartmentName
    FROM MOM_Meetings m
    JOIN MOM_MeetingVenue mv ON mv.MeetingVenueID = m.MeetingVenueID
    JOIN MOM_MeetingType mt ON mt.MeetingTypeID = m.MeetingTypeID
    JOIN MOM_Department d ON d.DepartmentID = m.DepartmentID
    ORDER BY m.MeetingDate DESC;
END
GO

CREATE PROCEDURE PR_MOM_Meetings_Insert
    @MeetingDate DATETIME,
    @MeetingVenueID INT,
    @MeetingTypeID INT,
    @DepartmentID INT,
    @MeetingDescription NVARCHAR(250),
    @DocumentPath NVARCHAR(250),
    @IsCancelled BIT,
    @CancellationDateTime DATETIME,
    @CancellationReason NVARCHAR(250)
AS
BEGIN
    INSERT INTO MOM_Meetings
    (
        MeetingDate,
        MeetingVenueID,
        MeetingTypeID,
        DepartmentID,
        MeetingDescription,
        DocumentPath,
        Created,
        Modified,
        IsCancelled,
        CancellationDateTime,
        CancellationReason
    )
    VALUES
    (
        @MeetingDate,
        @MeetingVenueID,
        @MeetingTypeID,
        @DepartmentID,
        @MeetingDescription,
        @DocumentPath,
        GETDATE(),
        GETDATE(),
        @IsCancelled,
        @CancellationDateTime,
        @CancellationReason
    );
END
GO

CREATE PROCEDURE PR_MOM_Meetings_UpdateByPK
    @MeetingID INT,
    @MeetingDate DATETIME,
    @MeetingVenueID INT,
    @MeetingTypeID INT,
    @DepartmentID INT,
    @MeetingDescription NVARCHAR(250),
    @DocumentPath NVARCHAR(250),
    @IsCancelled BIT,
    @CancellationDateTime DATETIME,
    @CancellationReason NVARCHAR(250)
AS
BEGIN
    UPDATE MOM_Meetings
    SET
        MeetingDate = @MeetingDate,
        MeetingVenueID = @MeetingVenueID,
        MeetingTypeID = @MeetingTypeID,
        DepartmentID = @DepartmentID,
        MeetingDescription = @MeetingDescription,
        DocumentPath = @DocumentPath,
        IsCancelled = @IsCancelled,
        CancellationDateTime = @CancellationDateTime,
        CancellationReason = @CancellationReason,
        Modified = GETDATE()
    WHERE MeetingID = @MeetingID;
END
GO

CREATE PROCEDURE PR_MOM_Meetings_DeleteByPK
    @MeetingID INT
AS
BEGIN
    DELETE FROM MOM_Meetings
    WHERE MeetingID = @MeetingID;
END
GO

-- Meeting Members (Attendance)

CREATE PROCEDURE PR_MOM_MeetingMember_Insert
    @MeetingID INT,
    @StaffID INT,
    @IsPresent BIT,
    @Remarks NVARCHAR(250)
AS
BEGIN
    INSERT INTO MOM_MeetingMember
    (
        MeetingID,
        StaffID,
        IsPresent,
        Remarks,
        Created,
        Modified
    )
    VALUES
    (
        @MeetingID,
        @StaffID,
        @IsPresent,
        @Remarks,
        GETDATE(),
        GETDATE()
    );
END
GO

CREATE PROCEDURE PR_MOM_MeetingMember_UpdateByPK
    @MeetingMemberID INT,
    @MeetingID INT,
    @StaffID INT,
    @IsPresent BIT,
    @Remarks NVARCHAR(250)
AS
BEGIN
    UPDATE MOM_MeetingMember
    SET
        MeetingID = @MeetingID,
        StaffID = @StaffID,
        IsPresent = @IsPresent,
        Remarks = @Remarks,
        Modified = GETDATE()
    WHERE MeetingMemberID = @MeetingMemberID;
END
GO

CREATE PROCEDURE PR_MOM_MeetingMember_DeleteByPK
    @MeetingMemberID INT
AS
BEGIN
    DELETE FROM MOM_MeetingMember
    WHERE MeetingMemberID = @MeetingMemberID;
END
GO

-- for Search

CREATE PROCEDURE PR_MOM_Search_Meetings
(
    @MeetingTypeID     INT           = NULL,
    @DepartmentID      INT           = NULL,
    @MeetingVenueID    INT           = NULL,
    @StartDate         DATE          = NULL,
    @EndDate           DATE          = NULL,
    @SearchText        NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.MeetingID,
        m.MeetingDate,
        mt.MeetingTypeName,
        d.DepartmentName,
        mv.MeetingVenueName,
        m.MeetingDescription,
        m.IsCancelled
    FROM MOM_Meetings m
    INNER JOIN MOM_MeetingType mt 
        ON m.MeetingTypeID = mt.MeetingTypeID
    INNER JOIN MOM_Department d 
        ON m.DepartmentID = d.DepartmentID
    INNER JOIN MOM_MeetingVenue mv 
        ON m.MeetingVenueID = mv.MeetingVenueID
    WHERE
        (@MeetingTypeID IS NULL OR m.MeetingTypeID = @MeetingTypeID)
        AND (@DepartmentID IS NULL OR m.DepartmentID = @DepartmentID)
        AND (@MeetingVenueID IS NULL OR m.MeetingVenueID = @MeetingVenueID)
        AND (@MeetingVenueID IS NULL OR m.MeetingVenueID = @MeetingVenueID)
        AND (@StartDate IS NULL OR CAST(m.MeetingDate AS DATE) >= @StartDate)
        AND (@EndDate IS NULL OR CAST(m.MeetingDate AS DATE) <= @EndDate)
        AND (
            @SearchText IS NULL
            OR mt.MeetingTypeName LIKE '%' + @SearchText + '%'
            OR d.DepartmentName LIKE '%' + @SearchText + '%'
            OR mv.MeetingVenueName LIKE '%' + @SearchText + '%'
            OR m.MeetingDescription LIKE '%' + @SearchText + '%'
        )
    ORDER BY m.MeetingDate DESC;
END
GO

CREATE OR ALTER PROCEDURE PR_MOM_MeetingType_Search
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        [MeetingTypeID],
        [MeetingTypeName],
        [Remarks],
        [Created],
        [Modified]
    FROM [dbo].[MOM_MeetingType]
    WHERE 
        (@SearchText IS NULL OR [MeetingTypeName] LIKE '%' + @SearchText + '%')
    ORDER BY [MeetingTypeName]
END
GO

CREATE OR ALTER PROCEDURE PR_MOM_Department_Search
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        [DepartmentID],
        [DepartmentName],
        [Created],
        [Modified]
    FROM [dbo].[MOM_Department]
    WHERE 
        (@SearchText IS NULL OR [DepartmentName] LIKE '%' + @SearchText + '%')
    ORDER BY [DepartmentName]
END
GO

CREATE OR ALTER PROCEDURE PR_MOM_MeetingVenue_Search
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        [MeetingVenueID],
        [MeetingVenueName],
        [Created],
        [Modified]
    FROM [dbo].[MOM_MeetingVenue]
    WHERE 
        (@SearchText IS NULL OR [MeetingVenueName] LIKE '%' + @SearchText + '%')
    ORDER BY [MeetingVenueName]
END
GO

CREATE OR ALTER PROCEDURE PR_MOM_Staff_Search
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        S.[StaffID],
        S.[DepartmentID],
        S.[StaffName],
        S.[MobileNo],
        S.[EmailAddress],
        S.[Remarks],
        S.[Created],
        S.[Modified]
    FROM [dbo].[MOM_Staff] S
    LEFT JOIN [dbo].[MOM_Department] D ON S.[DepartmentID] = D.[DepartmentID]
    WHERE 
        (@SearchText IS NULL OR 
         S.[StaffName] LIKE '%' + @SearchText + '%' OR
         S.[EmailAddress] LIKE '%' + @SearchText + '%' OR
         S.[MobileNo] LIKE '%' + @SearchText + '%' OR
        D.[DepartmentName] LIKE '%' + @SearchText + '%')
    ORDER BY S.[StaffName]
END
GO

-- =============================================
--             Sample Data (Seed Data)
-- =============================================

-- 1. Departments
INSERT INTO dbo.MOM_Department (DepartmentName) VALUES 
('Engineering'), ('Marketing'), ('Sales'), ('Human Resources'), 
('Finance'), ('Operations'), ('IT Support'), ('Product Design'), 
('Legal'), ('Customer Success');

-- 2. Meeting Types
INSERT INTO dbo.MOM_MeetingType (MeetingTypeName, Remarks) VALUES 
('Daily Standup', 'Daily sync-up'), 
('Sprint Planning', 'Planning next sprint'), 
('Sprint Retrospective', 'Reviewing past sprint'), 
('Board Meeting', 'Executive meeting'), 
('Client Presentation', 'Pitching to clients'), 
('1-on-1 Review', 'Performance review'), 
('Brainstorming', 'Creative ideation'), 
('Town Hall', 'All-hands meeting'), 
('Project Kickoff', 'Starting new project'), 
('Training Session', 'Skill development');

-- 3. Meeting Venues
INSERT INTO dbo.MOM_MeetingVenue (MeetingVenueName) VALUES 
('Conference Room A'), ('Conference Room B'), ('Boardroom 1'), 
('Training Hall'), ('Virtual - Zoom'), ('Virtual - Teams'), 
('Auditorium'), ('Huddle Room 1'), ('Huddle Room 2'), ('Cafeteria');

-- 4. Staff (Indian Names)
INSERT INTO dbo.MOM_Staff (DepartmentID, StaffName, MobileNo, EmailAddress, Remarks) VALUES 
(1, 'Rahul Sharma', '+919876543210', 'rahul.s@example.com', 'Senior Developer'),
(2, 'Priya Patel', '+919876543211', 'priya.p@example.com', 'Marketing Lead'),
(3, 'Amit Singh', '+919876543212', 'amit.s@example.com', 'Sales Manager'),
(4, 'Neha Gupta', '+919876543213', 'neha.g@example.com', 'HR Representative'),
(5, 'Vikram Desai', '+919876543214', 'vikram.d@example.com', 'Finance Analyst'),
(6, 'Anjali Verma', '+919876543215', 'anjali.v@example.com', 'Operations Head'),
(7, 'Kiran Kumar', '+919876543216', 'kiran.k@example.com', 'IT Admin'),
(8, 'Pooja Reddy', '+919876543217', 'pooja.r@example.com', 'UI/UX Designer'),
(9, 'Suresh Nair', '+919876543218', 'suresh.n@example.com', 'Legal Counsel'),
(10, 'Sneha Joshi', '+919876543219', 'sneha.j@example.com', 'Customer Success Agent');

-- 5. Meetings
INSERT INTO dbo.MOM_Meetings (MeetingDate, MeetingVenueID, MeetingTypeID, DepartmentID, MeetingDescription, IsCancelled) VALUES 
(DATEADD(day, -1, GETDATE()), 5, 1, 1, 'Tech Team Sync', 0),
(DATEADD(day, 2, GETDATE()), 1, 2, 1, 'Q3 Roadmap Planning', 0),
(DATEADD(day, -5, GETDATE()), 3, 4, 5, 'Annual Financial Review', 0),
(DATEADD(day, 1, GETDATE()), 6, 8, 4, 'Company All-Hands', 0),
(DATEADD(day, 3, GETDATE()), 2, 7, 2, 'Ad Campaign Ideation', 0),
(DATEADD(day, -2, GETDATE()), 4, 10, 7, 'Cybersecurity Protocols', 0),
(DATEADD(day, -10, GETDATE()), 8, 6, 6, 'Vendor Assessment', 0),
(DATEADD(day, 5, GETDATE()), 5, 5, 3, 'Enterprise Client Pitch', 0),
(DATEADD(day, -7, GETDATE()), 9, 3, 8, 'Design Sprint Feedback', 0),
(DATEADD(day, 4, GETDATE()), 7, 9, 10, 'New Support Tool Rollout', 0);

-- 6. Meeting Members (Attendance tracking)
INSERT INTO dbo.MOM_MeetingMember (MeetingID, StaffID, IsPresent, Remarks) VALUES 
(1, 1, 1, 'On time'),
(1, 7, 1, 'Joined late'),
(2, 1, 0, 'On leave'),
(2, 8, 1, 'Present'),
(3, 5, 1, 'Presented slides'),
(4, 4, 1, 'Host - town hall overview'),
(4, 2, 0, 'Missed it'),
(5, 2, 1, 'Active participant'),
(6, 7, 1, 'Trainer'),
(7, 6, 1, 'Completed review'),
(8, 3, 1, 'Closing deal'),
(9, 8, 1, 'Led feedback'),
(10, 10, 1, 'Setup complete');
