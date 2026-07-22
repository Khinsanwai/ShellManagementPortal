-- Add Audit Logs menu item under System Management

-- Ensure System Management parent exists
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'System Management' AND [ParentId] IS NULL)
    INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
    VALUES (NEWID(), 'System Management', NULL, NULL, 5);

-- Add Audit Logs under System Management
IF NOT EXISTS (SELECT 1 FROM [appshell].[MenuItem] WHERE [Name] = 'Audit Logs')
BEGIN
    DECLARE @sysParentId UNIQUEIDENTIFIER;
    SELECT @sysParentId = [Id] FROM [appshell].[MenuItem] WHERE [Name] = 'System Management' AND [ParentId] IS NULL;
    IF @sysParentId IS NOT NULL
        INSERT INTO [appshell].[MenuItem] ([Id], [Name], [Url], [ParentId], [MenuOrder])
        VALUES (NEWID(), 'Audit Logs', '/audit-logs', @sysParentId, 1);
END
