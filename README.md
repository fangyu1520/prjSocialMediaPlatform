# prjSocialMediaPlatform

# 入口點：localhost/Home/Index

# DDL
CREATE TABLE Users (
    UserId BIGINT PRIMARY KEY IDENTITY,
    UserName NVARCHAR(100),
    Email NVARCHAR(100),
    PasswordHash VARBINARY(256),
    Salt VARBINARY(128),
    CoverImage NVARCHAR(MAX),
    Biography NVARCHAR(MAX)
);

CREATE TABLE Posts (
    PostId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    Content NVARCHAR(MAX),
    Image NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Comments (
    CommentId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    PostId INT FOREIGN KEY REFERENCES Posts(PostId),
    Content NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);

# Controller: HelloController(發布貼文、評論、編輯刪除貼文), UserController(登入、註冊)
# Model: User, Post, Comment
# View: Home(Index.cs, CreatePost.cs, Edit.cs), User(Login.cs, Register.cs)
