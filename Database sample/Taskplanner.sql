USE [TaskPlanner]
GO
/****** Object:  User [TaskPlanner]    Script Date: 12/29/2017 9:49:12 AM ******/
CREATE USER [TaskPlanner] FOR LOGIN [TaskPlanner] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [TaskPlanner]
GO
ALTER ROLE [db_datareader] ADD MEMBER [TaskPlanner]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [TaskPlanner]
GO
