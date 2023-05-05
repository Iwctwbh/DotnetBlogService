-- 创建表
CREATE TABLE `tblTags` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `DateCreated` DATETIME NOT NULL,
  `DateModified` DATETIME NOT NULL,
  `IsActive` BIT NOT NULL,
  PRIMARY KEY (`Id`)
)

-- 生成历史表
CREATE TABLE `tblTags_History` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TagId` int NOT NULL,
  `Name` varchar(50) NOT NULL,
  `DateCreated` datetime(6) NOT NULL,
  `DateModified` datetime(6) NOT NULL,
  `Action` varchar(10) NOT NULL,
  `IsActive` BIT NOT NULL,
  PRIMARY KEY (`Id`)
)

-- 添加触发器
CREATE TRIGGER `tblTags_Insert` AFTER INSERT ON `tblTags` FOR EACH ROW 
BEGIN
    INSERT INTO `tblTags_History`(`TagId`, `Name`, `DateCreated`, `DateModified`, `IsActive`,`Action`)
    VALUES (NEW.Id, NEW.Name, NEW.DateCreated, NEW.DateModified, NEW.IsActive,'Insert');
END

CREATE TRIGGER `tblTags_Update` AFTER UPDATE ON `tblTags` FOR EACH ROW 
BEGIN
    INSERT INTO `tblTags_History`(`TagId`, `Name`, `DateCreated`, `DateModified`, `IsActive`,`Action`)
    VALUES (OLD.Id, OLD.Name, OLD.DateCreated, OLD.DateModified, OLD.IsActive,'Update');
END

CREATE TRIGGER `tblTags_Delete` AFTER DELETE ON `tblTags` FOR EACH ROW 
BEGIN
    INSERT INTO `tblTags_History`(`TagId`, `Name`, `DateCreated`, `DateModified`, `IsActive`,`Action`)
    VALUES (OLD.Id, OLD.Name, OLD.DateCreated, OLD.DateModified, OLD.IsActive,'Delete');
END



-- 创建表
CREATE TABLE `tblPosts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(100) NOT NULL,
  `Content` longtext NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `ModifiedDate` datetime(6) NOT NULL,
  `IsActive` BIT NOT NULL,
  PRIMARY KEY (`Id`)
);

-- 生成历史表
CREATE TABLE `tblPosts_History` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PostId` int NOT NULL,
  `Title` varchar(100) NOT NULL,
  `Content` longtext NOT NULL,
  `DateCreated` datetime(6) NOT NULL,
  `DateModified` datetime(6) NOT NULL,
  `Action` varchar(10) NOT NULL,
  `IsActive` BIT NOT NULL,
  PRIMARY KEY (`Id`)
);

-- 添加触发器
CREATE TRIGGER `tblPosts_Insert` AFTER INSERT ON `tblPosts` FOR EACH ROW 
BEGIN
    INSERT INTO `tblPosts_History`(`PostId`, `Title`, `Content`, `DateCreated`, `DateModified`, `IsActive`, `Action`)
    VALUES (NEW.Id, NEW.Title, NEW.Content, NEW.CreatedDate, NEW.ModifiedDate, NEW.IsActive,'Insert');
END

CREATE TRIGGER `tblPosts_Update` AFTER UPDATE ON `tblPosts` FOR EACH ROW 
BEGIN
    INSERT INTO `tblPosts_History`(`PostId`, `Title`, `Content`, `DateCreated`, `DateModified`, `IsActive`,`Action`)
    VALUES (OLD.Id, OLD.Title, OLD.Content, OLD.CreatedDate, OLD.ModifiedDate, OLD.IsActive,'Update');
END

CREATE TRIGGER `tblPosts_Delete` AFTER DELETE ON `tblPosts` FOR EACH ROW 
BEGIN
    INSERT INTO `tblPosts_History`(`PostId`, `Title`, `Content`, `DateCreated`, `DateModified`, `IsActive`,`Action`)
    VALUES (OLD.Id, OLD.Title, OLD.Content, OLD.CreatedDate, OLD.ModifiedDate, OLD.IsActive,'Delete');
END

SHOW TRIGGERS FROM Blog;

SHOW TRIGGERS;