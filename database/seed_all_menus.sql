-- Seed all menu items for Shell Management Portal (SQL Server)
-- Parent menus first, then children

-- Dashboard
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Dashboard')
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'Dashboard', '/dashboard', NULL, 1);

-- Users (parent)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Users' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'Users', NULL, NULL, 2);

-- Users > User List (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'User List')
BEGIN
    DECLARE @usersId UNIQUEIDENTIFIER;
    SELECT @usersId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Users' AND [ParentId] IS NULL;
    IF @usersId IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'User List', '/users', @usersId, 1);
END

-- Users > User Groups (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'User Groups')
BEGIN
    DECLARE @usersParentId UNIQUEIDENTIFIER;
    SELECT @usersParentId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Users' AND [ParentId] IS NULL;
    IF @usersParentId IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'User Groups', '/user-groups', @usersParentId, 2);
END

-- Users > Groups (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Groups' AND [Url] = '/groups')
BEGIN
    DECLARE @usersPId UNIQUEIDENTIFIER;
    SELECT @usersPId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Users' AND [ParentId] IS NULL;
    IF @usersPId IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Groups', '/groups', @usersPId, 3);
END

-- Administration (parent)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Administration' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'Administration', NULL, NULL, 3);

-- Administration > Menu Items (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Menu Items')
BEGIN
    DECLARE @adminId UNIQUEIDENTIFIER;
    SELECT @adminId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Administration' AND [ParentId] IS NULL;
    IF @adminId IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Menu Items', '/menu-items', @adminId, 1);
END

-- Administration > Claims (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Claims')
BEGIN
    DECLARE @adminId2 UNIQUEIDENTIFIER;
    SELECT @adminId2 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Administration' AND [ParentId] IS NULL;
    IF @adminId2 IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Claims', '/claims', @adminId2, 2);
END

-- Administration > Institutions (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Institutions')
BEGIN
    DECLARE @adminId3 UNIQUEIDENTIFIER;
    SELECT @adminId3 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Administration' AND [ParentId] IS NULL;
    IF @adminId3 IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Institutions', '/institutions', @adminId3, 3);
END

-- Administration > Organization (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Organization')
BEGIN
    DECLARE @adminId4 UNIQUEIDENTIFIER;
    SELECT @adminId4 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Administration' AND [ParentId] IS NULL;
    IF @adminId4 IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Organization', '/org-units', @adminId4, 4);
END

-- Administration > Apps (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Apps')
BEGIN
    DECLARE @adminId5 UNIQUEIDENTIFIER;
    SELECT @adminId5 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Administration' AND [ParentId] IS NULL;
    IF @adminId5 IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Apps', '/apps', @adminId5, 5);
END

-- Settings (parent)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Settings' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'Settings', NULL, NULL, 4);

-- Settings > Resources (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Resources')
BEGIN
    DECLARE @settingsId UNIQUEIDENTIFIER;
    SELECT @settingsId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Settings' AND [ParentId] IS NULL;
    IF @settingsId IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Resources', '/resources', @settingsId, 1);
END

-- Settings > Tenants (child)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Tenants')
BEGIN
    DECLARE @settingsId2 UNIQUEIDENTIFIER;
    SELECT @settingsId2 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Settings' AND [ParentId] IS NULL;
    IF @settingsId2 IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Tenants', '/tenants', @settingsId2, 2);
END
