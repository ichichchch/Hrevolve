/*
 Navicat Premium Dump SQL

 Source Server         : Local
 Source Server Type    : SQL Server
 Source Server Version : 15002145 (15.00.2145)
 Source Host           : Admin:1433
 Source Catalog        : Hrevolve
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 15002145 (15.00.2145)
 File Encoding         : 65001

 Date: 27/09/2025 23:22:08
*/


-- ----------------------------
-- Table structure for Attendance
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Attendance]') AND type IN ('U'))
	DROP TABLE [dbo].[Attendance]
GO

CREATE TABLE [dbo].[Attendance] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [StaffId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Position] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Department] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Period] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [ScheduledHours] decimal(18,2)  NULL,
  [HoursWorked] decimal(18,2)  NULL,
  [StatutoryDayOvertime] decimal(18,2)  NULL,
  [RestDayOvertime] decimal(18,2)  NULL,
  [WorkingDayOvertime] decimal(18,2)  NULL,
  [BelateLength] datetime2(7)  NULL,
  [LeaveTime] datetime2(7)  NULL,
  [AbsenceLength] datetime2(7)  NULL,
  [Adjust] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [TimeOffInLieu] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Attendance] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for AttendanceData
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[AttendanceData]') AND type IN ('U'))
	DROP TABLE [dbo].[AttendanceData]
GO

CREATE TABLE [dbo].[AttendanceData] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [StaffId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Department] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [ClockInTime] datetime2(7)  NULL,
  [AttendanceType] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [GPS] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [DeviceName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [CodeSource] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [FieldWork] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [BirdDeviceId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[AttendanceData] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for AttendanceItem
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[AttendanceItem]') AND type IN ('U'))
	DROP TABLE [dbo].[AttendanceItem]
GO

CREATE TABLE [dbo].[AttendanceItem] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [AttendanceItem] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [CustomName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [RegulationRulesAlias] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Status] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Formula] nvarchar(max) COLLATE Chinese_PRC_CI_AS  NULL,
  [UpdateTime] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[AttendanceItem] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for AttendanceRecord
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[AttendanceRecord]') AND type IN ('U'))
	DROP TABLE [dbo].[AttendanceRecord]
GO

CREATE TABLE [dbo].[AttendanceRecord] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [StaffId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Date] date  NULL,
  [ShiftType] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [ShiftIn] datetime2(7)  NULL,
  [ShiftOut] datetime2(7)  NULL,
  [LaborHours] decimal(18,2)  NULL,
  [ActualAmount] decimal(18,2)  NULL,
  [MinutesLate] decimal(18,2)  NULL,
  [LeaveEarlyMinutes] decimal(18,2)  NULL,
  [LaborStatus] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Remark] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[AttendanceRecord] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BalanceAdjustment
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BalanceAdjustment]') AND type IN ('U'))
	DROP TABLE [dbo].[BalanceAdjustment]
GO

CREATE TABLE [dbo].[BalanceAdjustment] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [StaffId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Amount] decimal(18,2)  NULL,
  [LeaveType] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Time] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[BalanceAdjustment] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for BankAccount
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[BankAccount]') AND type IN ('U'))
	DROP TABLE [dbo].[BankAccount]
GO

CREATE TABLE [dbo].[BankAccount] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [BankCode] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [BankBranchCode] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [BankAccountNo] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [CurrencyCode] nvarchar(10) COLLATE Chinese_PRC_CI_AS  NULL,
  [AccountType] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [PaymentCode] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [PaymentReference] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[BankAccount] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Company
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Company]') AND type IN ('U'))
	DROP TABLE [dbo].[Company]
GO

CREATE TABLE [dbo].[Company] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [SystemDisplayName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [BRName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [BRNo] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [PhoneNumber] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [EmailAddress] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [CompanyAddress] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [Region] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [TimeZone] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Website] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Company] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Department
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Department]') AND type IN ('U'))
	DROP TABLE [dbo].[Department]
GO

CREATE TABLE [dbo].[Department] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Code] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [DepartmentName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Status] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [CreatedBy] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [CreatedTime] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[Department] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Device
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Device]') AND type IN ('U'))
	DROP TABLE [dbo].[Device]
GO

CREATE TABLE [dbo].[Device] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Location] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [CheckTime] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [VerificationMethod] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [NFCUid] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [PinCode] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [UploadTime] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [IpAddress] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Device] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for EmergencyContact
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[EmergencyContact]') AND type IN ('U'))
	DROP TABLE [dbo].[EmergencyContact]
GO

CREATE TABLE [dbo].[EmergencyContact] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Relationship] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [PhoneNumber] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [PeopleId] int  NULL
)
GO

ALTER TABLE [dbo].[EmergencyContact] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for EmployeeReturn
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeReturn]') AND type IN ('U'))
	DROP TABLE [dbo].[EmployeeReturn]
GO

CREATE TABLE [dbo].[EmployeeReturn] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [FullName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Designation] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [EmployerFileNo] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[EmployeeReturn] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for ExpenseClaim
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ExpenseClaim]') AND type IN ('U'))
	DROP TABLE [dbo].[ExpenseClaim]
GO

CREATE TABLE [dbo].[ExpenseClaim] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [ExpenseContent] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [ExpenseType] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Date] date  NULL,
  [Amount] decimal(18,2)  NULL
)
GO

ALTER TABLE [dbo].[ExpenseClaim] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for ExpenseType
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ExpenseType]') AND type IN ('U'))
	DROP TABLE [dbo].[ExpenseType]
GO

CREATE TABLE [dbo].[ExpenseType] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [TypeName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[ExpenseType] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LeaveApplication
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaveApplication]') AND type IN ('U'))
	DROP TABLE [dbo].[LeaveApplication]
GO

CREATE TABLE [dbo].[LeaveApplication] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [StartTime] datetime2(7)  NULL,
  [EndTime] datetime2(7)  NULL,
  [ApplicationNo] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Reason] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [Status] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [People] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [LeaveType] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Duration] time(7)  NULL
)
GO

ALTER TABLE [dbo].[LeaveApplication] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LeaveBalance
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaveBalance]') AND type IN ('U'))
	DROP TABLE [dbo].[LeaveBalance]
GO

CREATE TABLE [dbo].[LeaveBalance] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Avatar] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [StaffId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [LeavePolicy] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [CurrentBalance] decimal(18,2)  NULL,
  [OnPlan] decimal(18,2)  NULL,
  [Usable] decimal(18,2)  NULL,
  [EmploymentType] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [SalaryType] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LeaveBalance] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LeaveBalanceRecord
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaveBalanceRecord]') AND type IN ('U'))
	DROP TABLE [dbo].[LeaveBalanceRecord]
GO

CREATE TABLE [dbo].[LeaveBalanceRecord] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Time] datetime2(7)  NULL,
  [Status] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Description] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [Reason] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [Amount] decimal(18,2)  NULL,
  [Balance] decimal(18,2)  NULL,
  [SubmissionDate] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[LeaveBalanceRecord] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for LeaveType
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaveType]') AND type IN ('U'))
	DROP TABLE [dbo].[LeaveType]
GO

CREATE TABLE [dbo].[LeaveType] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [LeaveCode] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [LeaveTypeValue] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [MinimumLength] time(7)  NULL,
  [LeaveBalanceCalculationFormula] nvarchar(max) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[LeaveType] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Location
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Location]') AND type IN ('U'))
	DROP TABLE [dbo].[Location]
GO

CREATE TABLE [dbo].[Location] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Code] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [AddressName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [FullAddress] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [IsGpsActive] bit DEFAULT 0 NOT NULL,
  [Status] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [CreatedBy] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [CreatedTime] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[Location] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for People
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[People]') AND type IN ('U'))
	DROP TABLE [dbo].[People]
GO

CREATE TABLE [dbo].[People] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [EnglishName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [StaffId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [FirstName] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [SecondName] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [PhoneNumber] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [EmailAddress] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [IdentityCardNo] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [ForeignEmployeeIdentificationNumber] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [OccupationTaxNumber] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Country] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Gender] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [MaritalStatus] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [HomeAddress] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[People] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Position
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Position]') AND type IN ('U'))
	DROP TABLE [dbo].[Position]
GO

CREATE TABLE [dbo].[Position] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Code] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [PositionName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Status] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [CreatedBy] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [CreatedTime] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[Position] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for PublicHoliday
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PublicHoliday]') AND type IN ('U'))
	DROP TABLE [dbo].[PublicHoliday]
GO

CREATE TABLE [dbo].[PublicHoliday] (
  [Code] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [PolicyName] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Type] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Date] date  NULL,
  [Week] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[PublicHoliday] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Tag
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Tag]') AND type IN ('U'))
	DROP TABLE [dbo].[Tag]
GO

CREATE TABLE [dbo].[Tag] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Code] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Parent] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Status] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Tag] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Timesheet
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Timesheet]') AND type IN ('U'))
	DROP TABLE [dbo].[Timesheet]
GO

CREATE TABLE [dbo].[Timesheet] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [StaffId] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Type] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [WorkingDayType] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [StartTime] datetime2(7)  NULL,
  [EndTime] datetime2(7)  NULL,
  [MealTime] datetime2(7)  NULL,
  [Duration] time(7)  NULL,
  [Location] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [Reason] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL,
  [Approver] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [SubmissionTime] datetime2(7)  NULL,
  [Status] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Timesheet] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Table structure for Upload
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Upload]') AND type IN ('U'))
	DROP TABLE [dbo].[Upload]
GO

CREATE TABLE [dbo].[Upload] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Name] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [URL] nvarchar(500) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Upload] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Auto increment value for Attendance
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Attendance]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Attendance
-- ----------------------------
ALTER TABLE [dbo].[Attendance] ADD CONSTRAINT [PK__Attendan__3214EC0708E67775] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for AttendanceData
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[AttendanceData]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table AttendanceData
-- ----------------------------
ALTER TABLE [dbo].[AttendanceData] ADD CONSTRAINT [PK__Attendan__3214EC07C4CF53BB] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for AttendanceItem
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[AttendanceItem]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table AttendanceItem
-- ----------------------------
ALTER TABLE [dbo].[AttendanceItem] ADD CONSTRAINT [PK__Attendan__3214EC079D3107E9] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for AttendanceRecord
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[AttendanceRecord]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table AttendanceRecord
-- ----------------------------
ALTER TABLE [dbo].[AttendanceRecord] ADD CONSTRAINT [PK__Attendan__3214EC073682B629] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for BalanceAdjustment
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[BalanceAdjustment]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table BalanceAdjustment
-- ----------------------------
ALTER TABLE [dbo].[BalanceAdjustment] ADD CONSTRAINT [PK__BalanceA__3214EC07BD7BC9EF] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for BankAccount
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[BankAccount]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table BankAccount
-- ----------------------------
ALTER TABLE [dbo].[BankAccount] ADD CONSTRAINT [PK__BankAcco__3214EC0744EF64BE] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Company
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Company]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Company
-- ----------------------------
ALTER TABLE [dbo].[Company] ADD CONSTRAINT [PK__Company__3214EC077A4840ED] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Department
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Department]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Department
-- ----------------------------
ALTER TABLE [dbo].[Department] ADD CONSTRAINT [PK__Departme__3214EC071CF3C5D2] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Device
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Device]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Device
-- ----------------------------
ALTER TABLE [dbo].[Device] ADD CONSTRAINT [PK__Device__3214EC07D6A0BF7E] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for EmergencyContact
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[EmergencyContact]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table EmergencyContact
-- ----------------------------
ALTER TABLE [dbo].[EmergencyContact] ADD CONSTRAINT [PK__Emergenc__3214EC076DF05118] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for EmployeeReturn
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[EmployeeReturn]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table EmployeeReturn
-- ----------------------------
ALTER TABLE [dbo].[EmployeeReturn] ADD CONSTRAINT [PK__Employee__3214EC070506600A] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for ExpenseClaim
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[ExpenseClaim]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table ExpenseClaim
-- ----------------------------
ALTER TABLE [dbo].[ExpenseClaim] ADD CONSTRAINT [PK__ExpenseC__3214EC07C50DC7DC] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for ExpenseType
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[ExpenseType]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table ExpenseType
-- ----------------------------
ALTER TABLE [dbo].[ExpenseType] ADD CONSTRAINT [PK__ExpenseT__3214EC079744F69C] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for LeaveApplication
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[LeaveApplication]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table LeaveApplication
-- ----------------------------
ALTER TABLE [dbo].[LeaveApplication] ADD CONSTRAINT [PK__LeaveApp__3214EC072D6F8FDF] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for LeaveBalance
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[LeaveBalance]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table LeaveBalance
-- ----------------------------
ALTER TABLE [dbo].[LeaveBalance] ADD CONSTRAINT [PK__LeaveBal__3214EC07DB61E526] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for LeaveBalanceRecord
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[LeaveBalanceRecord]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table LeaveBalanceRecord
-- ----------------------------
ALTER TABLE [dbo].[LeaveBalanceRecord] ADD CONSTRAINT [PK__LeaveBal__3214EC07C35982C0] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for LeaveType
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[LeaveType]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table LeaveType
-- ----------------------------
ALTER TABLE [dbo].[LeaveType] ADD CONSTRAINT [PK__LeaveTyp__3214EC072516EE4B] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Location
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Location]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Location
-- ----------------------------
ALTER TABLE [dbo].[Location] ADD CONSTRAINT [PK__Location__3214EC07C6C538DD] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for People
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[People]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table People
-- ----------------------------
ALTER TABLE [dbo].[People] ADD CONSTRAINT [PK__People__3214EC07C37776B8] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Position
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Position]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Position
-- ----------------------------
ALTER TABLE [dbo].[Position] ADD CONSTRAINT [PK__Position__3214EC07B7BE4702] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Tag
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Tag]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Tag
-- ----------------------------
ALTER TABLE [dbo].[Tag] ADD CONSTRAINT [PK__Tag__3214EC070B651F2E] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Timesheet
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Timesheet]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Timesheet
-- ----------------------------
ALTER TABLE [dbo].[Timesheet] ADD CONSTRAINT [PK__Timeshee__3214EC07B7A02E8D] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Upload
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Upload]', RESEED, 1)
GO


-- ----------------------------
-- Primary Key structure for table Upload
-- ----------------------------
ALTER TABLE [dbo].[Upload] ADD CONSTRAINT [PK__Upload__3214EC07D66D8064] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Foreign Keys structure for table EmergencyContact
-- ----------------------------
ALTER TABLE [dbo].[EmergencyContact] ADD CONSTRAINT [FK_EmergencyContact_People] FOREIGN KEY ([PeopleId]) REFERENCES [dbo].[People] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

