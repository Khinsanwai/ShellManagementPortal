-- Menu Restructure for Shell Management Portal
-- Reorganizes menus into: Identity Management, Organization Management,
-- Application Management, Authorization Management, System Management

-- ============================================================
-- 1. IDENTITY MANAGEMENT (parent)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Identity Management' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'Identity Management', NULL, NULL, 1);

-- Move User List under Identity Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'User List')
BEGIN
    DECLARE @identityId1 UNIQUEIDENTIFIER;
    SELECT @identityId1 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Identity Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @identityId1, [MenuOrder] = 1 WHERE [Name] = 'User List' AND @identityId1 IS NOT NULL;
END

-- Move User Groups under Identity Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'User Groups')
BEGIN
    DECLARE @identityId2 UNIQUEIDENTIFIER;
    SELECT @identityId2 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Identity Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @identityId2, [MenuOrder] = 2 WHERE [Name] = 'User Groups' AND @identityId2 IS NOT NULL;
END

-- Move Roles under Identity Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Roles')
BEGIN
    DECLARE @identityId3 UNIQUEIDENTIFIER;
    SELECT @identityId3 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Identity Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @identityId3, [MenuOrder] = 3 WHERE [Name] = 'Roles' AND @identityId3 IS NOT NULL;
END

-- Move Claims under Identity Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Claims')
BEGIN
    DECLARE @identityId4 UNIQUEIDENTIFIER;
    SELECT @identityId4 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Identity Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @identityId4, [MenuOrder] = 4 WHERE [Name] = 'Claims' AND @identityId4 IS NOT NULL;
END

-- Move Tenants under Identity Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Tenants')
BEGIN
    DECLARE @identityId5 UNIQUEIDENTIFIER;
    SELECT @identityId5 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Identity Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @identityId5, [MenuOrder] = 5 WHERE [Name] = 'Tenants' AND @identityId5 IS NOT NULL;
END

-- ============================================================
-- 2. ORGANIZATION MANAGEMENT (parent)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Organization Management' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'Organization Management', NULL, NULL, 2);

-- Move Organization under Organization Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Organization')
BEGIN
    DECLARE @orgId1 UNIQUEIDENTIFIER;
    SELECT @orgId1 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Organization Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @orgId1, [MenuOrder] = 1 WHERE [Name] = 'Organization' AND @orgId1 IS NOT NULL;
END

-- Move Institutions under Organization Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Institutions')
BEGIN
    DECLARE @orgId2 UNIQUEIDENTIFIER;
    SELECT @orgId2 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Organization Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @orgId2, [MenuOrder] = 2 WHERE [Name] = 'Institutions' AND @orgId2 IS NOT NULL;
END

-- ============================================================
-- 3. APPLICATION MANAGEMENT (parent)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Application Management' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'Application Management', NULL, NULL, 3);

-- Move Apps under Application Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Apps')
BEGIN
    DECLARE @appId1 UNIQUEIDENTIFIER;
    SELECT @appId1 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Application Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @appId1, [MenuOrder] = 1 WHERE [Name] = 'Apps' AND @appId1 IS NOT NULL;
END

-- ============================================================
-- 4. AUTHORIZATION MANAGEMENT (parent)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Authorization Management' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'Authorization Management', NULL, NULL, 4);

-- Move Menu Items under Authorization Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Menu Items')
BEGIN
    DECLARE @authId1 UNIQUEIDENTIFIER;
    SELECT @authId1 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Authorization Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @authId1, [MenuOrder] = 1 WHERE [Name] = 'Menu Items' AND @authId1 IS NOT NULL;
END

-- Move Resources under Authorization Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Resources')
BEGIN
    DECLARE @authId2 UNIQUEIDENTIFIER;
    SELECT @authId2 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Authorization Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @authId2, [MenuOrder] = 2 WHERE [Name] = 'Resources' AND @authId2 IS NOT NULL;
END

-- Move User Menu Assignment under Authorization Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'User Menu Assignment')
BEGIN
    DECLARE @authId3 UNIQUEIDENTIFIER;
    SELECT @authId3 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Authorization Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @authId3, [MenuOrder] = 3 WHERE [Name] = 'User Menu Assignment' AND @authId3 IS NOT NULL;
END

-- Add Group Menu Assignment under Authorization Management (if not exists)
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Group Menu Assignment')
BEGIN
    DECLARE @authId4 UNIQUEIDENTIFIER;
    SELECT @authId4 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'Authorization Management' AND [ParentId] IS NULL;
    IF @authId4 IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Group Menu Assignment', '/group-menu-assign', @authId4, 4);
END

-- ============================================================
-- 5. SYSTEM MANAGEMENT (parent)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'System Management' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'System Management', NULL, NULL, 5);

-- Add Audit Logs under System Management
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Audit Logs')
BEGIN
    DECLARE @sysId1 UNIQUEIDENTIFIER;
    SELECT @sysId1 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'System Management' AND [ParentId] IS NULL;
    IF @sysId1 IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Audit Logs', '/audit-logs', @sysId1, 1);
END

-- Move Settings under System Management
IF EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Settings')
BEGIN
    DECLARE @sysId2 UNIQUEIDENTIFIER;
    SELECT @sysId2 = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'System Management' AND [ParentId] IS NULL;
    UPDATE [appshell].[MenuItem] SET [ParentId] = @sysId2, [MenuOrder] = 2 WHERE [Name] = 'Settings' AND @sysId2 IS NOT NULL;
END

-- ============================================================
-- CLEANUP: Remove old parent menus that are now empty
-- ============================================================
-- Remove old 'Users' parent if it has no children
DELETE FROM [appshell].[MenuItem]
WHERE [Name] = 'Users' AND [ParentId] IS NULL
AND NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] c WHERE c.[ParentId] = [appshell].[MenuItem].[Id]);

-- Remove old 'Administration' parent if it has no children
DELETE FROM [appshell].[MenuItem]
WHERE [Name] = 'Administration' AND [ParentId] IS NULL
AND NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] c WHERE c.[ParentId] = [appshell].[MenuItem].[Id]);
