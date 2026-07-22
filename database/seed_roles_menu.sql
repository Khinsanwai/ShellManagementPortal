-- Seed Role menu item for Shell Management Portal (SQL Server)
-- Adds "Roles" under the "Users" parent menu

-- Users > Roles (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Roles' AND [Url] = '/roles')
BEGIN
    DECLARE @usersRoleId UNIQUEIDENTIFIER;
    SELECT @usersRoleId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Users' AND [ParentId] IS NULL;
    IF @usersRoleId IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Roles', '/roles', @usersRoleId, 4);
END
