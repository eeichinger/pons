if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='TestTable')
	drop table dbo.TestTable
go

CREATE TABLE TestTable
(
    A int PRIMARY KEY NOT NULL,
    B nvarchar(255) NOT NULL
)
GO
