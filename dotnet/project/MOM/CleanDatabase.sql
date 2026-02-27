USE MOM_Management_Database;
GO

-- Delete transactional data first (leaf nodes)
DELETE FROM dbo.MOM_MeetingMember;
DELETE FROM dbo.MOM_Meetings;
DELETE FROM dbo.MOM_Staff;

-- Delete master data
DELETE FROM dbo.MOM_MeetingType;
DELETE FROM dbo.MOM_MeetingVenue;
DELETE FROM dbo.MOM_Department;

-- Reset identity counters
DBCC CHECKIDENT ('dbo.MOM_MeetingMember', RESEED, 0);
DBCC CHECKIDENT ('dbo.MOM_Meetings', RESEED, 0);
DBCC CHECKIDENT ('dbo.MOM_Staff', RESEED, 0);
DBCC CHECKIDENT ('dbo.MOM_MeetingType', RESEED, 0);
DBCC CHECKIDENT ('dbo.MOM_MeetingVenue', RESEED, 0);
DBCC CHECKIDENT ('dbo.MOM_Department', RESEED, 0);
GO
