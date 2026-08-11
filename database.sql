-- database.sql
CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'user',
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);

-- Parameterized query example:
-- SELECT UserID, Username, Email
-- FROM Users
-- WHERE Username = @Username;

-- Authentication example:
-- SELECT UserID, Username, PasswordHash, Role
-- FROM Users
-- WHERE Username = @Username;
