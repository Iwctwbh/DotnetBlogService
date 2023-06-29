-- 创建库
CREATE DATABASE Blog CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci;

-- 创建tblTags表
CREATE TABLE `tblTags` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `DateCreated` DATETIME NOT NULL,
  `DateModified` DATETIME NOT NULL,
  `IsActive` BIT NOT NULL,
  PRIMARY KEY (`Id`)
)

-- 创建tblTags_History历史表
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

-- 添加tblTags => tblTags_History触发器
CREATE TRIGGER `tblTags_Insert` AFTER INSERT ON `tblTags` FOR EACH ROW 
BEGIN
    INSERT INTO `tblTags_History`(`TagId`, `Name`, `DateCreated`, `DateModified`, `IsActive`, `Action`)
    VALUES (NEW.Id, NEW.Name, NEW.DateCreated, NEW.DateModified, NEW.IsActive, 'Insert');
END

CREATE TRIGGER `tblTags_Update` AFTER UPDATE ON `tblTags` FOR EACH ROW 
BEGIN
    INSERT INTO `tblTags_History`(`TagId`, `Name`, `DateCreated`, `DateModified`, `IsActive`, `Action`)
    VALUES (OLD.Id, OLD.Name, OLD.DateCreated, OLD.DateModified, OLD.IsActive, 'Update');
END

CREATE TRIGGER `tblTags_Delete` AFTER DELETE ON `tblTags` FOR EACH ROW 
BEGIN
    INSERT INTO `tblTags_History`(`TagId`, `Name`, `DateCreated`, `DateModified`, `IsActive`, `Action`)
    VALUES (OLD.Id, OLD.Name, OLD.DateCreated, OLD.DateModified, OLD.IsActive, 'Delete');
END



-- 创建tblPosts表
CREATE TABLE `tblPosts` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(100) NOT NULL,
  `Content` longtext NOT NULL,
  `DateCreated` datetime(6) NOT NULL,
  `DateModified` datetime(6) NOT NULL,
  `IsActive` BIT NOT NULL,
  PRIMARY KEY (`Id`)
);

-- 创建tblPosts_History历史表
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

-- 添加tblPosts -> tblPosts_History触发器
CREATE TRIGGER `tblPosts_Insert` AFTER INSERT ON `tblPosts` FOR EACH ROW 
BEGIN
    INSERT INTO `tblPosts_History`(`PostId`, `Title`, `Content`, `DateCreated`, `DateModified`, `IsActive`, `Action`)
    VALUES (NEW.Id, NEW.Title, NEW.Content, NEW.DateCreated, NEW.DateModified, NEW.IsActive, 'Insert');
END

CREATE TRIGGER `tblPosts_Update` AFTER UPDATE ON `tblPosts` FOR EACH ROW 
BEGIN
    INSERT INTO `tblPosts_History`(`PostId`, `Title`, `Content`, `DateCreated`, `DateModified`, `IsActive`,`Action`)
    VALUES (OLD.Id, OLD.Title, OLD.Content, OLD.DateCreated, OLD.DateModified, OLD.IsActive, 'Update');
END

CREATE TRIGGER `tblPosts_Delete` AFTER DELETE ON `tblPosts` FOR EACH ROW 
BEGIN
    INSERT INTO `tblPosts_History`(`PostId`, `Title`, `Content`, `DateCreated`, `DateModified`, `IsActive`,`Action`)
    VALUES (OLD.Id, OLD.Title, OLD.Content, OLD.DateCreated, OLD.DateModified, OLD.IsActive, 'Delete');
END

SHOW TRIGGERS FROM Blog;

-- 存储过程SP
CREATE PROCEDURE sp_AddPost(IN Title varchar(100), IN Content LONGTEXT, IN IsActive bit(1))
BEGIN
	DECLARE ErrorCode int DEFAULT 0;
	DECLARE ErrorDesc varchar(50) DEFAULT NULL;
	DECLARE EXIT HANDLER FOR SQLEXCEPTION SET ErrorCode = -1;

	START TRANSACTION;

	INSERT INTO `tblPosts`(`Title`, `Content`, `DateCreated`, `DateModified`, `IsActive`)
    VALUES (Title, Content, NOW(3), NOW(3), IsActive);

	IF ErrorCode = -1 THEN
		ROLLBACK;
	ELSE
		COMMIT;
	END IF;
	
 	SELECT ErrorCode, ErrorDesc;
END

CREATE PROCEDURE sp_UpdatePost(IN Id int, IN Title varchar(100), IN Content LONGTEXT, IN IsActive bit(1))
BEGIN
	UPDATE `tblPosts`
	SET `Title` = Title,
		`Content` = Content,
		`DateModified` = NOW(3),
		`IsActive` = IsActive
    WHERE Id = ID;
END

-- ALTER TABLE tblPosts ENGINE=InnoDB;
-- ALTER TABLE tblPosts_History ENGINE=InnoDB;
-- ALTER TABLE tblTags ENGINE=InnoDB;
-- ALTER TABLE tblTags_History ENGINE=InnoDB;

-- 创建关系表
CREATE TABLE `tblPostsTagsMapping` (
	`PostId` int NOT NULL,
	`TagId` int NOT NULL,
	`IsActive` BIT NOT NULL,
	FOREIGN KEY(`PostId`) REFERENCES `tblPosts` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY(`TagId`) REFERENCES `tblTags` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
);

-- 生成通用结果表
CREATE TABLE `tblResultGeneral` (
  `ErrorCode` int NOT NULL,
  `ErrorDesc` varchar(50)
);


SHOW variables LIKE '%log_output%'
SHOW variables LIKE '%general_log%'

SET GLOBAL log_output = 'TABLE'
SET GLOBAL general_log = 'ON'
SET GLOBAL general_log_file = 'mariadb.log'

SELECT * FROM mysql.general_log ORDER BY event_time DESC 