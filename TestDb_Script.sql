CREATE DATABASE TestDB
GO
USE TestDB
GO
create table Student(
id int identity(1,1) primary key,
fName nvarchar(30),
lName nvarchar(30))
GO
INSERT Student SELECT 'Nancy','Davolio'
INSERT Student SELECT 'Andrew','Fuller'
INSERT Student SELECT 'Janet','Leverling'
INSERT Student SELECT 'Margaret','Peacock'
GO